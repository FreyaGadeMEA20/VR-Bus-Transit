using System;
using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

public class BusStop : MonoBehaviour {
    [HideInInspector] public GameObject sign;

    GameObject player;

    public List<BusLineSO> associatedLines; 
    public Waypoint associatedWaypoint;
    
    bool playerInProx { get; set; }

    // Start is called before the first frame update
    void Start() {
        sign = this.gameObject;
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            player = other.gameObject;
            // Perform actions when player enters proximity
            Debug.Log("Player is within proximity");
            playerInProx = CheckValidStop();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")){
            playerInProx = false;
            Debug.Log("Player exits proximity");
        }
    }

    public bool CheckValidStop(){
        foreach (var line in associatedLines)
        {
            if(GameManager.Instance.BusLine.BusLineID.v2 == associatedWaypoint.name){
                return true;
            } 
        }
        return false;
    }

    public void AddBusLine(BusLineSO line){
        associatedLines.Add(line);
        Debug.Log("Bus Line added to stop");
    }

    public bool CheckPlayerProximity(){
        return playerInProx;
    }
}
