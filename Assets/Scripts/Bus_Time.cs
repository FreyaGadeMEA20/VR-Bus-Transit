using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus_Time : MonoBehaviour
{
    public List<BusController> Bus;
    public float currentTime;
    public static Bus_Time Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*void Update()
    {
        currentTime += Time.deltaTime;
    }*/
}
