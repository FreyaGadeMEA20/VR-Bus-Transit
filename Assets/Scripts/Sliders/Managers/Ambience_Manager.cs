using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ambience_Manager : MonoBehaviour
{
    public static Ambience_Manager Instance { get; private set; }

    private float ambienceVolume = 0.4f; // Default volume

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            Debug.Log("[Ambience_Manager] Instance initialized.");

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.LogWarning("[Ambience_Manager] Duplicate instance detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[Ambience_Manager] Scene loaded: {scene.name}. Reapplying ambience volume.");
        UpdateAmbienceAudioSources();
    }
}
