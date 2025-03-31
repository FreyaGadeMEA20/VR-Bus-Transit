using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] int _NPCAmount;
    [SerializeField] float _Ambience, _Chatter;
    [SerializeField] int difficulty;
    public void SelectDifficulty()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.LevelSelected = difficulty; // Set LevelSelected 
            GameSettings.Instance.SetNPCAmount(_NPCAmount); // Set NPC slider
            //Debug.Log($"[LevelSelect] NPC slider set to {_NPCAmount}.");
        
            GameSettings.Instance.SetAmbienceVolume(_Ambience); // Set Ambience slider 
            //Debug.Log($"[LevelSelect] Ambience slider set to {_Ambience}.");
        
            GameSettings.Instance.SetChatterVolume(_Chatter); // Set Chatter slider
            //Debug.Log($"[LevelSelect] Chatter slider set to {_Chatter}.");
        }
    }
}
