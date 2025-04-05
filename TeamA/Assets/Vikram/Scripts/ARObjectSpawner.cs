using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class ARObjectSpawner : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; 
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private ARTrackedImageManager arTrackedImageManager;

    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnImageChange;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnImageChange;
    }

    private void OnImageChange(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            SpawnObject(trackedImage);
        }

        foreach (var updatedImage in args.updated)
        {
            UpdateObjectPosition(updatedImage);
        }

        foreach (var removedImage in args.removed)
        {
            RemoveObject(removedImage);
        }
    }

    void SpawnObject(ARTrackedImage trackedImage)
    {
        if (spawnedObjects.ContainsKey(trackedImage.referenceImage.name)) return; // Avoid duplicate spawns

        int index = GetIndexFromImageName(trackedImage.referenceImage.name);
        if (index == -1) return;

        GameObject obj = Instantiate(objectsToSpawn[index], trackedImage.transform.position, trackedImage.transform.rotation);
       
        spawnedObjects[trackedImage.referenceImage.name] = obj;
    }

    void UpdateObjectPosition(ARTrackedImage trackedImage)
    {
        if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject obj))
        {
            obj.transform.position = trackedImage.transform.position;
            obj.transform.rotation = trackedImage.transform.rotation;
        }
    }

    void RemoveObject(ARTrackedImage trackedImage)
    {
        if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject obj))
        {
            Destroy(obj);
            spawnedObjects.Remove(trackedImage.referenceImage.name);
        }
    }

    int GetIndexFromImageName(string imageName)
    {
        for (int i = 0; i < objectsToSpawn.Count; i++)
        {
            if (objectsToSpawn[i].name == imageName)
                return i;
        }
        return -1;
    }
}
