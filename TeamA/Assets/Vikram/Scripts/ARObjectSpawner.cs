using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

public class ARObjectSpawner : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; 
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private ARTrackedImageManager arTrackedImageManager;

    private GameObject lastSelected;
    public LayerMask interactableLayer;
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
        if (spawnedObjects.ContainsKey(trackedImage.referenceImage.name)) return; 

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

    //void Update()
    //{
    //    if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.isPressed == false)
    //        return;

    //    if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
    //    {
    //        Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
    //        Ray ray = Camera.main.ScreenPointToRay(touchPos);

    //        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
    //        {
    //            GameObject selectedObject = hit.collider.gameObject;

    //            if (lastSelected != null && lastSelected != selectedObject)
    //            {
    //                // remove previous highlight
    //                HighlightObject(lastSelected, false);
    //            }

    //            HighlightObject(selectedObject, true);
    //            lastSelected = selectedObject;
    //        }
    //    }
    //}

    //void HighlightObject(GameObject obj, bool highlight)
    //{
    //    Renderer rend = obj.GetComponent<Renderer>();
    //    if (rend)
    //    {
    //        if (highlight)
    //            rend.material.color = Color.yellow; // highlighted
    //        else
    //            rend.material.color = Color.white;  // default
    //    }
    //}
}
