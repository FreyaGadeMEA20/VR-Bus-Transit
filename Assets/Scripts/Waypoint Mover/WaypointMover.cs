using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointMover : MonoBehaviour
{
    public Waypoints waypoints;
    public CarSpawner carSpawner;
    public WaypointClass waypointClass;

    public BusController busController;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float safeDistance = 2f;

    private Transform currentWaypoint;

    [SerializeField] private bool canMove{
        get{return CanMove;}
        set{CanMove = value;}
    }

    public bool CanMove = true;

    public int routeIndex;

    public enum EntityTypes {
        Car,
        Bus
    }
    public EntityTypes entityType = EntityTypes.Car;

    public bool hasCheckedIn = false;

    private enum MovementState{
        Moving,
        Waiting
    }

    [SerializeField] private MovementState currentMovementState;
    Rigidbody rb;


    private IEnumerator MovementSM(){
        while(true){
            switch (currentMovementState){
                case MovementState.Moving:
                    rb.constraints = RigidbodyConstraints.FreezePositionY & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
                    //carSpawner.doSpawnCars = true;
                    break;

                case MovementState.Waiting:
                    //carSpawner.doSpawnCars = false;
                    // Let the cars go again when the waypoint is no longer a waiting point
                    if (entityType == EntityTypes.Car && waypointClass.isWaitingPoint == true){
                        //Debug.Log("SM Waiting point: "+ currentWaypoint.name);
                        yield return new WaitUntil(() => waypointClass.isWaitingPoint == false);
                        //Debug.Log("Car is no longer at a waiting point");
                        currentMovementState = MovementState.Moving;
                        break;
                    }

                    if(busController.firstTime){
                        busController.firstTime = false;
                    }

                    yield return new WaitUntil(() => hasCheckedIn == true);
                    Debug.Log("Bus has checked in");
                    hasCheckedIn = false;

                    //yield return new WaitForSeconds(waypointClass.waitingTime);

                    //carSpawner.doSpawnCars = true;

                    currentMovementState = MovementState.Moving;
                    break;
            }

            yield return null;
        }
    }

    void Start(){
        waypoints = GameObject.Find("Waypoints").GetComponent<Waypoints>();
        carSpawner = GameObject.Find("Spawn Manager").GetComponent<CarSpawner>();

        if (entityType == EntityTypes.Bus){
            busController = this.GetComponent<BusController>();
//            busController = GameObject.Find("BusController").GetComponent<BusController>();
            busController.GetWPM(this.GetComponent<WaypointMover>(), this.GetComponent<DoorController>());
        }

        StartCoroutine(GiveSelf());

        rb = GetComponent<Rigidbody>();


        if (carSpawner == null){
            Debug.LogWarning("CarSpawner component not found");
        }
        //routeIndex = carSpawner.routeIndex;
        //Debug.Log("Chosen Route index: " + carSpawner.routeIndex);

        currentMovementState = MovementState.Moving;

        // Set the current waypoint to the first waypoint in the list
        currentWaypoint = waypoints.routes[carSpawner.routeIndex].waypoints[0];
        //Debug.Log("Current waypoint: " + currentWaypoint.position);

        // Set the current waypoint to the first waypoint in the list
        //currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        //transform.position = currentWaypoint.position;

        // Move the object towards the current waypoint
        //StartCoroutine(MoveTowardsWaypoint());
        StartCoroutine(MovementSM());
    }

    IEnumerator GiveSelf(){
        yield return new WaitForSeconds(1);

    }
    void FixedUpdate(){
        switch(currentMovementState){
            case MovementState.Moving:
                MoveTowardsWaypoint();
                RotateTowardsWaypoint();
                break;
        }
        //CheckIfCanMove();

        routeIndex = carSpawner.routeIndex;
    }

    private void RotateTowardsWaypoint(){
        if (currentWaypoint == null) return;    // If the current waypoint is null, return

        // Calculate the direction to the current waypoint
        Vector3 direction = currentWaypoint.position - transform.position;
        direction.y = 0;

        // Rotate the object to face the current waypoint
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }
    
    private void MoveTowardsWaypoint(){
        if (canMove){
            // Destroy vehicle if there is no waypoints left
            if (waypoints.doLoop == false && currentWaypoint == null){
                Destroy(gameObject);
                carSpawner.DecrementActiveCars();
                Debug.Log("No waypoints left, destroying object");
                return;
            }

            Vector3 targetPosition = currentWaypoint.position;
            targetPosition.y = transform.position.y;
            
            // Move towards the waypoint
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            float distanceToPoint = Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosition.x, targetPosition.z));

            // If we are at the waypoint, find the next
            if (distanceToPoint <= 0.1f){
                waypointClass = currentWaypoint.GetComponent<WaypointClass>();

                //Debug.Log("Waiting point: "+ currentWaypoint.name);

                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint, carSpawner.routeIndex);

                // If the bus is at the bus stop, wait
                /* if (waypointClass.isBusStop && entityType == "Bus"){
                    currentMovementState = MovementState.Waiting;
                    rb.constraints = RigidbodyConstraints.FreezePosition & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
                } */

                if (waypointClass.isBusStop && entityType == EntityTypes.Bus && (busController.BusStopped || busController.firstTime)){
                    currentMovementState = MovementState.Waiting;
                    rb.constraints = RigidbodyConstraints.FreezePosition & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
                } else if (waypointClass.isBusStop && entityType == EntityTypes.Bus){
                    Debug.Log(":)");
                }

                // if a car is at a waiting point, wait
                if (waypointClass.isWaitingPoint && entityType == EntityTypes.Car){
                    //Debug.Log("Car is at a waiting point, waiting...");
                    //Debug.Log("WAITING Waiting point: "+ currentWaypoint.name);
                    currentMovementState = MovementState.Waiting;
                    rb.constraints = RigidbodyConstraints.FreezePosition & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;

                }
            }
        }

    }

    private void CheckIfCanMove(){
        // Check for overlaps with other colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * safeDistance, safeDistance);
        foreach (var collider in colliders){
            if (collider.CompareTag("Car") || collider.CompareTag("Bus") || collider.CompareTag("Player")){
                Debug.Log("Car detected");
                canMove = false;
                return;
            }
        }

        canMove = true;
    }

    private void OnTriggerEnter(Collider other){
        //Debug.Log("Object entered trigger");

        if (other.CompareTag("Car")){
            //Debug.Log("Car entered");
                
            // Check if car is in front of the object
            Vector3 direction = other.transform.position - transform.position;
            float dotProduct = Vector3.Dot(direction, transform.forward);

            if (dotProduct > 0){
                canMove = false;
            }
        }
    }

    private void OnTriggerExit(Collider other){
        //Debug.Log("Object exited trigger");
        
        if (other.CompareTag("Car")){
            //Debug.Log("Car exited");
            canMove = true;
        }
    }

/*     private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * safeDistance, safeDistance);
    } */
}

