using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager_Chatter : MonoBehaviour
{
    public string audioSourceTag = "Bus Chatter"; // Replace with your actual tag
    private Slider volumeSlider_Chatter;

    void Start()
    {
        // Find the slider GameObject in the scene
        volumeSlider_Chatter = FindObjectOfType<AudioSlider_Chatter>().volumeSlider_Chatter;

        // Set the audio source volumes based on the slider value
        UpdateAudioSourcesVolume(volumeSlider_Chatter.value);

        // Add listener to update volume when slider value changes
        volumeSlider_Chatter.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    void OnVolumeChange()
    {
        // Update the audio source volumes
        UpdateAudioSourcesVolume(volumeSlider_Chatter.value);
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

