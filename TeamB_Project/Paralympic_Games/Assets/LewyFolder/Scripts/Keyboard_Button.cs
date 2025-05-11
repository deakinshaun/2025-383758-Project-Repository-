using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Keyboard_Button : MonoBehaviour
{
    Keyboard keyboard; // Reference to the Keyboard script
    TextMeshProUGUI buttonText; // Reference to the TextMeshProUGUI component



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = GetComponentInParent<Keyboard>(); // Get the Keyboard component from the parent object
        buttonText = GetComponentInChildren<TextMeshProUGUI>(); // Get the TextMeshProUGUI component from the child object
        if (buttonText.text.Length == 1)
        {
            NameToButtonText(); // Call the method to set the button text
            GetComponentInChildren<VRButton>().onRelease.AddListener(delegate { keyboard.InsertChar(buttonText.text); }); 
            // Call the InsertChar method from the Keyboard script
        }

    }
    public void NameToButtonText()
    {
        buttonText.text = gameObject.name; // Set the button text to the name of the GameObject
    }
}
