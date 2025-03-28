using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Chatter_Manager : MonoBehaviour
{
    public static Chatter_Manager Instance { get; private set; }

    private float chatterVolume = 0.2f; // Default volume

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            Debug.Log("[Chatter_Manager] Instance initialized.");

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.LogWarning("[Chatter_Manager] Duplicate instance detected. Destroying duplicate.");
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
        Debug.Log($"[Chatter_Manager] Scene loaded: {scene.name}. Reapplying chatter volume.");
        UpdateChatterAudioSources();
    }
}



