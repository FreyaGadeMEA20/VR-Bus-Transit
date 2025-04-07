using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacher : MonoBehaviour
{
    [SerializeField] GameObject bus;

    [SerializeField] BusSeatAssigner busSeatAssigner;
    public bool PlayerDetachable = false;
    // On trigger enter
    //  - triggers when an object enters the area of the box collider
    void OnTriggerEnter(Collider other){
        // Only checking on the tag being the player...
        if(other.tag == "Player" && this.tag == "PlayerAttacher"){
            // ... then attaches the transform to the bus game object
            other.transform.parent.parent = bus.transform;

            Debug.Log("Player attached to bus");
            
            busSeatAssigner.GetPlayer(other.gameObject);

            PlayerDetachable = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player" && this.tag == "PlayerAttacher"){
            PlayerDetachable = true;
        }
    }
}
