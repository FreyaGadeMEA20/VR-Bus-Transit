using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhoneTime : MonoBehaviour
{
    [SerializeField] int hour;
    [SerializeField] int minute;
    [SerializeField] TextMeshProUGUI text;

    public static PhoneTime Instance { get; internal set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void UpdateTime()
    {
        minute = (minute + 1) % 60;
        if(minute == 0){
            hour = (hour + 1) % 24;
        }
        //hour = System.DateTime.Now.Hour;
        //minute = System.DateTime.Now.Minute;
        text.text = $"{hour:00}:{minute:00}";
    }
}
