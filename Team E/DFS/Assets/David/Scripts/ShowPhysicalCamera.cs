using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowPhysicalCamera : MonoBehaviour {

    public Material camTexMaterial;
    public Camera camera;

    private WebCamTexture webcamTexture;

    void Start () {
        webcamTexture = new WebCamTexture ();

        camTexMaterial.mainTexture = webcamTexture;
        webcamTexture.Play ();
    }

    void Update ()
    {
        //plane infront of camera
        float pos = (camera.nearClipPlane + 0.01f);

        transform.position = camera.transform.position + camera.transform.forward * pos;

        // Scale plane to for view
        float h = Mathf.Tan (camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2.0f;

        transform.localScale = new Vector3(h * camera.aspect, h, 1.0f);
    }
}