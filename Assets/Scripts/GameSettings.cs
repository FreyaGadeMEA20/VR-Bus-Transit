using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Movement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }
    public int LevelSelected {
        get{return levelSelected;}
        set{levelSelected = value;}
    } // Value to be retrieved by LevelSelect
    [SerializeField] int levelSelected = 0; // Default level selected

    // NPC settings
    public int NPCAmount {
        get{return npcAmount;}
        set{npcAmount = value;}
    } // Value to be retrieved by npcAmount
    [SerializeField] int npcAmount = 0; // Default NPC amount

    public float SchoolNPCValue {
        get{return schoolNPCAmount;}
        set{schoolNPCAmount = value;}
    } // Value to be retrieved by schoolNPCAmount
    [SerializeField] float schoolNPCAmount = 0; // Default school NPC amount

    public float BusNPCValue {
        get{return busNPCAmount;}
        set{busNPCAmount = value;}
    } // Value to be retrieved by busNPCAmount
    [SerializeField] float busNPCAmount = 0; // Default bus NPC amount
    private List<GameObject> schoolNPCPairs = new List<GameObject>();
    private List<GameObject> busNPCs = new List<GameObject>();

    // Ambience settings
    private float ambienceVolume = 0.4f; // Default volume

    // Chatter settings
    private float chatterVolume = 0.2f; // Default volume

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
    }

    void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void SetNPCAmount(int amount)
    {
        NPCAmount = amount;
        SchoolNPCValue = amount;
        BusNPCValue = amount;
        Debug.Log($"[NPCManager] NPCAmount set to {NPCAmount}");
    }

    private void UpdateSchoolNPCPairs()
    {
        if (schoolNPCPairs.Count == 0)
        {
            Debug.LogWarning("[NPCManager] No school NPC pairs found to update.");
            return;
        }

        // Calculate the number of pairs to enable based on the slider value
        int pairsToEnable = Mathf.Clamp(Mathf.FloorToInt(SchoolNPCValue / 9f), 0, schoolNPCPairs.Count);
        Debug.Log($"[NPCManager] Enabling {pairsToEnable} school NPC pairs.");

        // Enable or disable NPC pairs
        for (int i = 0; i < schoolNPCPairs.Count; i++)
        {
            schoolNPCPairs[i].SetActive(i < pairsToEnable);
        }
    }

    private void UpdateBusNPCs()
    {
        if (busNPCs.Count == 0)
        {
            Debug.LogWarning("[NPCManager] No bus NPCs found to update.");
            return;
        }

        int npcsToEnable = Mathf.Clamp(Mathf.FloorToInt(BusNPCValue / 2.5f), 0, busNPCs.Count);
        Debug.Log($"[NPCManager] Enabling {npcsToEnable} random bus NPCs.");

        foreach (GameObject npc in busNPCs)
        {
            npc.SetActive(false);
        }

        List<GameObject> shuffledBusNPCs = new List<GameObject>(busNPCs);
        for (int i = 0; i < shuffledBusNPCs.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledBusNPCs.Count);
            GameObject temp = shuffledBusNPCs[i];
            shuffledBusNPCs[i] = shuffledBusNPCs[randomIndex];
            shuffledBusNPCs[randomIndex] = temp;
        }

        for (int i = 0; i < npcsToEnable; i++)
        {
            shuffledBusNPCs[i].SetActive(true);
        }
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceVolume = volume;
        Debug.Log($"[Ambience_Manager] Ambience volume set to {ambienceVolume}");

        // Update all audio sources tagged as "Ambience"
        UpdateAmbienceAudioSources();
    }

    private void UpdateAmbienceAudioSources()
    {
        GameObject[] ambienceObjects = GameObject.FindGameObjectsWithTag("Ambience");
        foreach (GameObject obj in ambienceObjects)
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = ambienceVolume;
                Debug.Log($"[Ambience_Manager] Updated volume for {obj.name} to {ambienceVolume}");
            }
        }
    }


    public void SetChatterVolume(float volume)
    {
        chatterVolume = volume;
        Debug.Log($"[Chatter_Manager] Chatter volume set to {chatterVolume}");

        // Update all audio sources tagged as "Bus Chatter"
        UpdateChatterAudioSources();
    }

    private void UpdateChatterAudioSources()
    {
        GameObject[] chatterObjects = GameObject.FindGameObjectsWithTag("Bus Chatter");
        foreach (GameObject obj in chatterObjects)
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = chatterVolume;
                Debug.Log($"[Chatter_Manager] Updated volume for {obj.name} to {chatterVolume}");
            }
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[NPCManager] Scene loaded: {scene.name}. Reinitializing NPCs.");

        // Clear the current lists
        schoolNPCPairs.Clear();
        busNPCs.Clear();

        // Find all NPC pairs tagged as "SchoolNPCPair"
        GameObject[] schoolPairs = GameObject.FindGameObjectsWithTag("SchoolNPCPair");
        schoolNPCPairs.AddRange(schoolPairs);
        Debug.Log($"[NPCManager] Found {schoolNPCPairs.Count} school NPC pairs.");

        // Find all NPCs tagged as "Bus NPC"
        GameObject[] busNPCObjects = GameObject.FindGameObjectsWithTag("Bus NPC");
        busNPCs.AddRange(busNPCObjects);
        Debug.Log($"[NPCManager] Found {busNPCs.Count} bus NPCs.");

        // Apply the current values to update the NPCs
        UpdateSchoolNPCPairs();
        UpdateBusNPCs();

        Debug.Log($"[Ambience_Manager] Scene loaded: {scene.name}. Reapplying ambience volume.");
        UpdateAmbienceAudioSources();

        Debug.Log($"[Chatter_Manager] Scene loaded: {scene.name}. Reapplying chatter volume.");
        UpdateChatterAudioSources();
    }
}