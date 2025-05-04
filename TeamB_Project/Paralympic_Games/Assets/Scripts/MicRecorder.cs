// using System.Collections.Generic;
// using UnityEngine;
// using LudicWorlds;
// using UnityEngine.InputSystem;


// public class MicRecorder : MonoBehaviour
// {
//     private const int CLIP_LENGTH = 20;
//     private const int CLIP_FREQUENCY = 16000;

//     [SerializeField] private InputActionReference leftActivateAction;
//     [SerializeField] private InputActionReference leftSelectAction;
//     [SerializeField] private InputActionReference rightActivateAction;
//     [Space]
//     [Header("Used to trim audio:")]
//     [SerializeField, Range(0.0f, 0.1f)] private float silenceThreshold = 0.02f; // Threshold to consider as silence
//     [SerializeField, Range(0.0f, 1.0f)] private float minSilenceLength = 0.5f; // Minimum length of silence to trim, in seconds
//     [Space]
//     [SerializeField] private RunWhisper RunWhisper; // This is the reference to the RunWhisper script

//     private AudioClip audioClip; //clip to which the microphone output is recorded
//     private AudioSource audioSource; //audio source attached to camera
//     private string deviceName;  //holds the name of the detected Microphone device
//     private bool isRecording;

//     void Start()
//     {
//         Debug.Log("* Hold down the Left Trigger to Record.");
//         Debug.Log("* Release the Left Trigger to stop Recording.");
//         Debug.Log("* Press the Right Trigger to Transcribe.");

//         //NOTE: make sure an Audio Source is attached to the main camera
//         audioSource = Camera.main.GetComponent<AudioSource>();

//         if (audioSource is null)
//         {
//             Debug.LogError("-> Camera AudioSource is NULL! :(");
//         }

//         if (Microphone.devices.Length > 0)
//         {
//             Debug.Log("-> Microphones: " + Microphone.devices.Length);
//             deviceName = Microphone.devices[0];

//             if(Microphone.devices.Length > 1)
//             { //There is more than one Mic available ...
//                 for(int i=0; i < Microphone.devices.Length; i++)
//                 {
//                     Debug.Log(Microphone.devices[i]);
//                     string device = Microphone.devices[i].ToUpper();
//                     if(device.Contains("ANDROID") || device.Contains("OCULUS"))
//                     {
//                         deviceName = Microphone.devices[i];
//                     }
//                 }
//             }

//             // On the MetaQuest the device name should be "Android audio input"
//             DebugPanel.SetStatus("Microphone Name: " + deviceName);
//         }
//         else
//         {
//             DebugPanel.SetStatus("No Microphone");
//             Debug.LogError("-> No Microphone found! :(");
//         }

//         leftActivateAction.action.performed += OnLeftActivateAction;        //Start Recording
//         leftActivateAction.action.canceled += OnLeftActivateActionCanceled; //End Recording
//         leftSelectAction.action.performed += OnLeftSelectAction;        // Playback
//         rightActivateAction.action.performed += OnRightActivateAction;  // Send Recording
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (isRecording)
//         {
//             if (Microphone.GetPosition(deviceName) >= audioClip.samples)
//             {
//                 StopRecording();
//             }
//         }
//     }

//     private void OnLeftActivateAction(InputAction.CallbackContext obj)
//     {
//         //Debug.Log("-> OnLeftActivateAction()");
//         StartRecording();
//     }

//     private void OnLeftActivateActionCanceled(InputAction.CallbackContext obj)
//     {
//         StopRecording();
//         TrimSilence();

//         if(audioClip.channels > 1)
//         { //We have a stereo recording...
//           //We want to feed a mono audioClip to the 'Whisper' model.
//             ConvertToMono();
//         }
//     }
        
//     private void OnLeftSelectAction(InputAction.CallbackContext obj)
//     {
//         PlayRecording();      
//     }

//     private void OnRightActivateAction(InputAction.CallbackContext obj)
//     {
//         TranscribeUsingWhisper();
//     }

//     private void StartRecording()
//     {
//         if (RunWhisper.IsReady && !isRecording)
//         { //Only start recording if Whisper is ready
//             Debug.Log("-> StartRecording()");
//             audioClip = Microphone.Start(deviceName, false, CLIP_LENGTH, CLIP_FREQUENCY);
//             isRecording = true;
//         }
//     }

