using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

public class AROnboardingManager : MonoBehaviour
{
    // Define the onboarding phases.
    public enum OnboardingPhase
    {
        Greeting,      // Show the greeting prompt.
        Instruction,   // Display movement instructions.
        Recording,     // Record movement data.
        Completed      // Recording complete.
    }
    
    public enum ArmTypes
    {
        Left,
        Right
    }
    public enum MotionTypes
    {
        Vertical,
        Horizontal
    }

    [Header("UI Elements")]
    [Tooltip("Greeting panel to show at start.")]
    public GameObject greetingPrompt;
    [Tooltip("End panel to show when recording is complete.")]
    public GameObject endPrompt;
    [Tooltip("Text element to display instructions and results.")]
    public TextMeshProUGUI instructionText;
    [Tooltip("Start button in the greeting prompt.")]
    public Button startButton;

    [Header("AR Components")]
    [Tooltip("The AR Camera from the XR Origin.")]
    public Camera arCamera;

    [Header("User Settings")]
    [Tooltip("Arm length in meters. This value is used to calculate the degrees moved.")]
    public float armLength = 0.6f; // Default arm length in meters.
    [Tooltip("Dropdown for selecting arm type (e.g., Left Shoulder, Right Shoulder).")]
    public GameObject ArmType; // Expected to have a TMP_Dropdown component.
    [Tooltip("Dropdown for selecting motion type (Vertical or Horizontal).")]
    public GameObject MotionType; // Expected to have a TMP_Dropdown component.

    // Internal variables for storing selected options.
    private string armType;
    private string motionType;

    // Internal variables for phase management and recording.
    private OnboardingPhase currentPhase = OnboardingPhase.Greeting;
    private Vector3 startPosition;
    private float recordedDistance = 0f;
    private bool isRecording = false;
    private Coroutine phaseCoroutine;
    
    // For timing data.
    private float startTime;
    private float endTime;
    
    public GameObject humanoidPrefab; // Prefab for the humanoid model.

