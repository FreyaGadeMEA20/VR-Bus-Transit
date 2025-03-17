using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Movement;

public class NPCManager : MonoBehaviour
{
    public string ObjectTag = "Pedestrians"; // Replace with your actual tag
    private Slider Slider_NPCAmount;
    public Movement.PedestrianSpawner NPCSpawner; 

    void Start()
    {
        //Find the Pedestrian spawner in the scene
        NPCSpawner = GetComponent<PedestrianSpawner>();

        // Find the slider GameObject in the scene
        Slider_NPCAmount = FindObjectOfType<Slider_NPCs>().Slider_NPCAmount;

        // Set the audio source volumes based on the slider value
        UpdateNPCAmount(Slider_NPCAmount.value);

        // Add listener to update volume when slider value changes
        Slider_NPCAmount.onValueChanged.AddListener(delegate { OnAmountChange(); });
    }

    void OnAmountChange()
    {
        // Update the audio source volumes
        UpdateNPCAmount(Slider_NPCAmount.value);
    }

    void UpdateNPCAmount(float amount)
    {
        if (NPCSpawner != null)
        {
            int convertedAmount = (int)Slider_NPCAmount.value;
            NPCSpawner.pedestriansToSpawn = convertedAmount;
        }
        
    }
}
