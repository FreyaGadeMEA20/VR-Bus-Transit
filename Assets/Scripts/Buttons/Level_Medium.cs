using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_Medium : MonoBehaviour
{
    public Slider npcSlider;
    public Slider ambienceSlider;
    public Slider chatterSlider;

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

    public void SetMediumLevel()
    {
        if (npcSlider != null)
        {
            npcSlider.value = 0.66f; // Set NPC slider to 66
            Debug.Log("[Level_Medium] NPC slider set to 66.");
        }

        if (ambienceSlider != null)
        {
            ambienceSlider.value = 0.66f; // Set Ambience slider to 0.66
            Debug.Log("[Level_Medium] Ambience slider set to 0.66.");
        }

        if (chatterSlider != null)
        {
            chatterSlider.value = 0.66f; // Set Chatter slider to 0.66
            Debug.Log("[Level_Medium] Chatter slider set to 0.66.");
        }
    }
}
