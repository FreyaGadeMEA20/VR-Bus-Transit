using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Movement;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    public int NPCAmount { get; private set; } // Value to be retrieved by PedestrianSpawner

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            Debug.Log("NPCManager instance initialized.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate NPCManager instance destroyed.");
        }
    }




    public void SetNPCAmount(int amount)
    {
        NPCAmount = amount;
        Debug.Log($"NPCManager: NPCAmount set to {NPCAmount}");
    }
}
