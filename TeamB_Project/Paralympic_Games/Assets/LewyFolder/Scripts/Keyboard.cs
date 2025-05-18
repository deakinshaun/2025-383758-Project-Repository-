using UnityEngine;


public class Keyboard : MonoBehaviour
{
    public TMPro.TMP_InputField inputField; // Reference to the input field
    public GameObject normalButton; // Reference to the keyboard GameObject
    public GameObject capsButtons; // Reference to the keyboard GameObject
    private bool caps; // Flag to check if caps lock is on





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        caps = false; // Initialize caps lock to false

    }

    public void InsertChar(string c)
    {
        // Insert the character into the input field
        inputField.text += c;
    }

    public void DeleteChar()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);

        }
    }

    public void InsertSpace()
    {
        // Insert a space into the input field
        inputField.text += " ";
    }

    public void CapsPressed()
    {
        // Toggle caps lock
        // Set the keyboard to the appropriate state
        if (!caps)
        {
            normalButton.SetActive(false);
            capsButtons.SetActive(true);
            caps = true;
        }
        else
        {
            normalButton.SetActive(true);
            capsButtons.SetActive(false);
            caps = false;
        }
    }

}
