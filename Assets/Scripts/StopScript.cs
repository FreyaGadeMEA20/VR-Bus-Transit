using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopScript : MonoBehaviour
{
    public BusScreenController busScreenController;
    [SerializeField] BusGameManager busGameManager;

    void Start(){
        busGameManager = GameObject.Find("GameManager").GetComponent<BusGameManager>();
    }

    public void StopButton(){
        busGameManager.StopBus();
        busScreenController.ApplyStopTexture();
    }
}
