using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Android;
// TextToDrill module—using Google’s Gemini API
public class Record : MonoBehaviour, ISpeechToTextListener
{
    public TextMeshProUGUI SpeechText;
    public Button StartSpeechToTextButton, StopSpeechToTextButton;
    public Slider VoiceLevelSlider;
    private bool isRecording = false;
    public bool PreferOfflineRecognition;
    private string SpeechTextString;

    public GameObject continueButton;
    private float normalizedVoiceLevel;
    public TextToDrill textToDrill;
    public GameObject initMenu;
    private void Awake()
    {
        SpeechToText.Initialize("en-US");
        StopSpeechToTextButton.interactable = false;
        StartSpeechToTextButton.onClick.AddListener(StartSpeechToText);
        StopSpeechToTextButton.onClick.AddListener(StopSpeechToText);
        continueButton.SetActive(false);
        // continueButton.GetComponent<Button>().onClick.AddListener(ContinueToDrill);
    }
    private void ContinueToDrill()
    {
        // Call the method in TextToDrill to continue with the drill
        textToDrill.GetDrill(SpeechTextString);
        // Optionally, you can also reset the SpeechText to an empty string
        SpeechText.text = "";
        continueButton.SetActive(false);
        initMenu.SetActive(false);

    }

    private void Update()
    {
        StartSpeechToTextButton.interactable = SpeechToText.IsServiceAvailable(PreferOfflineRecognition) && !SpeechToText.IsBusy();
        StopSpeechToTextButton.interactable = SpeechToText.IsBusy();

        // You may also apply some noise to the voice level for a more fluid animation (e.g. via Mathf.PerlinNoise)
        // VoiceLevelSlider.value = Mathf.Lerp(VoiceLevelSlider.value, normalizedVoiceLevel, 15f * Time.unscaledDeltaTime);
    }

    public void ChangeLanguage(string preferredLanguage)
    {
        if (!SpeechToText.Initialize(preferredLanguage))
            SpeechText.text = "Couldn't initialize with language: " + preferredLanguage;
    }

    public void StartSpeechToText()
    {

        SpeechToText.RequestPermissionAsync((permission) =>
        {
            if (permission == SpeechToText.Permission.Granted)
            {
                if (SpeechToText.Start(this, preferOfflineRecognition: PreferOfflineRecognition))
                {
                    SpeechText.text = "";
                    // StopSpeechToTextButton.interactable = true;
                }
                else
                    SpeechText.text = "Couldn't start speech recognition session!";
            }
            else
                SpeechText.text = "Permission is denied!";
        });
    }

    public void StopSpeechToText()
    {
        // StopSpeechToTextButton.interactable = false;
        SpeechToText.ForceStop();
    }

    void ISpeechToTextListener.OnReadyForSpeech()
    {
        Debug.Log("OnReadyForSpeech");
    }

    void ISpeechToTextListener.OnBeginningOfSpeech()
    {
        Debug.Log("OnBeginningOfSpeech");
    }

    void ISpeechToTextListener.OnVoiceLevelChanged(float normalizedVoiceLevel)
    {
        // Note that On Android, voice detection starts with a beep sound and it can trigger this callback. You may want to ignore this callback for ~0.5s on Android.
        this.normalizedVoiceLevel = normalizedVoiceLevel;
    }

    void ISpeechToTextListener.OnPartialResultReceived(string spokenText)
    {
        Debug.Log("OnPartialResultReceived: " + spokenText);
        SpeechText.text = spokenText;
    }

    void ISpeechToTextListener.OnResultReceived(string spokenText, int? errorCode)
    {
        Debug.Log("OnResultReceived: " + spokenText + (errorCode.HasValue ? (" --- Error: " + errorCode) : ""));
        SpeechText.text = spokenText;
        normalizedVoiceLevel = 0f;
        // if we got not errors, we can show the continue button
        if (errorCode == null)
        {
            continueButton.SetActive(true);
            SpeechTextString = spokenText;
            // StopSpeechToTextButton.interactable = false;
        }
        else
        {
            continueButton.SetActive(false);
            // StopSpeechToTextButton.interactable = true;
        }

        // Recommended approach:
        // - If errorCode is 0, session was aborted via SpeechToText.Cancel. Handle the case appropriately.
        // - If errorCode is 9, notify the user that they must grant Microphone permission to the Google app and call SpeechToText.OpenGoogleAppSettings.
        // - If the speech session took shorter than 1 seconds (should be an error) or a null/empty spokenText is returned, prompt the user to try again (note that if
        //   errorCode is 6, then the user hasn't spoken and the session has timed out as expected).
    }
}
