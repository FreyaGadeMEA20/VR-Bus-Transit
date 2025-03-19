using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class AudioSlider_Ambience : MonoBehaviour
{
    public Slider ambienceSlider;
    public TextMeshProUGUI ambienceValueText; // Reference to the TextMeshPro component

    void Start()
    {
        if (ambienceSlider == null)
        {
            ambienceSlider = GetComponent<Slider>();
        }

        // Add listener to handle value change
        ambienceSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        Debug.Log("[AudioSlider_Ambience] Listener added to slider.");

        // Initialize the text display
        UpdateText(ambienceSlider.value);
    }

    void OnSliderValueChange()
    {
        // Update the Ambience_Manager
        if (Ambience_Manager.Instance != null)
        {
            Ambience_Manager.Instance.SetAmbienceVolume(ambienceSlider.value);
            Debug.Log($"[AudioSlider_Ambience] Sent value ({ambienceSlider.value}) to Ambience_Manager.");
        }

        // Update the text display
        UpdateText(ambienceSlider.value);
    }

    void UpdateText(float value)
    {
        if (ambienceValueText != null)
        {
            ambienceValueText.text = value.ToString("0.00"); // Display value with 2 decimal places
        }
    }
}