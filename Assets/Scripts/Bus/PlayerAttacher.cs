using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;   
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerAttacher : MonoBehaviour
{
    [SerializeField] GameObject bus;
    [SerializeField] GameObject player;

    [SerializeField] BusSeatAssigner busSeatAssigner;
    // Start is called before the first frame update
    ConstraintSource constraintSource;
    void Start()
    {
        player = GameObject.Find("XR Player");
    }

    // On trigger enter
    //  - triggers when an object enters the area of the box collider
    void OnTriggerEnter(Collider other){
        // Only checking on the tag being the player...
        if(other.tag == "Player"){
            // ... then attaches the transform to the bus game object
            other.transform.parent = bus.transform;
            
            busSeatAssigner.GetPlayer(other.gameObject);
        }

    }
    
    // On trigger exit
    //  - triggers when an object exits the area of the box collider
    void OnTriggerExit(Collider other){
        // Only checking on the tag being the player...
        if (other.tag == "Player"){
            // ... then sets the player to have no parent
            other.transform.parent = null;
        }
    }
}
