using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopScript : MonoBehaviour
{
    public BusScreenController busScreenController;
    [SerializeField] BusController busController;

    void Start(){
        busController = this.GetComponent<BusController>();
//        busController = GameObject.Find("BusController").GetComponent<BusController>();
    }

    public void StopButton(){
        busController.StopBus();
        busScreenController.ApplyStopTexture();
    }
}
