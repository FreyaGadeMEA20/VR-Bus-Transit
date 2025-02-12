using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    bool vehicleInfront = false;
    List<GameObject> obstaclesInfront = new List<GameObject>();
    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Car" || other.gameObject.tag == "NPC") {
            Debug.Log("Player has collided with the object");
            vehicleInfront = true;
            obstaclesInfront.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Car" || other.gameObject.tag == "NPC") {
            Debug.Log("Player has exited the object");
            obstaclesInfront.Remove(other.gameObject);
            if(obstaclesInfront.Count <= 0) 
                vehicleInfront = false;
        }
    }

    public bool CheckForVehicleInfront() {
        return vehicleInfront;
    }

    public List<GameObject> GetVehiclesInfront() {
        List<GameObject> _obstaclesInfront = new List<GameObject>();
        foreach(GameObject obj in obstaclesInfront) {
            if(obj.tag == "Car" || obj.tag == "Bus"){
                _obstaclesInfront.Add(obj);
            }
        }
        return _obstaclesInfront;
    }
}
