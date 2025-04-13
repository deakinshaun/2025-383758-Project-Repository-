using UnityEngine;
using TMPro;

public class InstructionManager : MonoBehaviour
{
    public TMP_Text instructionText;

    private int currentStepIndex = 0;

    private string[] steps = new string[]
    {
        "Step 1: Place the base",
        "Step 2: Attach the column",
        "Step 3: Connect the top panel",
        "Step 4: Tighten the screws",
        "Step 5: Finish and test"
    };

    void Start()
    {
        ShowStep(currentStepIndex);
    }

    public void ShowStep(int index)
    {
        if (index >= 0 && index < steps.Length)
        {
            Debug.Log("Showing step: " + steps[index]);
            instructionText.text = steps[index];
        }
    }


    public void NextStep()
    {
        Debug.Log("NextStep() called!");

        if (currentStepIndex < steps.Length - 1)
        {
            currentStepIndex++;
            ShowStep(currentStepIndex);
        }
    }


    public void ResetInstructions()
    {
        currentStepIndex = 0;
        ShowStep(currentStepIndex);
        Debug.Log("Instructions reset to beginning.");
    }
}
