using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopScript : MonoBehaviour
{
    public BusScreenController busScreenController;
    [SerializeField] BusController busController;
    public bool Active = false;

    void Start(){
        busController = GetComponentInParent<BusController>();
//        busController = GameObject.Find("BusController").GetComponent<BusController>();
    }

    public void StopButton(){
        if(Active){
            busController.StopBus();
            busScreenController.ApplyStopTexture();       
        }
    }
}
