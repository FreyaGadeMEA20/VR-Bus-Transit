using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slider_NPCs : MonoBehaviour
{
    public Slider npcSlider;
    public TextMeshProUGUI npcValueText;

    void Start()
    {
        if (npcSlider == null)
        {
            npcSlider = GetComponent<Slider>();
        }

        npcSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        UpdateText(npcSlider.value);
    }

    void OnSliderValueChange()
    {
        if (NPCManager.Instance != null)
        {
            int value = Mathf.RoundToInt(npcSlider.value * 100);
            NPCManager.Instance.SetNPCAmount(value);
        }

        UpdateText(npcSlider.value);
    }

    void UpdateText(float value)
    {
        if (npcValueText != null)
        {
            npcValueText.text = Mathf.RoundToInt(value * 100).ToString();
        }
    }
}