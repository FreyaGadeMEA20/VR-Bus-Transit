using UnityEngine;
using UnityEngine.UI;

public class AudioSlider_SchoolNPCs : MonoBehaviour
{
    public Slider schoolNPCSlider;

    void Start()
    {
        if (schoolNPCSlider == null)
        {
            schoolNPCSlider = GetComponent<Slider>();
        }

        // Add listener to handle value change
        schoolNPCSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        Debug.Log("[AudioSlider_SchoolNPCs] Listener added to slider.");
    }

    void OnSliderValueChange()
    {
        float value = schoolNPCSlider.value;

        // Send the value to the NPCManager
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetSchoolNPCValue(value);
            Debug.Log($"[AudioSlider_SchoolNPCs] Sent value ({value}) to NPCManager.");
        }
        else
        {
            Debug.LogError("[AudioSlider_SchoolNPCs] NPCManager instance not found!");
        }
    }
}