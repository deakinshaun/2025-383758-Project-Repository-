using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using TMPro;

public class ARObjectSpawner : MonoBehaviour
{
    public static ARObjectSpawner arObjectSpawnerInstacne { get; private set; }
    public List<GameObject> objectsToSpawn;

    [SerializeField] public Dictionary<string, GameObject> trackedObjects = new Dictionary<string, GameObject>();
    private ARTrackedImageManager arTrackedImageManager;

    private GameObject lastSelected;
    public LayerMask interactableLayer;

    public TMP_Text debugText;

    void Awake()
    {
        if (arObjectSpawnerInstacne != null && arObjectSpawnerInstacne != this)
        {
            Destroy(this.gameObject);
            return;
        }

        arObjectSpawnerInstacne = this;

      
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void Start()
    {
        for (int i = 0; i < arTrackedImageManager.referenceLibrary.count; i++)
        {
            GameObject gameObject = Instantiate(objectsToSpawn[i]);
            gameObject.SetActive(false);
            trackedObjects[arTrackedImageManager.referenceLibrary[i].name] = gameObject;
        }
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
            
            UpdateObjectTransform(updatedImage);
        }

        foreach (var removedImage in args.removed)
        {
           
            RemovedObjects(removedImage);
        }
    }

    private void RemovedObjects(ARTrackedImage removedImage)
    {
        trackedObjects[removedImage.referenceImage.name].SetActive(false);
    }

    private void UpdateObjectTransform(ARTrackedImage updatedImage)
    {
        trackedObjects[updatedImage.referenceImage.name].transform.position = updatedImage.transform.position;
        trackedObjects[updatedImage.referenceImage.name].transform.rotation = updatedImage.transform.rotation;
    }

    private void SpawnObject(ARTrackedImage trackedImage)
    {
       trackedObjects[trackedImage.referenceImage.name].SetActive(true);
       trackedObjects[trackedImage.referenceImage.name].transform.localScale = new Vector3(1, 1, 1);
    }

    void Update()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.isPressed == false)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
            {
                GameObject selectedObject = hit.collider.gameObject;

                if (lastSelected != null && lastSelected != selectedObject)
                {
                    // remove previous highlight
                    HighlightObject(lastSelected, false);
                    
                }

                HighlightObject(selectedObject, true);
                lastSelected = selectedObject;

                debugText.text = lastSelected.name;
            }
        }
    }

    void HighlightObject(GameObject obj, bool highlight)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend)
        {
            if (highlight)
                rend.material.color = Color.yellow; // highlighted
            else
                rend.material.color = Color.white;  // default
        }
    }
}
