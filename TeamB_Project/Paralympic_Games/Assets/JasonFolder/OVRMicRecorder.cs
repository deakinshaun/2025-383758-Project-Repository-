using System.Collections.Generic;
using LudicWorlds;
using UnityEngine;

public class MicRecorder : MonoBehaviour
{
    private const int CLIP_LENGTH = 20;
    private const int CLIP_FREQUENCY = 16000;

    [Header("Used to trim audio:")]
    [SerializeField, Range(0.0f, 0.1f)] private float silenceThreshold = 0.02f;
    [SerializeField, Range(0.0f, 1.0f)] private float minSilenceLength = 0.5f;
    [Space]
    [SerializeField] private RunWhisper RunWhisper;

    private AudioClip audioClip;
    private AudioSource audioSource;
    private string deviceName;
    private bool isRecording;

    void Start()
    {
        Debug.Log("* Hold down the Left Trigger to Record.");
        Debug.Log("* Release the Left Trigger to stop Recording.");
        Debug.Log("* Press the Left Grip to Playback.");
        Debug.Log("* Press the Right Trigger to Transcribe.");

        audioSource = Camera.main.GetComponent<AudioSource>();

        if (audioSource is null)
        {
            Debug.LogError("-> Camera AudioSource is NULL! :(");
        }

        if (Microphone.devices.Length > 0)
        {
            Debug.Log("-> Microphones: " + Microphone.devices.Length);
            deviceName = Microphone.devices[0];

            if (Microphone.devices.Length > 1)
            {
                for (int i = 0; i < Microphone.devices.Length; i++)
                {
                    Debug.Log(Microphone.devices[i]);
                    string device = Microphone.devices[i].ToUpper();
                    if (device.Contains("ANDROID") || device.Contains("OCULUS"))
                    {
                        deviceName = Microphone.devices[i];
                    }
                }
            }

            DebugPanel.SetStatus("Microphone Name: " + deviceName);
        }
        else
        {
            DebugPanel.SetStatus("No Microphone");
            Debug.LogError("-> No Microphone found! :(");
        }
    }

    void Update()
    {
        // Start recording when LEFT trigger is pressed
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            StartRecording();
        }

        // Stop recording when LEFT trigger is released
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            StopRecording();
            TrimSilence();

            if (audioClip.channels > 1)
            {
                ConvertToMono();
            }
        }

        // Play recording when LEFT grip is pressed
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            PlayRecording();
        }

        // Transcribe when RIGHT trigger is pressed
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            TranscribeUsingWhisper();
        }

        if (isRecording && Microphone.GetPosition(deviceName) >= audioClip.samples)
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        if (RunWhisper.IsReady && !isRecording)
        {
            Debug.Log("-> StartRecording()");
            audioClip = Microphone.Start(deviceName, false, CLIP_LENGTH, CLIP_FREQUENCY);
            isRecording = true;
        }
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            Debug.Log("-> StopRecording() - " + PrintAudioClipDetail(audioClip));
            Microphone.End(deviceName);
            audioClip.name = "Recording";
            isRecording = false;
        }
    }

    private void PlayRecording()
    {
        if (!isRecording)
        {
            if (audioClip != null)
            {
                Debug.Log("-> PlayRecording() - " + PrintAudioClipDetail(audioClip));
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.Log("-> PlayRecording() - clip is NULL! :(");
            }
        }
    }

    public void TrimSilence()
    {
        if (!isRecording)
        {
            if (audioClip == null)
            {
                Debug.LogError("-> audioClip is NULL! :(");
                return;
            }

            int channels = audioClip.channels;
            int frequency = audioClip.frequency;
            int samples = audioClip.samples;

            float[] audioData = new float[samples * channels];
            audioClip.GetData(audioData, 0);

            bool isSilent = false;
            float silenceStart = 0;
            var trimmedSamples = new List<float>();

            for (int i = 0; i < audioData.Length; i += channels)
            {
                float volume = Mathf.Abs(audioData[i]);
                if (volume < silenceThreshold)
                {
                    if (!isSilent)
                    {
                        isSilent = true;
                        silenceStart = i / (float)(frequency * channels);
                    }
                }
                else
                {
                    if (isSilent)
                    {
                        float silenceDuration = i / (float)(frequency * channels) - silenceStart;
                        if (silenceDuration < minSilenceLength)
                        {
                            for (int j = (int)(silenceStart * frequency * channels); j < i; j++)
                            {
                                trimmedSamples.Add(audioData[j]);
                            }
                        }
                        isSilent = false;
                    }

                    trimmedSamples.Add(audioData[i]);
                }
            }

            if (trimmedSamples.Count > 0)
            {
                AudioClip trimmedClip = AudioClip.Create(audioClip.name + "_Trimmed", trimmedSamples.Count, channels, frequency, false);
                trimmedClip.SetData(trimmedSamples.ToArray(), 0);
                audioClip = trimmedClip;
                Debug.Log("-> TrimSilence() - " + PrintAudioClipDetail(audioClip));
            }
        }
    }

    public void ConvertToMono()
    {
        int channels = audioClip.channels;
        int samples = audioClip.samples;

        float[] stereoData = new float[samples * channels];
        audioClip.GetData(stereoData, 0);

        float[] monoData = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float sum = 0f;
            for (int j = 0; j < channels; j++)
            {
                sum += stereoData[i * channels + j];
            }
            monoData[i] = sum / channels;
        }

        AudioClip monoClip = AudioClip.Create(audioClip.name + "_Mono", samples, 1, audioClip.frequency, false);
        monoClip.SetData(monoData, 0);
        audioClip = monoClip;
        Debug.Log("-> ConvertToMono() - " + PrintAudioClipDetail(audioClip));
    }

    private string PrintAudioClipDetail(AudioClip clip)
    {
        return $"clip secs: {clip.length}, samp: {clip.samples}, chan: {clip.channels}, freq: {clip.frequency}";
    }

    private void TranscribeUsingWhisper()
    {
        if (RunWhisper.IsReady && !isRecording)
        {
            RunWhisper.Transcribe(audioClip);
        }
    }
}
