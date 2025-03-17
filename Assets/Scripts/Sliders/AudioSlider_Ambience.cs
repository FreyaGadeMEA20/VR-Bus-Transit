using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider_Ambience : MonoBehaviour
{
    public Slider volumeSlider_Ambience;

    void Awake()
    {
        // Ensure this GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);

        if (volumeSlider_Ambience == null)
        {
            volumeSlider_Ambience = GetComponent<Slider>();
        }

        // Add listener to handle value change
        volumeSlider_Ambience.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    void OnVolumeChange()
    {
        // This method can be used to handle any additional logic when the volume changes
    }
}