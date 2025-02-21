using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMurmur : MonoBehaviour
{
    public AudioClip[] conversationClips; // Array to hold conversation audio clips
    public AudioClip[] tiktokClips; // Array to hold tiktok audio clips
    public int minAudioSources = 1; // Minimum number of audio sources
    public int maxAudioSources = 10; // Maximum number of audio sources
    public Transform npcParent; // Parent transform containing NPCs
    private List<AudioSource> audioSources = new List<AudioSource>();

    // Start is called before the first frame update
    void Start()
    {
        int npcCount = GetNPCCount(); // Replace with your method to get the number of NPCs
        int audioSourceCount = Random.Range(Mathf.Max(minAudioSources, npcCount / 2), Mathf.Min(maxAudioSources, npcCount) + 1);
        InitializeAudioSources(audioSourceCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int GetNPCCount()
    {
        if (npcParent != null)
        {
            return npcParent.childCount;
        }
        return 0;
    }

    private void InitializeAudioSources(int count)
    {
        HashSet<int> usedIndices = new HashSet<int>();

        for (int i = 0; i < count; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = GetRandomClip();
            audioSource.loop = true;
            audioSource.Play();
            audioSources.Add(audioSource);

            // Set the position of the audio source to a random NPC position
            if (npcParent != null && npcParent.childCount > 0)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, npcParent.childCount);
                } while (usedIndices.Contains(randomIndex));

                usedIndices.Add(randomIndex);
                Transform randomNPC = npcParent.GetChild(randomIndex);
                audioSource.transform.position = randomNPC.position;
            }
        }
    }

    private AudioClip GetRandomClip()
    {
        // Randomly choose between conversation and tiktok clips
        if (Random.value > 0.5f && conversationClips.Length > 0)
        {
            return conversationClips[Random.Range(0, conversationClips.Length)];
        }
        else if (tiktokClips.Length > 0)
        {
            return tiktokClips[Random.Range(0, tiktokClips.Length)];
        }
        return null;
    }
}
