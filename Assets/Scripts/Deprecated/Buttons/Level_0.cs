using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_Zero : MonoBehaviour
{
    public Slider npcSlider;
    public Slider ambienceSlider;
    public Slider chatterSlider;

    // Start is called before the first frame update
    void Start()
    {
        // Optional: Add a listener to the button if needed
        Debug.Log("[Level_Zero] Button initialized.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetZeroLevel()
    {
        if (npcSlider != null)
        {
            npcSlider.value = 0f; // Set NPC slider to 66
            Debug.Log("[Level_Medium] NPC slider set to 0.");
        }

        if (ambienceSlider != null)
        {
            ambienceSlider.value = 0f; // Set Ambience slider to 0.66
            Debug.Log("[Level_Medium] Ambience slider set to 0.");
        }

        if (chatterSlider != null)
        {
            chatterSlider.value = 0f; // Set Chatter slider to 0.66
            Debug.Log("[Level_Medium] Chatter slider set to 0.");
        }
    }
}
