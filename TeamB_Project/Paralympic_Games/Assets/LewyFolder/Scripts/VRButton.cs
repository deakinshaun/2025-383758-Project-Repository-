using UnityEngine;
using UnityEngine.Events;
public class VRButton : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    AudioSource sound;
    bool isPressed;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sound = GetComponent<AudioSource>(); // Get the AudioSource component attached to the GameObject
        isPressed = false; // Initialize the isPressed variable to false
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0); // Move the button up when pressed
            presser = other.gameObject; // Store the object that pressed the button 
            onPress.Invoke(); // Invoke the onPress event
            sound.Play(); // Play the sound
            isPressed = true; // Set isPressed to true to prevent multiple presses

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0, 0); // Move the button back down when released
            onRelease.Invoke(); // Invoke the onRelease event
            isPressed = false; // Reset isPressed to allow for future presses
        }
    }

    public void spawnSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // Create a new sphere GameObject
        sphere.transform.position = new Vector3(0, 1, 2); // Set the position of the sphere
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Set the scale of the sphere
        Rigidbody rb = sphere.AddComponent<Rigidbody>(); // Add a Rigidbody component to the sphere
        rb.useGravity = true; // Enable gravity for the sphere
    }
}