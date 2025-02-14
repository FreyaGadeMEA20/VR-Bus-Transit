using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public int FieldOfView = 45;
    public int ViewDistance = 100;

    public float detectionRate = 1f;
    public float elapsedTime = 0f;

    [SerializeField] List<Transform> vehicleTrans = new List<Transform>();
    private Vector3 rayDirection;
    void Start()
    {
        elapsedTime = 0f;
        foreach(var vehicle in GameObject.FindGameObjectsWithTag("Detector")) {
            vehicleTrans.Add(vehicle.transform);
        }

        vehicleTrans.Remove(this.transform.parent);
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= detectionRate) {
            foreach(var vehicle in vehicleTrans) {
                if(DetectVehicle(vehicle)){
                    break;
                }
            }

            //add more knowledge to reset
        }
    }

    bool DetectVehicle(Transform vehicle) {
        RaycastHit hit;
        rayDirection = vehicle.position - transform.position;
        
        vehicle = this.transform;
        if(Vector3.Angle(rayDirection, transform.forward) < FieldOfView){
            Debug.Log("Sending Ray");
            if(Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance)) {
                if(hit.collider.gameObject.tag == "Detector") {
                    Debug.Log("Vehicle infront detected");
                    
                    if(DetermineIfBreak(hit)) {
                        vehicleInfront = true;
                        return true;
                    }
                } else if (vehicleInfront){
                    vehicleInfront = false;
                }
            }
        }

        elapsedTime = 0f;
        return false;
    }

    // Bunch of criteria to determine whether or not the bus should stop for the vehicle infront
    bool DetermineIfBreak(RaycastHit _hit){
        bool breakNow = false;

        float otherBusRotation = _hit.collider.gameObject.transform.parent.rotation.y;
        float thisBusRotation = this.transform.parent.rotation.y;
        float rotationDifference = Mathf.Abs(thisBusRotation - otherBusRotation);

        if(rotationDifference < 10f) {
            breakNow = true;
        }

        return breakNow;
    }

    bool vehicleInfront = false;
    public bool CheckForVehicleInfront() {
        return vehicleInfront;
    }

    void OnDrawGizmos() {
        foreach(var vehicle in vehicleTrans) {
            if (!Application.isEditor || vehicle == null)
                return;

            Debug.DrawLine(transform.position, vehicle.position, Color.red);

            Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

            //Approximate perspective visualization
            Vector3 leftRayPoint = Quaternion.Euler(0,FieldOfView * 0.5f ,0) * frontRayPoint;

            Vector3 rightRayPoint = Quaternion.Euler(0, - FieldOfView*0.5f, 0) *  frontRayPoint;

            Debug.DrawLine(transform.position, frontRayPoint, Color.green);
            Debug.DrawLine(transform.position, leftRayPoint, Color.green);
            Debug.DrawLine(transform.position, rightRayPoint, Color.green);
        }
    }
    /*List<GameObject> obstaclesInfront = new List<GameObject>();
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
    }*/


    /*public List<GameObject> GetVehiclesInfront() {
        List<GameObject> _obstaclesInfront = new List<GameObject>();
        foreach(GameObject obj in obstaclesInfront) {
            if(obj.tag == "Car" || obj.tag == "Bus"){
                _obstaclesInfront.Add(obj);
            }
        }
        return _obstaclesInfront;
    }*/
}
