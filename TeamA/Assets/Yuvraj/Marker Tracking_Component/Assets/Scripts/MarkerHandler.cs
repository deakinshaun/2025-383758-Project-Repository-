using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class MarkerHandler : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;

    public GameObject batteryPrefab;
    public GameObject coolantPrefab;
    public GameObject enginePrefab;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        // Instantiate and disable all prefabs initially
        spawnedPrefabs["Battery"] = Instantiate(batteryPrefab, Vector3.zero, Quaternion.identity);
        spawnedPrefabs["Coolant"] = Instantiate(coolantPrefab, Vector3.zero, Quaternion.identity);
        spawnedPrefabs["Engine"] = Instantiate(enginePrefab, Vector3.zero, Quaternion.identity);

        foreach (var prefab in spawnedPrefabs.Values)
            prefab.SetActive(false);
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            string imageName = trackedImage.referenceImage.name;

            foreach (var kvp in spawnedPrefabs)
                kvp.Value.SetActive(false); // Disable all to avoid overlap

            if (trackedImage.trackingState == TrackingState.Tracking && spawnedPrefabs.ContainsKey(imageName))
            {
                GameObject prefab = spawnedPrefabs[imageName];
                prefab.SetActive(true);
                prefab.transform.position = trackedImage.transform.position;
                prefab.transform.rotation = trackedImage.transform.rotation;
            }
        }
    }
}