    void Start()
    {
        // Initialize onboarding with greeting.
        currentPhase = OnboardingPhase.Greeting;
        if (greetingPrompt != null)
            greetingPrompt.SetActive(true);
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        // Assign button listener.
        startButton.onClick.AddListener(OnStartButtonClicked);

        // Ensure AR Camera is assigned.
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
                Debug.LogError("AR Camera not assigned and no Main Camera found!");
        }
    }

    void HandleStateChange()
    {
        switch (currentPhase)
        {
            case OnboardingPhase.Greeting:
                if (greetingPrompt != null)
                {
                    endPrompt.SetActive(false);
                    greetingPrompt.SetActive(true);
                }
                break;
            case OnboardingPhase.Instruction:
                if (greetingPrompt != null)
                {
                    if (humanoidPrefab != null)
                        humanoidPrefab.SetActive(true);
                    greetingPrompt.SetActive(false);
                }
                break;
            case OnboardingPhase.Recording:
                if (instructionText != null)
                    instructionText.gameObject.SetActive(true);
                break;
            case OnboardingPhase.Completed:
                if (endPrompt != null)
                    endPrompt.SetActive(true);
                break;
        }
    }

    public void animateHumanoid()
    {
        // Assuming humanoidPrefab has an Animator component to handle animations.
        Animator animator = humanoidPrefab.GetComponent<Animator>();
        if (animator != null)
        {
            if (armType == "Left Shoulder")
            {
                if (motionType == "Vertical")
                {
                    animator.SetTrigger("LeftVertical");
                }
                else if (motionType == "Horizontal")
                {
                    animator.SetTrigger("LeftHorizontal");
                }
            }
            else if (armType == "Right Shoulder")
            {
                if (motionType == "Vertical")
                {
                    animator.SetTrigger("RightVertical");
                }
                else if (motionType == "Horizontal")
                {
                    animator.SetTrigger("RightHorizontal");
                }
            }
            else
            {
                Debug.LogError("Invalid arm type selected.");
                return;
            }
        }
        else
        {
            Debug.LogError("Animator component not found on humanoid prefab.");
        }
    }

    // Called when the user clicks the Start button.
    void OnStartButtonClicked()
    {
        // Get the arm type and motion type from the dropdown values.
        armType = ArmType.GetComponent<TMP_Dropdown>().options[ArmType.GetComponent<TMP_Dropdown>().value].text;
        motionType = MotionType.GetComponent<TMP_Dropdown>().options[MotionType.GetComponent<TMP_Dropdown>().value].text;

        // Set the arm length based on the selected arm type.
        if (armType == "Left Shoulder")
        {
            armLength = 0.6f; // Example value for left arm.
        }
        else if (armType == "Right Shoulder")
        {
            armLength = 0.7f; // Example value for right arm.
        }
        else
        {
            Debug.LogError("Invalid arm type selected.");
            return;
        }

        // Update instructions based on motion type.
        string motionInstruction = "";
        if (motionType == "Vertical")
        {
            motionInstruction = "Move your phone up or down.\nTap the screen when done.";
        }
        else if (motionType == "Horizontal")
        {
            motionInstruction = "Move your phone left or right.\nTap the screen when done.";
        }
        else
        {
            Debug.LogError("Invalid motion type selected.");
            return;
        }

        // Transition from Greeting to Instruction phase.
        if (greetingPrompt != null)
            greetingPrompt.SetActive(false);
        currentPhase = OnboardingPhase.Instruction;
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);
            instructionText.text = motionInstruction;
        }

        // After a short delay, transition to Recording phase.
        if (phaseCoroutine != null)
            StopCoroutine(phaseCoroutine);
        phaseCoroutine = StartCoroutine(BeginRecordingAfterDelay(5f));
    }

    // Wait a specified delay then start recording.
    IEnumerator BeginRecordingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        instructionText.text = "Hold your phone steady in the arm you are going to move.\nRecording will start now...";
        StartRecording();
    }

    // Start the recording phase.
    void StartRecording()
    {
        currentPhase = OnboardingPhase.Recording;
        startPosition = arCamera.transform.position;
        recordedDistance = 0f;
        isRecording = true;
        startTime = Time.time;  // Record start time
    }

    void Update()
    {
        // Handle state changes based on the current phase.
        HandleStateChange();

        if (humanoidPrefab != null && humanoidPrefab.activeSelf)
        {
            animateHumanoid();
        }

        // During Recording phase, update displacement and check for touch input.
        if (currentPhase == OnboardingPhase.Recording && isRecording && arCamera != null)
        {
            Vector3 currentPos = arCamera.transform.position;
            // Choose displacement axis based on selected motion type.
            if (motionType == "Vertical")
            {
                recordedDistance = currentPos.y - startPosition.y;
            }
            else // Assume Horizontal
            {
                recordedDistance = currentPos.x - startPosition.x;
            }

            // Calculate the angle in degrees based on arm length.
            float angleDegrees = (armLength > 0f) ? (recordedDistance / armLength) * Mathf.Rad2Deg : 0f;
            // Calculate elapsed time.
            float elapsedTime = Time.time - startTime;
            // Calculate speed (m/s) - using absolute value of distance.
            float speed = (elapsedTime > 0f) ? Mathf.Abs(recordedDistance) / elapsedTime : 0f;

            if (instructionText != null)
            {
                string movementAxis = (motionType == "Vertical") ? "vertical" : "horizontal";
                instructionText.text = $"Recording {movementAxis} movement...\n" +
                                         $"Distance: {recordedDistance:F2} m\n" +
                                         $"Angle: {angleDegrees:F2}°\n" +
                                         $"Elapsed time: {elapsedTime:F2} s\n" +
                                         $"Average speed: {speed:F2} m/s\n" +
                                         $"Tap the screen to finish.";
            }

            // Stop recording when user taps the screen using the new Input System.
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                StopRecording();
            }
        }
    }

    // Stop the recording and display the final distance, angle, elapsed time, and speed.
    void StopRecording()
    {
        isRecording = false;
        currentPhase = OnboardingPhase.Completed;
        endTime = Time.time; // Record end time

        float duration = endTime - startTime;
        float finalAngle = (armLength > 0f) ? (recordedDistance / armLength) * Mathf.Rad2Deg : 0f;
        float averageSpeed = (duration > 0f) ? Mathf.Abs(recordedDistance) / duration : 0f;

        // Disable the humanoid animation if active.
        if (humanoidPrefab != null)
            humanoidPrefab.SetActive(false);

        if (instructionText != null)
        {
            instructionText.text = "Recording stopped!\n" +
                                     $"Total distance moved: {recordedDistance:F2} m\n" +
                                     $"Total angle: {finalAngle:F2}°\n" +
                                     $"Time elapsed: {duration:F2} s\n" +
                                     $"Average speed: {averageSpeed:F2} m/s";
        }

        // Also update the end prompt panel if available.
        if (endPrompt != null)
        {
            TextMeshProUGUI endText = endPrompt.GetComponentInChildren<TextMeshProUGUI>();
            if (endText != null)
            {
                string movementAxis = (motionType == "Vertical") ? "vertical" : "horizontal";
                endText.text = $"Recording complete!\n" +
                               $"Distance: {recordedDistance:F2} m\n" +
                               $"Angle: {finalAngle:F2}°\n" +
                               $"Time: {duration:F2} s\n" +
                               $"Speed: {averageSpeed:F2} m/s";
            }

            Button restartButton = endPrompt.GetComponentInChildren<Button>();
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartOnboarding);
                restartButton.gameObject.SetActive(true);
            }
        }
    }

    // Optional: Restart the onboarding process.
    public void RestartOnboarding()
    {
        currentPhase = OnboardingPhase.Greeting;
        isRecording = false;
        recordedDistance = 0f;
        if (greetingPrompt != null)
            greetingPrompt.SetActive(true);
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
            instructionText.text = "";
        }
        if (endPrompt != null)
            endPrompt.SetActive(false);
    }
}
