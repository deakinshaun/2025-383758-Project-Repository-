using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class MarkerDetectionHandler : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject batteryPrefab;
    public GameObject coolantPrefab;
    public GameObject enginePrefab;

    private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        prefabDict.Add("Battery", batteryPrefab);
        prefabDict.Add("Coolant", coolantPrefab);
        prefabDict.Add("Engine", enginePrefab);
    }

    void OnEnable() => imageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => imageManager.trackedImagesChanged -= OnChanged;

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added)
            UpdateImage(img);
        foreach (var img in args.updated)
            UpdateImage(img);
        foreach (var img in args.removed)
        {
            if (spawnedPrefabs.ContainsKey(img.referenceImage.name))
                spawnedPrefabs[img.referenceImage.name].SetActive(false);
        }
    }

    void UpdateImage(ARTrackedImage image)
    {
        string name = image.referenceImage.name;

        if (!spawnedPrefabs.ContainsKey(name))
        {
            var go = Instantiate(prefabDict[name], image.transform.position, image.transform.rotation);
            go.transform.parent = image.transform;
            spawnedPrefabs[name] = go;
        }
        else
        {
            var go = spawnedPrefabs[name];
            go.transform.position = image.transform.position;
            go.transform.rotation = image.transform.rotation;
            go.SetActive(true);
        }
    }
}
