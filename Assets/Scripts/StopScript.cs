using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopScript : MonoBehaviour
{
    public BusScreenController busScreenController;
    [SerializeField] GameManager busGameManager;

    void Start(){
        busGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StopButton(){
        busGameManager.StopBus();
        busScreenController.ApplyStopTexture();
    }
}
