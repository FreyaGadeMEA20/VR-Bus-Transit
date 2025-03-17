using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_NPCs : MonoBehaviour
{
    public Slider Slider_NPCAmount;

    void Awake()
    {
        // Ensure this GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);

        if (Slider_NPCAmount == null)
        {
            Slider_NPCAmount = GetComponent<Slider>();
        }

        // Add listener to handle value change
        Slider_NPCAmount.onValueChanged.AddListener(delegate { OnAmountChange(); });
    }

        void OnAmountChange()
    {
        // This method can be used to handle any additional logic when the volume changes
    }
}
