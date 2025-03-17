using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider_Chatter : MonoBehaviour
{
    public Slider volumeSlider_Chatter;

    void Awake()
    {
        // Ensure this GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);

        if (volumeSlider_Chatter == null)
        {
            volumeSlider_Chatter = GetComponent<Slider>();
        }

        // Add listener to handle value change
        volumeSlider_Chatter.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    void OnVolumeChange()
    {
        // This method can be used to handle any additional logic when the volume changes
    }
}