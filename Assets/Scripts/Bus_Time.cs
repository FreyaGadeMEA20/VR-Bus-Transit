using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus_Time : MonoBehaviour
{
    public int hour{
        get{return hour;}
        set{
            hour = (int)(Mathf.Round((minute / 60)) % 24);
        }
    }
    public int minute;
    public static Bus_Time Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.time);
    }
}
