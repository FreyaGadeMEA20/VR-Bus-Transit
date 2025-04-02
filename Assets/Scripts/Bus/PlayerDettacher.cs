using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDettacher : MonoBehaviour
{
    [SerializeField] GameObject bus;
    [SerializeField] BusSeatAssigner busSeatAssigner;
    [SerializeField] PlayerAttacher playerAttacher;

    // On trigger enter
    //  - triggers when an object enters the area of the box collider
    void OnTriggerExit(Collider other){
        // Only checking on the tag being the player...
        if(other.tag == "Player" && this.tag == "PlayerDettacher" && playerAttacher.PlayerDetachable){
            // ... then attaches the transform to the bus game object
            other.transform.parent.parent = null;

            Debug.Log("Player detached to bus");
            
            busSeatAssigner.GetPlayer(null);
        }
    }
}
