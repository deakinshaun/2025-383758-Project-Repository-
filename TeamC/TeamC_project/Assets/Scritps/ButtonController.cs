using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Input;


public class ButtonController : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject MainCanvas;
    private XROrigin Origin;
    private UnityEngine.Pose placementPose;
    private bool menuOpen = false;

    public void Start()
    {
        Origin = FindAnyObjectByType<XROrigin>();
        MainCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
    }

    void Update()
    {
        if(menuOpen == false)
        {
            UpdatePlacement();
        }
    }

    public void UpdatePlacement()
    {
        var center = Camera.main.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        var hits = new List<ARRaycastHit>();

        Origin.GetComponent<ARRaycastManager>().Raycast(center,hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneEstimated);
        ;
        if(hits.Count > 0)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }

    }
    public void openMenu()
   {
        if (MenuCanvas != null)
        {
            MenuCanvas.SetActive(true);
            MainCanvas.SetActive(false);
            MenuCanvas.transform.position = placementPose.position;
            MenuCanvas.transform.rotation = placementPose.rotation;
        }
   }

      public void closeMenu()
   {
        if (MenuCanvas != null)
        {
            MenuCanvas.SetActive(false);
            MainCanvas.SetActive(true);
        }
   }
}
