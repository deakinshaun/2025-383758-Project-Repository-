using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrgilFolder.Scripts.Utility
{
    public static class AudioSourceHelper 
    {

        public static void PlayOneShot(this AudioSource audioSource, AudioClip clip, Vector2 pitchRange,
            float volumeScale = 1.0f)
        {
            audioSource.clip = clip;
            audioSource.volume *= volumeScale;
            audioSource.pitch = Random.Range(pitchRange.y, pitchRange.y);
            audioSource.Play();
        }
        
    }
}