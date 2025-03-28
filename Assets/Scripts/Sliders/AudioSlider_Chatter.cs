using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSlider_Chatter : MonoBehaviour
{
    public Slider chatterSlider;
    public TextMeshProUGUI chatterValueText;

    void Start()
    {
        if (chatterSlider == null)
        {
            chatterSlider = GetComponent<Slider>();
        }

        // Add listener to handle value change
        chatterSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        Debug.Log("[AudioSlider_Chatter] Listener added to slider.");

        // Initialize the text display
        UpdateText(chatterSlider.value);
    }

    void OnSliderValueChange()
    {
        // Update the Chatter_Manager
        if (Chatter_Manager.Instance != null)
        {
            Chatter_Manager.Instance.SetChatterVolume(chatterSlider.value);
            Debug.Log($"[AudioSlider_Chatter] Sent value ({chatterSlider.value}) to Chatter_Manager.");
        }

        // Update the text display
        UpdateText(chatterSlider.value);
    }

    void UpdateText(float value)
    {
        if (chatterValueText != null)
        {
            chatterValueText.text = value.ToString("0.00"); // Display value with 2 decimal places
        }
    }
}