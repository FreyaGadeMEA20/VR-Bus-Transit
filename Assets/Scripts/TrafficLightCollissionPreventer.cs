
using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

public class TrafficLightCollissionPreventer : MonoBehaviour
{

// TODO: needs to be moved to a seperate code, so it doesn't also check the crosswalks
    void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject.CompareTag("Bus") || other.gameObject.CompareTag("Car")){
            GetComponentInParent<TrafficLightController>().AddVehicles();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Bus") || other.gameObject.CompareTag("Car")){
            GetComponentInParent<TrafficLightController>().RemoveVehicles();
        }
    }

}