using System;
using UnityEngine;


public class WheelInteractable : MonoBehaviour
{
    private float wheelRadius;

    [Range(0,0.5f)]
    [SerializeField] private float deselectionThreshold = 0.25f;

    private GameObject grabPoint;

    private void Start()
    {
        wheelRadius = GetComponent<SphereCollider>().radius;
    }
}