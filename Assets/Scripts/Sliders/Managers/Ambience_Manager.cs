using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager_Ambience : MonoBehaviour
{
    public string audioSourceTag = "Ambience"; // Replace with your actual tag
    private Slider volumeSlider_Ambience;

    void Start()
    {
        // Find the slider GameObject in the scene
        volumeSlider_Ambience = FindObjectOfType<AudioSlider_Ambience>().volumeSlider_Ambience;

        // Set the audio source volumes based on the slider value
        UpdateAudioSourcesVolume(volumeSlider_Ambience.value);

        // Add listener to update volume when slider value changes
        volumeSlider_Ambience.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    void OnVolumeChange()
    {
        // Update the audio source volumes
        UpdateAudioSourcesVolume(volumeSlider_Ambience.value);
    }

    void UpdateAudioSourcesVolume(float volume)
    {
        // Find all audio sources with the specified tag
        GameObject[] audioSources = GameObject.FindGameObjectsWithTag(audioSourceTag);
        foreach (GameObject audioSourceObject in audioSources)
        {
            AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
    }
}
