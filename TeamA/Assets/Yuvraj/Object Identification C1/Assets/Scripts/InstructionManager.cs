using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public Button nextButton;
    public Button previousButton;

    private Dictionary<string, List<string>> instructions;
    private string currentObject;
    private int instructionIndex;

    private void Start()
    {
        Debug.Log("Instruction Manager Initialized");

        // Initialize instructions for each object
        instructions = new Dictionary<string, List<string>>()
        {
            {
                "Battery", new List<string>()
                {
                    "Remove the ends properly.",
                    "Lift the battery carefully.",
                    "Check the battery terminals."
                }
            },
            {
                "Coolant", new List<string>()
                {
                    "Open the coolant cap.",
                    "Check the coolant level.",
                    "Close the coolant cap securely."
                }
            },
            {
                "Engine", new List<string>()
                {
                    "Ensure engine is off.",
                    "Check engine oil level.",
                    "Secure the engine cover."
                }
            }
        };

        // Set initial UI state
        instructionText.text = "Scan the Object";
        nextButton.onClick.AddListener(NextInstruction);
        previousButton.onClick.AddListener(PreviousInstruction);

        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);

        Debug.Log("Instruction Manager Setup Complete");
    }

    // Called from the Marker Tracking script
    public void SetObject(string objectName)
    {
        Debug.Log("SetObject called with: " + objectName);

        currentObject = objectName;
        instructionIndex = 0;

        if (instructions.ContainsKey(objectName))
        {
            instructionText.text = instructions[objectName][instructionIndex];
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);
            Debug.Log("Instructions set for: " + objectName);
        }
        else
        {
            instructionText.text = "No instructions available.";
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
        }
    }

    // Next Instruction Button Click
    private void NextInstruction()
    {
        if (currentObject == null || !instructions.ContainsKey(currentObject))
            return;

        instructionIndex = Mathf.Min(instructionIndex + 1, instructions[currentObject].Count - 1);
        instructionText.text = instructions[currentObject][instructionIndex];
    }

    // Previous Instruction Button Click
    private void PreviousInstruction()
    {
        if (currentObject == null || !instructions.ContainsKey(currentObject))
            return;

        instructionIndex = Mathf.Max(instructionIndex - 1, 0);
        instructionText.text = instructions[currentObject][instructionIndex];
    }
}
