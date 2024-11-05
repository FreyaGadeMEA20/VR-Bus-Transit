using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    bool vehicleInfront = false;
    List<GameObject> objectsInside = new List<GameObject>();
    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Car" || other.gameObject.tag == "NPC") {
            Debug.Log("Player has collided with the object");
            vehicleInfront = true;
            objectsInside.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Car" || other.gameObject.tag == "NPC") {
            Debug.Log("Player has exited the object");
            objectsInside.Remove(other.gameObject);
            if(objectsInside.Count <= 0) 
                vehicleInfront = false;
        }
    }

    public bool CheckForVehicleInfront() {
        return vehicleInfront;
    }
}
