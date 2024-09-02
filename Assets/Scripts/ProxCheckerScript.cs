using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxCheckerScript : MonoBehaviour {
    [SerializeField] GameObject sign;
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
            // Perform actions when player enters proximity
            Debug.Log("Player is within proximity");
            playerInProx = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")){
            playerInProx = false;
            Debug.Log("Player exits proximity");
        }
    }

    public bool CheckPlayerProximity(){
        return playerInProx;
    }
}
