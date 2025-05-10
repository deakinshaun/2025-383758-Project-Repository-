using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class ShowObjectOnTrackable : MonoBehaviour
{
    public List<GameObject> ObjectsToPlace;
    public InstructionManager instructionManager; // Reference to InstructionManager

    private ARTrackedImageManager arTrackedManager;
    private Dictionary<string, GameObject> trackedObjects;

    private void Awake()
    {
        arTrackedManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        arTrackedManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        arTrackedManager.trackedImagesChanged -= OnImageChanged;
    }

    void Start()
    {
        trackedObjects = new Dictionary<string, GameObject>();
        for (int i = 0; i < arTrackedManager.referenceLibrary.count; i++)
        {
            GameObject go = Instantiate(ObjectsToPlace[i]);
            go.SetActive(false);
            trackedObjects[arTrackedManager.referenceLibrary[i].name] = go;
        }

        Debug.Log("AR Image Tracking Initialized");
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var addedImage in args.added)
        {
            Debug.Log("Tracked image added: " + addedImage.referenceImage.name);
            trackedObjects[addedImage.referenceImage.name].SetActive(true);
            instructionManager.SetObject(addedImage.referenceImage.name);
        }

        foreach (var updatedImage in args.updated)
        {
            trackedObjects[updatedImage.referenceImage.name].SetActive(true);
            trackedObjects[updatedImage.referenceImage.name].transform.position = updatedImage.transform.position;
            trackedObjects[updatedImage.referenceImage.name].transform.rotation = updatedImage.transform.rotation;
        }

        foreach (var removedImage in args.removed)
        {
            trackedObjects[removedImage.referenceImage.name].SetActive(false);
            instructionManager.instructionText.text = "Scan the Object"; // Reset text
            instructionManager.nextButton.gameObject.SetActive(false);
            instructionManager.previousButton.gameObject.SetActive(false);
        }
    }
}
