using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public Slider volumeSlider;
    public Toggle audioToggle;

    private Dictionary<string, AudioClip> audioClips;
    private float volume = 1f;
    private bool isAudioEnabled = true;

    void Start()
    {
        // Load all audio files from the Resources/Audio folder
        audioClips = new Dictionary<string, AudioClip>();
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
        foreach (var clip in clips)
        {
            audioClips[clip.name] = clip;
            Debug.Log("Loaded audio clip: " + clip.name);
        }

        // Set up volume and toggle
        volume = PlayerPrefs.GetFloat("Volume", 1f);
        isAudioEnabled = PlayerPrefs.GetInt("AudioEnabled", 1) == 1;

        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }
       
        audioToggle.isOn = isAudioEnabled;

        audioSource.volume = volume;
        audioSource.mute = !isAudioEnabled;

        // Add listeners
      
        audioToggle.onValueChanged.AddListener(ToggleAudio);
    }

    public void PlayAudio(string clipName)
    {
        if (!isAudioEnabled) return;

        if (audioClips.ContainsKey(clipName))
        {
            audioSource.clip = audioClips[clipName];
            audioSource.Play();
            Debug.Log("Playing audio: " + clipName);
        }
        else
        {
            Debug.LogWarning("Audio clip not found: " + clipName);
        }
    }

    private void UpdateVolume(float value)
    {
        volume = value;
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    private void ToggleAudio(bool isEnabled)
    {
        isAudioEnabled = isEnabled;
        audioSource.mute = !isEnabled;
        PlayerPrefs.SetInt("AudioEnabled", isEnabled ? 1 : 0);
    }
}
