using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Movement;

// Detection tool to prevent crashing
public class CollisionDetector : MonoBehaviour
{
    public int FieldOfView = 45;
    public int ViewDistance = 100;

    public float detectionRate = .25f;
    public float elapsedTime = 0f;

    [SerializeField] List<Transform> vehicleTrans = new List<Transform>();
    private Vector3 rayDirection;
    void Start()
    {
        elapsedTime = 0f;

        // Gets all the vehicles in the scene
        foreach(var vehicle in GameObject.FindGameObjectsWithTag("Detector")) {
            vehicleTrans.Add(vehicle.transform);
        }

        vehicleTrans.Remove(this.transform.parent);
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        // Prevents it from running EACH frame
        if(elapsedTime >= detectionRate) {
            foreach(var vehicle in vehicleTrans) {
                if(DetectVehicle(vehicle)){
                    break;
                }
            }

            //add more knowledge to reset
        }
    }

    // Script to try and mitigate bus crashing
    // This works by sending a raycast to the vehicle infront of the bus
    // and then telling the detector there's a vehicle infront
    bool DetectVehicle(Transform vehicle) {
        // Makes a ray and shoots it towards the given transform
        RaycastHit hit;
        rayDirection = vehicle.position - transform.position;
        
        //vehicle = this.transform;

        // Checks to make sure the bus is within the field of view
        // No need to check behind
        if(Vector3.Angle(rayDirection, transform.forward) < FieldOfView){
            // Checks if the bus is within the view distance
            if(Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance)) {
                // Checks if the object hit is a detector
                // Prevents accidental double detection and false positives
                if(hit.collider.gameObject.tag == "Detector") {
                    // Checks if the bus should stop
                    if(DetermineIfBreak(hit)) {
                        // If the bus should stop, then set the vehicleInfront to true
                        vehicleInfront = true;
                        Debug.Log("Vehicle infront detected");
                        return true; // return true to break the loop
                    }
                } else if (vehicleInfront){
                    vehicleInfront = false;
                }
            }
        }

        elapsedTime = 0f; // not checking EVERY frame
        return false;
    }

    // Bunch of criteria to determine whether or not the bus should stop for the vehicle infront
    // This works, but is pretty bad.
    bool DetermineIfBreak(RaycastHit _hit){
        bool breakNow = false; // controls whether or not the bus should stop
        bool[] conditions = new bool[2]; // conditions to determine if it should stop

        // Checks if the bus is driving parallel to it, so it doesn't see a bus it won't hit
        Vector3 otherBusRotation = _hit.collider.gameObject.transform.parent.rotation.eulerAngles;
        Vector3 thisBusRotation = this.transform.parent.parent.rotation.eulerAngles;
        float rotationDifference = Mathf.Abs((thisBusRotation.y % 180)  - (otherBusRotation.y % 180));

        if(rotationDifference > 2f && rotationDifference < 178f) {
            Debug.Log("Rotation Difference: " + rotationDifference + " between " + this.transform.parent.parent.name + " and " + _hit.collider.gameObject.transform.parent.name);
            conditions[0] = true;
        } else {
            conditions[0] = false;
        }

        // checks if the bus infront is holding still
        if(_hit.collider.gameObject.transform.GetComponentInParent<VehicleMovement>().Breaks) {
            conditions[1] = false;
        } else {
            conditions[1] = true;
        }

        if(conditions[0] && conditions[1]) {
            breakNow = true;
        } else {
            breakNow = false;
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
