using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_High : MonoBehaviour
{
    public Slider npcSlider;
    public Slider ambienceSlider;
    public Slider chatterSlider;

    public void SetHighLevel()
    {
        if (npcSlider != null)
        {
            npcSlider.value = 1.0f; // Set NPC slider to 100
            Debug.Log("[Level_High] NPC slider set to 100.");
        }

        if (ambienceSlider != null)
        {
            ambienceSlider.value = 1.0f; // Set Ambience slider to 1.0
            Debug.Log("[Level_High] Ambience slider set to 1.0.");
        }

        if (chatterSlider != null)
        {
            chatterSlider.value = 1.0f; // Set Chatter slider to 1.0
            Debug.Log("[Level_High] Chatter slider set to 1.0.");
        }
    }
}


