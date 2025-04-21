using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpatializer : MonoBehaviour {

public List<AudioSource> sources;
    
    public float dropoffDistanceConstant = 0.8f;
    
    public float attenuationFactor = 1.5f; // Should be 2.0f for realistic attenuation
    
    public float speedOfSound = 330.0f;
    
    void Update () {
        foreach (AudioSource source in sources)
        {
            // Volume effects - attenuate based on distance
            GameObject sourceObject = source.gameObject;
            float distance = Vector3.Distance(sourceObject.transform.position, transform.position);
            source.volume = 1.0f / Mathf.Pow(dropoffDistanceConstant * distance, attenuationFactor);
            
            // Doppler effects - pitch shift based on relative motion
            // This assumes the avatar is moving and sound sources are stationary
            Vector3 sourceVelocity = Vector3.zero; // Sound sources are stationary in this example
            
            // Get velocity from a location tracking component if you have one
            // Otherwise, we can calculate a simple velocity based on frame-to-frame position changes
            Vector3 myVelocity = CalculateVelocity();
            
            Vector3 relativeVelocity = myVelocity - sourceVelocity;
            Vector3 directionBetweenMeAndSource = Vector3.Normalize(sourceObject.transform.position - transform.position);
            float relativeSpeed = Vector3.Dot(directionBetweenMeAndSource, relativeVelocity);
            
            // Apply Doppler effect to pitch
            source.pitch = (speedOfSound + relativeSpeed) / (speedOfSound - relativeSpeed);
        }
    }
    
    // Simple velocity calculation based on position change
    private Vector3 lastPosition;
    private Vector3 velocity = Vector3.zero;
    
    private Vector3 CalculateVelocity() {
        if (lastPosition == Vector3.zero) {
            lastPosition = transform.position;
            return Vector3.zero;
        }
        
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        return velocity;
    }
}