using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_Low : MonoBehaviour
{
    public Slider npcSlider; // Reference to the NPC slider
    public Slider ambienceSlider; // Reference to the Ambience slider
    public Slider chatterSlider; // Reference to the Chatter slider

    // Start is called before the first frame update
    void Start()
    {
        // Optional: Add a listener to the button if needed
        Debug.Log("[Level_Low] Button initialized.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLowLevel()
    {
        if (npcSlider != null)
        {
            npcSlider.value = 0.33f; // Set NPC slider to 33
            Debug.Log("[Level_Low] NPC slider set to 33.");
        }

        if (ambienceSlider != null)
        {
            ambienceSlider.value = 0.33f; // Set Ambience slider to 0.33
            Debug.Log("[Level_Low] Ambience slider set to 0.33.");
        }

        if (chatterSlider != null)
        {
            chatterSlider.value = 0.33f; // Set Chatter slider to 0.33
            Debug.Log("[Level_Low] Chatter slider set to 0.33.");
        }
    }
}
