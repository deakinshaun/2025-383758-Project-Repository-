using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    public GameObject arSession;
    public GameObject xrOrigin;
    public ARTrackedImageManager trackedImageManager;

    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject mainCamera;
    public void StartAR()
    {
        arSession.SetActive(true);
        xrOrigin.SetActive(true);
        mainCamera.SetActive(false);
       
        StartCoroutine(EnableTrackingDelayed());

        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        
    }

    private System.Collections.IEnumerator EnableTrackingDelayed()
    {
        yield return new WaitForSeconds(.4f); 
        trackedImageManager.enabled = true;
    }

    public void BackButton()
    {
       

        trackedImageManager.enabled = false;
        arSession.SetActive(false);
        xrOrigin.SetActive(false);
        mainCamera.SetActive(true);

      
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
    }
}
