using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_NPCs : MonoBehaviour
{
    public Slider Slider_NPCAmount;
    public static Slider_NPCs Instance { get; private set; } // Singleton instance
    
    int npcAmount;
    public int NPCAmount {
        get{
            return npcAmount;
        }, set{
            npcAmount = value;
        }
    } // Converted slider value (0 to 100)
    public event System.Action<int> OnNPCAmountChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Slider_NPCs instance initialized.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate Slider_NPCs instance destroyed.");
            return;
        }

        if (Slider_NPCAmount != null)
        {
            Slider_NPCAmount.onValueChanged.AddListener(delegate { OnAmountChange(); });
            Debug.Log("Slider onValueChanged listener added.");
        }
        else
        {
            Debug.LogError("Slider_NPCAmount is not assigned!");
        }
    }

    void Start()
    {
        // Ensure the value is updated at the start
        OnAmountChange();
    }

    void OnAmountChange()
    {
        NPCAmount = Mathf.RoundToInt(Slider_NPCAmount.value * 100);
        PlayerPrefs.SetInt("NPCAmount", NPCAmount);
        PlayerPrefs.Save();
        Debug.Log($"Slider value: {Slider_NPCAmount.value}, NPCAmount set to: {NPCAmount}");
        
        // Notify listeners
        OnNPCAmountChanged?.Invoke(NPCAmount);
    }

    public void SubscribeToNPCAmountChanged(System.Action<int> listener)
    {
        OnNPCAmountChanged += listener;

        // Immediately invoke the listener with the current value
        listener?.Invoke(NPCAmount);
    }
}
