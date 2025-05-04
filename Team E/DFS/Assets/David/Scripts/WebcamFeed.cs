using UnityEngine;

public class WebcamFeed : MonoBehaviour
{
    public Material webcamMaterial;
    private WebCamTexture webcamTexture;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            Debug.Log("Webcam found: " + devices[0].name);

            webcamTexture = new WebCamTexture(devices[0].name);
            webcamTexture.Play();

            webcamMaterial.SetTexture("_BaseMap", webcamTexture);
        }
        else
        {
            Debug.LogWarning("No webcam found.");
        }
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            Debug.Log("Webcam width: " + webcamTexture.width + ", height: " + webcamTexture.height);
        }
    }
}
