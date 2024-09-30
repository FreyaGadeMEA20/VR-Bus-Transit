using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

/// Parts taken from: https://github.com/unity-car-tutorials/Unity-CarTutorialProject-CSharp-Gotow/blob/master/Assets/GotowTutorialCSharp/Scripts/Car%20Control/AICar_Script.cs 
/// Parts taken from: https://www.youtube.com/watch?v=MXCZ-n5VyJc 

namespace Movement{
    public class VehicleMovement : MonoBehaviour
    {
        [Header("VEHICLE TYPE AND ITS PROPERTIES")]
        // Is it a car or a bus? Important for checking bus stop, and turning
        public EntityTypes entityType = EntityTypes.Car; 
        public enum EntityTypes {
            Car,
            Bus
        }

        // The current state of the vehicle. Whether it is moving or waiting
        public MovementState currentMovementState = MovementState.Moving;
        public enum MovementState{
            Moving,
            WaitingAtPoint,
            WaitingBehind,
        }

        Rigidbody rb; // The rigidbody of the vehicle

        [SerializeField] private BusController busController; // reference to the bus controller (doors, stop button, screen)

        // Wheel references
        [Header("WHEELS")]
        [SerializeField] WheelCollider[] frontWheels;
        [SerializeField] WheelCollider[] rearWheels;

        // ALl of the variables that are used to control the movement of the vehicle
        [Header("MOVEMENT VARIABLES")]
        [SerializeField] float maxSteering = 45f; // The maximum steering angle of the vehicle
        [SerializeField] float breakingForce = 300f; // The breaking force of the vehicle. I.e. how powerful the breaks are
        [SerializeField] float acceleration = 0f; // The acceleration of the vehicle
        [Range(-1,1)][SerializeField] float steering = 0f; //how much it is steering
        [SerializeField] bool breaks = false; // Whether or not the vehicle is breaking
        [SerializeField] float currentBreakForce = 0f; // The current break force of the vehicle. Checks the above, as breaks are not a bool, but a value

        public float[] GearRatio; // The gear ratio of the vehicle
        public int CurrentGear = 0; // The current gear of the vehicle
        public float EngineTorque = 600.0f; // The engine torque of the vehicle
        public float MaxEngineRPM = 3000.0f; // The maximum engine RPM of the vehicle
        public float MinEngineRPM = 1000.0f; // The minimum engine RPM of the vehicle
        float EngineRPM = 0.0f; // The current engine RPM of the vehicle

        // Variables for controlling navigation and collision
        [Header("NAVIGATION AND COLLISSION")]
        public bool ReachedBusStop = false; // Variable to control whether or not it has reached the bus stop
        bool reachedBusStop{
            get{return ReachedBusStop;}
            set{ReachedBusStop = value;}
        }
        RouteManager routeManager; // The base reference to its starting waypoint
        [SerializeField] Waypoint currentWaypoint; // The current waypoint, in which it navigates towards
        int direction = 1; // Control which waypoint to navigate towards. Might be removed in the future

        [SerializeField] GameObject collisionDetector; // The collision detectors of the vehicle
        bool vehicleInfront = false; // The vehicle infront of the current vehicle

        // Start is called before the first frame update
        void Start()
        {
            //waypoints = GameObject.Find("Vehicle Route Manager").GetComponent<RouteManager>();
            //carSpawner = GameObject.Find("Spawn Manager").GetComponent<CarSpawner>();
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3 (rb.centerOfMass.x, -1.5f, rb.centerOfMass.z);
            //currentRoute = waypoints.routes[currentRouteIndex];
            if (entityType == EntityTypes.Bus){
                busController = this.GetComponent<BusController>();
            } 

            routeManager = GetComponent<RouteManager>();
            //currentWaypoint = routeManager.currentWaypoint;

            //StartCoroutine(MovementSM());
        }

        /* private IEnumerator MovementSM(){
            while(true){
                switch (currentMovementState){
                    case MovementState.Moving:
                        //Move(1,0,0);

                        
                        //Debug.Log("Applying Forces");
                        //ApplyForces(1,0,false);
                        
                        break;

                    case MovementState.WaitingAtPoint:
                        //carSpawner.doSpawnCars = false;
                        // Let the cars go again when the waypoint is no longer a waiting point
                        if (entityType == EntityTypes.Car && currentWaypoint.STOP_VEHICLE == true){
                            yield return new WaitUntil(() => currentWaypoint.STOP_VEHICLE == false);
                            currentMovementState = MovementState.Moving;
                            break;
                        }

                        // TODO: Implement bus checking in
                        /* if(busController.firstTime){
                            busController.firstTime = false;
                        } 

                        yield return new WaitUntil(() => hasCheckedIn == true);
                        Debug.Log("Bus has checked in");
                        hasCheckedIn = false;
                        

                        currentMovementState = MovementState.Moving;
                        break;
                }

                yield return null;
            }
        } */