//     private void StopRecording()
//     {
//         if (isRecording)
//         {
//             Debug.Log("-> StopRecording() - " + PrintAudioClipDetail(audioClip));
//             Microphone.End(deviceName);
//             audioClip.name = "Recording";
//             isRecording = false;
//         }
//     }

//     private void PlayRecording()
//     {
//         if (!isRecording)
//         {
//             if (audioClip != null)
//             {
//                 Debug.Log("-> PlayRecording() - " + PrintAudioClipDetail(audioClip));
//                 audioSource.clip = audioClip;
//                 audioSource.Play();
//             }
//             else
//             {
//                 Debug.Log("-> PlayRecording() -  clip is NULL! :(");
//             }
//         }
//     }

//     public void TrimSilence()
//     {
//         if (!isRecording)
//         {
//             if (audioClip is null)
//             {
//                 Debug.LogError("-> m_clip is NULL! :(");
//                 return;
//             }

//             int channels = audioClip.channels;
//             int frequency = audioClip.frequency;
//             int samples = audioClip.samples;

//             float[] audioData = new float[samples * channels];
//             audioClip.GetData(audioData, 0);

//             bool isSilent = false;
//             float silenceStart = 0;
//             var trimmedSamples = new List<float>();

//             for (int i = 0; i < audioData.Length; i += channels)
//             {
//                 float volume = Mathf.Abs(audioData[i]); // Simple volume estimation
//                 if (volume < silenceThreshold)
//                 {
//                     if (!isSilent)
//                     {
//                         isSilent = true;
//                         silenceStart = i / (float)(frequency * channels);
//                     }
//                 }
//                 else
//                 {
//                     if (isSilent)
//                     {
//                         float silenceDuration = i / (float)(frequency * channels) - silenceStart;
//                         if(silenceDuration < minSilenceLength)
//                         {
//                             // Add the "silence" back, as it's too short to be considered true silence
//                             for (int j = (int)(silenceStart * frequency * channels); j < i; j++)
//                             {
//                                 trimmedSamples.Add(audioData[j]);
//                             }
//                         }
//                         isSilent = false;
//                     }
//                     else
//                     {
//                         trimmedSamples.Add(audioData[i]);
//                     }
//                 }
//             }

//             if (trimmedSamples.Count > 0)
//             {// Create a new AudioClip
//                 AudioClip trimmedClip = AudioClip.Create(audioClip.name + "_Trimmed", trimmedSamples.Count, channels, frequency, false);
//                 trimmedClip.SetData(trimmedSamples.ToArray(), 0);
//                 audioClip = trimmedClip; // Replace the old clip with the trimmed clip
//                 Debug.Log("-> TrimSilence() - " + PrintAudioClipDetail(audioClip));
//             }
//         }
//     }

//     public void ConvertToMono()
//     {
//         int channels = audioClip.channels; // Typically 2 for stereo
//         int samples = audioClip.samples;   // Number of samples per channel

//         float[] stereoData = new float[samples * channels];
//         audioClip.GetData(stereoData, 0);

//         float[] monoData = new float[samples];

//         for (int i = 0; i < samples; i++)
//         {
//             float sum = 0f;

//             // Sum all the channel values for this sample
//             for (int j = 0; j < channels; j++)
//             {
//                 sum += stereoData[i * channels + j];
//             }

//             // Average the sum to get the mono sample value
//             monoData[i] = sum / channels;
//         }

//         // Create a new AudioClip in mono and set the data
//         AudioClip monoClip = AudioClip.Create(audioClip.name + "_Mono", samples, 1, audioClip.frequency, false);
//         monoClip.SetData(monoData, 0);
//         audioClip = monoClip; // Replace the old clip with the trimmed clip
//         Debug.Log("-> ConvertToMono() - " + PrintAudioClipDetail(audioClip));
//     }

//     private string PrintAudioClipDetail(AudioClip clip)
//     {
//         string details = "clip secs: " + audioClip.length + ", samp: " + audioClip.samples + ", chan: " + audioClip.channels + ", freq: " + audioClip.frequency;
//         return details;
//     }

//     private void TranscribeUsingWhisper()
//     {
//         if (RunWhisper.IsReady && !isRecording)
//         {
//             RunWhisper.Transcribe(audioClip);
//         }
//     }

//     void OnDestroy()
//     {
//         leftActivateAction.action.performed -= OnLeftActivateAction;
//         leftActivateAction.action.canceled -= OnLeftActivateActionCanceled;
//         leftSelectAction.action.performed -= OnLeftSelectAction;
//         rightActivateAction.action.performed -= OnRightActivateAction;
//     }
// }    