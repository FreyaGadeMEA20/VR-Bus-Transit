using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_BusNPCs : MonoBehaviour
{
    public Slider busNPCSlider;

    void Start()
    {
        if (busNPCSlider == null)
        {
            busNPCSlider = GetComponent<Slider>();
        }

        // Add listener to handle value change
        busNPCSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        Debug.Log("[Slider_BusNPCs] Listener added to slider.");
    }

    void OnSliderValueChange()
    {
        float value = busNPCSlider.value;

        // Send the value to the NPCManager
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.SetBusNPCValue(value);
            Debug.Log($"[Slider_BusNPCs] Sent value ({value}) to NPCManager.");
        }
        else
        {
            Debug.LogError("[Slider_BusNPCs] NPCManager instance not found!");
        }
    }
}