        void FixedUpdate(){
            // Current movement state keeps track of whether it is supposed to listen for waypoints, or be stopped
            switch(currentMovementState){
                case MovementState.Moving:
                    if(CheckIfCar()){
                        breaks = true;
                        break;
                    }
                    breaks = false;
                    rb.drag = rb.velocity.magnitude / 250;
                    NavigateTowardsWaypoint();
                    //MoveTowardsWaypoint();
                    //RotateTowardsWaypoint();

                    EngineRPM = (rearWheels[0].rpm + rearWheels[1].rpm)/2 * GearRatio[CurrentGear];
                    ShiftGears();

                    //audio.pitch = Mathf.Abs(EngineRPM) + 1;
                    // limit audio                    
                    break;
                case MovementState.WaitingAtPoint:
                    switch(currentWaypoint.waypointType){
                        case Waypoint.WaypointType.BusStop:
                            Debug.Log("Bus Stop");
                            if(entityType == EntityTypes.Bus && !reachedBusStop){
                                reachedBusStop = true;
                                breaks = true;
                                busController.StopBus();
                            }
                            // Add logic to tell the bus that it is at a bus stop
                            // - Open doors and set state
                            break;
                        case Waypoint.WaypointType.TrafficLight:
                            switch(currentWaypoint.TrafficState){
                                case Waypoint.TrafficLightState.Red:
                                    breaks = true;
                                    break;
                                case Waypoint.TrafficLightState.Green:
                                    currentMovementState = MovementState.Moving;
                                    break;
                            }
                            break;
                        case Waypoint.WaypointType.Nothing:
                            currentMovementState = MovementState.Moving;
                            break;
                    }
                    break;
            }
            // After the program has checked where it is in accordance to its surroundings, it will apply the forces to the wheels
            Move();
        }

        void  ShiftGears (){
            // this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
            // the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
            int AppropriateGear = CurrentGear;

            if ( EngineRPM >= MaxEngineRPM ) {
                for (int i= 0; i < GearRatio.Length; i ++ ) {
                    if ( rearWheels[0].rpm * GearRatio[i] < MaxEngineRPM ) {
                        AppropriateGear = i;
                        break;
                    }
                }
                
                CurrentGear = AppropriateGear;
            }
            
            if ( EngineRPM <= MinEngineRPM ) {
                AppropriateGear = CurrentGear;
                
                for ( int j= GearRatio.Length - 1; j >= 0; j -- ) {
                    if ( rearWheels[0].rpm * GearRatio[j] > MinEngineRPM ) {
                        AppropriateGear = j;
                        break;
                    }
                }
                
                CurrentGear = AppropriateGear;
            }
        }

        /// <summary>
        /// Moves the vehicle by the internal forces applied to the wheels
        /// </summary>
        public void Move(){
            // Apply forces to the front wheels
            currentBreakForce = breaks ? breakingForce : 0f;
            foreach (WheelCollider wheel in frontWheels) {
                wheel.steerAngle = maxSteering * steering;
                wheel.brakeTorque = currentBreakForce;
            }
            // Apply forces to the rear wheels
            foreach (WheelCollider wheel in rearWheels) {
                wheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * acceleration;
                wheel.brakeTorque = currentBreakForce;
            }
        }

        void NavigateTowardsWaypoint (){
            // now we just find the relative position of the waypoint from the car transform,
            // that way we can determine how far to the left and right the waypoint is.
            Vector3 RelativeWaypointPosition =
                        transform.InverseTransformPoint( new Vector3 (
                                                                    currentWaypoint.GetPosition().x, 
                                                                    transform.position.y, 
                                                                    currentWaypoint.GetPosition().z
                                                                )
                                                        );
            
            // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
            steering = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
            
            // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
            if ( Mathf.Abs( steering ) < 0.5f ) {
                acceleration = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs(steering);
            } else {
                acceleration = .5f;
            }
            
            // this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the
            // next in the list.
            if ( RelativeWaypointPosition.magnitude < 20 ) {
                switch(currentWaypoint.waypointType) {
                    case Waypoint.WaypointType.TrafficLight:
                        switch(currentWaypoint.TrafficState){
                            case Waypoint.TrafficLightState.Red:
                                currentMovementState = MovementState.WaitingAtPoint;
                                break;
                            case Waypoint.TrafficLightState.Green:
                                AdvanceToNextWaypoint();
                                break;
                        }
                        break;
                    case Waypoint.WaypointType.BusStop:
                        currentMovementState = MovementState.WaitingAtPoint;
                        break;
                    case Waypoint.WaypointType.Nothing:
                        AdvanceToNextWaypoint();
                        break;
                }
            }
        }

        public void AdvanceToNextWaypoint(){
            bool shouldBranch = false;
            // currentWaypoint.branches
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0){
                if(entityType == EntityTypes.Bus){
                    shouldBranch = true;//
                } else {
                    shouldBranch = Random.Range(0f,1f) <= currentWaypoint.branchRatio ? true : false;
                }
            }
            if(shouldBranch){
                Debug.Log("Branched");
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
            } else {
                if(direction == 1){
                    currentWaypoint = currentWaypoint.nextWaypoint;
                } else if (direction == -1){
                    currentWaypoint = currentWaypoint.previousWaypoint;
                }
            }
        }

        bool CheckIfCar(){
            // Send signal to the vehicle behind that it is safe to move
            return collisionDetector.GetComponent<CollisionDetector>().CheckForVehicleInfront();;
        }

        public void SetDestination(Waypoint destination){
            this.currentWaypoint = destination;
        }
    }
}
