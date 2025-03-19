using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Movement;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    public int NPCAmount { get; private set; } // Value to be retrieved by PedestrianSpawner
    public float SchoolNPCValue { get; private set; } // Slider value for school NPC pairs

    private List<GameObject> schoolNPCPairs = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            Debug.Log($"[NPCManager] Instance initialized. Instance ID: {GetInstanceID()}");

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.LogWarning($"[NPCManager] Duplicate instance detected. Destroying duplicate. Instance ID: {GetInstanceID()}");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log($"[NPCManager] Start called. Current NPCAmount: {NPCAmount}");
    }

    void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        Debug.Log($"[NPCManager] Instance destroyed. Instance ID: {GetInstanceID()}");
    }

    public void SetNPCAmount(int amount)
    {
        NPCAmount = amount;
        Debug.Log($"[NPCManager] NPCAmount set to {NPCAmount}");
    }

    public void SetSchoolNPCValue(float value)
    {
        SchoolNPCValue = value;
        Debug.Log($"[NPCManager] SchoolNPCValue set to {SchoolNPCValue}");

        // Update the NPC pairs near the school
        UpdateSchoolNPCPairs();
    }

    private void UpdateSchoolNPCPairs()
    {
        if (schoolNPCPairs.Count == 0)
        {
            Debug.LogWarning("[NPCManager] No school NPC pairs found to update.");
            return;
        }

        // Calculate the number of pairs to enable based on the slider value
        int pairsToEnable = Mathf.Clamp(Mathf.FloorToInt(SchoolNPCValue / 0.09f), 0, schoolNPCPairs.Count);
        Debug.Log($"[NPCManager] Enabling {pairsToEnable} school NPC pairs.");

        // Enable or disable NPC pairs
        for (int i = 0; i < schoolNPCPairs.Count; i++)
        {
            schoolNPCPairs[i].SetActive(i < pairsToEnable);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[NPCManager] Scene loaded: {scene.name}. Reinitializing school NPC pairs.");

        // Clear the current list of NPC pairs
        schoolNPCPairs.Clear();

        // Find all NPC pairs tagged as "SchoolNPCPair"
        GameObject[] pairs = GameObject.FindGameObjectsWithTag("SchoolNPCPair");
        schoolNPCPairs.AddRange(pairs);

        Debug.Log($"[NPCManager] Found {schoolNPCPairs.Count} school NPC pairs.");

        // Apply the current SchoolNPCValue to update the pairs
        UpdateSchoolNPCPairs();
    }
}
