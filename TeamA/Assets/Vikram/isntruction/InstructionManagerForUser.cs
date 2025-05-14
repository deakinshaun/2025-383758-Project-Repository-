using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionManagerForUser : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public Button nextButton;
    public Button previousButton;
    public AudioManager audioManager;

    private string currentObject;
    private int instructionIndex;
    public static InstructionManagerForUser instructionManagerForUser { get; private set; }
    private void Awake()
    {
        if (instructionManagerForUser != null && instructionManagerForUser != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instructionManagerForUser = this;
    }

    private Dictionary<string, List<string>> instructions = new Dictionary<string, List<string>>()
    {
        {
            "1", new List<string>()
            {
                "Remove the ends properly.",
                "Lift the battery carefully.",
                "Check the battery terminals."
            }
        },
        {
            "2", new List<string>()
            {
                "Open the coolant cap.",
                "Check the coolant level.",
                "Close the coolant cap securely."
            }
        },
        {
            "", new List<string>()
            {
                "Ensure engine is off.",
                "Check engine oil level.",
                "Secure the engine cover."
            }
        }
    };

    private void Start()
    {
        nextButton.onClick.AddListener(NextInstruction);
        previousButton.onClick.AddListener(PreviousInstruction);
    }

    public void SetObject(string objectName)
    {
        currentObject = objectName;
        instructionIndex = 0;
        UpdateInstruction();
        Debug.Log("Object set to: " + currentObject);
    }

    public void NextInstruction() // Now Public
    {
        if (instructions.ContainsKey(ARManager.aRManager.nameOfThePart))
        {
            if (instructionIndex < instructions[currentObject].Count - 1)
            {
                instructionIndex++;
                Debug.Log("Next Instruction: " + instructionIndex);
                UpdateInstruction();
            }
            else
            {
                Debug.Log("Already at the last instruction.");
            }
        }
        else
        {
            Debug.LogError("No instructions available for the current object.");
        }
    }

    public void PreviousInstruction() // Now Public
    {
        if (instructions.ContainsKey(currentObject))
        {
            if (instructionIndex > 0)
            {
                instructionIndex--;
                Debug.Log("Previous Instruction: " + instructionIndex);
                UpdateInstruction();
            }
            else
            {
                Debug.Log("Already at the first instruction.");
            }
        }
        else
        {
            Debug.LogError("No instructions available for the current object.");
        }
    }

    private void UpdateInstruction()
    {
        if (instructions.ContainsKey(currentObject))
        {
            string instruction = instructions[currentObject][instructionIndex];
            instructionText.text = instruction;

            // Play corresponding audio
            string audioClipName = currentObject + "_Step" + (instructionIndex + 1);
            audioManager.PlayAudio(audioClipName);
            Debug.Log("Updated Instruction: " + instruction);
        }
        else
        {
            instructionText.text = "No instructions available.";
            Debug.LogError("No instructions available for the current object: " + currentObject);
        }
    }
}
