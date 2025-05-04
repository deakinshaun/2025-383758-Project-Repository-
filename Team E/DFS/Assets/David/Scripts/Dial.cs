using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dial : MonoBehaviour
{
    // Basic settings
    public float rotationSensitivity = 2.0f;
    public float minAngle = -180f;
    public float maxAngle = 180f;
    public bool useAngleLimits = false;
    public bool debugMode = true;
    
    // Reference object settings
    public Transform referenceObject;
    
    // Rotation value (0-359)
    private int rotationValue = 0;

    // References to interaction components
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Transform interactorTransform;
    private Quaternion initialRotation;
    private Quaternion initialInteractorRotation;
    private bool isGrabbed = false;
    
    // Store original position and rotation to maintain consistency
    private Vector3 originalPosition;
    private float originalXRotation;
    private float originalYRotation;
    private float originalZRotation;
    
    // Current X rotation
    private float currentXRotation;
    
    // Original rotation of reference object
    private Quaternion originalReferenceRotation;

    void Start()
    {
        // Store  original position and rotation
        originalPosition = transform.position;
        Vector3 originalEuler = transform.rotation.eulerAngles;
        originalXRotation = originalEuler.x;
        originalYRotation = originalEuler.y;
        originalZRotation = originalEuler.z;
        currentXRotation = originalEuler.x;
        
        // Initialize rotation value based on starting X rotation
        rotationValue = Mathf.RoundToInt(currentXRotation) % 360;
        if (rotationValue < 0) rotationValue += 360;
        
        if (referenceObject != null)
        {
            originalReferenceRotation = referenceObject.rotation;
        }
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            
            // Configure  dial behavior
            grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Instantaneous;
            grabInteractable.trackPosition = false;
            grabInteractable.trackRotation = false;
        }
        
        grabInteractable.selectEntered.AddListener(StartGrab);
        grabInteractable.selectExited.AddListener(EndGrab);
        
        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
            collider.height = 0.05f;
            collider.radius = 0.1f;
            collider.direction = 0; // X-axis aligned
        }
        
        // Add  kinematic rigidbody to stop movmeent
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
        
       
    }

    private void StartGrab(SelectEnterEventArgs args)
    {
        // Store the initial rotations when grabbed
        isGrabbed = true;
        interactorTransform = args.interactorObject.transform;
        initialRotation = transform.rotation;
        initialInteractorRotation = interactorTransform.rotation;
        
        if (debugMode)
        {
            Debug.Log("Dial grabbed. Initial controller rotation: " + initialInteractorRotation.eulerAngles);
        }
    }

    private void EndGrab(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactorTransform = null;
        
        if (debugMode)
        {
            Debug.Log("Dial released. Final X rotation: " + currentXRotation + ", Value: " + rotationValue);
        }
    }

    void Update()
    {
        // Always maintain the original position
        transform.position = originalPosition;
        
        if (isGrabbed && interactorTransform != null)
        {
            // Track controller rotation specifically and change the sign to reverse direction
            float deltaXRotation = CalculateControllerTwistAngle();
            
            // Apply sensitivity
            deltaXRotation *= rotationSensitivity;
            
            // Apply the rotation to our current X rotation
            currentXRotation += deltaXRotation;
            
            // Apply limits if needed
            if (useAngleLimits)
            {
                currentXRotation = Mathf.Clamp(currentXRotation, minAngle, maxAngle);
            }
            
            float normalizedRotation = currentXRotation % 360;
            if (normalizedRotation < 0) normalizedRotation += 360;
            rotationValue = Mathf.RoundToInt(normalizedRotation);
            
            //maintains Y and Z while only changing X
            Quaternion newRotation = Quaternion.Euler(currentXRotation, originalYRotation, originalZRotation);
            
            // Apply   rotation
            transform.rotation = newRotation;
            
            //  apply rotation to reference object if assigned
            if (referenceObject != null)
            {
                // Get the original reference rotation
                Vector3 refEuler = originalReferenceRotation.eulerAngles;
                
                // Calculate how much the dial has rotated from its original position
                float rotationDelta = currentXRotation - originalXRotation;
                
                // Apply that same rotation to the reference object's X axis
                refEuler.x = originalReferenceRotation.eulerAngles.x + rotationDelta;
                
                // Apply the new rotation
                Quaternion refRotation = Quaternion.Euler(refEuler);
                referenceObject.rotation = refRotation;
                
                if (debugMode && Mathf.Abs(rotationDelta) > 0.1f)
                {
                    Debug.Log("Reference Object X rotation: " + refEuler.x);
                }
            }
            
            // Print the rotation value to console
            if (debugMode && Mathf.Abs(deltaXRotation) > 0.1f)
            {
                Debug.Log("Dial Value: " + rotationValue + " (X rotation: " + currentXRotation + ")");
            }
            
            // Update initial interactor rotation to prevent accumulation
            initialInteractorRotation = interactorTransform.rotation;
        }
    }
    
    private float CalculateControllerTwistAngle()
    {
        //track controller rotation around the right vector for X value
        
        // Get the controller's up vector at start and now
        Vector3 initialUp = initialInteractorRotation * Vector3.up;
        Vector3 currentUp = interactorTransform.rotation * Vector3.up;
        
        Vector3 rightAxis = interactorTransform.right;
        
        // Calculate the signed angle between these up vectors around the right axis
        float twistAngle = Vector3.SignedAngle(initialUp, currentUp, rightAxis);
        
        return twistAngle;
    }
    
    // Public method to get the current rotation value (0-359)
    public int GetRotationValue()
    {
        return rotationValue;
    }
}