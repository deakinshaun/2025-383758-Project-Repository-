using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipOrder : MonoBehaviour {

    public Material phyCamMaterial;
    public Material virCamMaterial;
    public GameObject transition;
    public GameObject portal;

    public bool inPhysical = true;

    void Update () {

 Debug.Log("In Update() method of FlipOrder class.");
        if (inPhysical)
        {
            phyCamMaterial.SetInt ("_Stencil_Level", 1);
            virCamMaterial.SetInt ("_Stencil_Level", 0);
        }
        else
        {
            phyCamMaterial.SetInt ("_Stencil_Level", 0);
            virCamMaterial.SetInt ("_Stencil_Level", 1);
        }
    }
void OnCollisionEnter(Collision collision) {
    Debug.Log("COLLISION with: " + collision.gameObject.name);
}
void OnTriggerEnter(Collider other) {
    Debug.Log("TRIGGER with: " + other.gameObject.name);
    
    if (other.gameObject == portal)
    {
        Debug.Log("Object is the portal!");
        transition.SetActive(true);
        Debug.Log("Changing universe");
        inPhysical = !inPhysical;
    }
    else
    {
        Debug.Log("Object is NOT the portal.");
    }
}


    public void OnTriggerExit (Collider other)
    {
        transition.SetActive (false);
    }
}