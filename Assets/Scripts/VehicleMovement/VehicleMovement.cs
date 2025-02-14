using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
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
            BackingUp,
        }

        [HideInInspector] public Rigidbody rb; // The rigidbody of the vehicle

        [SerializeField] private BusController busController; // reference to the bus controller (doors, stop button, screen)

        // Wheel references
        [System.Serializable]
        public class Wheel
        {
            public WheelCollider wheels;
            public Transform transforms;
        }

        [Header("WHEELS")]
        [SerializeField] Wheel[] frontWheels;
        [SerializeField] Wheel[] rearWheels;

        // ALl of the variables that are used to control the movement of the vehicle
        [Header("MOVEMENT VARIABLES")]
        [SerializeField] float maxSteering = 45f; // The maximum steering angle of the vehicle
        [SerializeField] float breakingForce = 300f; // The breaking force of the vehicle. I.e. how powerful the breaks are
        [SerializeField] float acceleration = 0f; // The acceleration of the vehicle
        float tempAcc; // Temporary acceleration value. For backing it up
        [Range(-1,1)][SerializeField] float steering = 0f; //how much it is steering
        [SerializeField] bool breaks = false; // Whether or not the vehicle is breaking
        [SerializeField] float currentBreakForce = 0f; // The current break force of the vehicle. Checks the above, as breaks are not a bool, but a value

        public float[] GearRatio; // The gear ratio of the vehicle
        public int CurrentGear = 0; // The current gear of the vehicle
        public float EngineTorque = 600.0f; // The engine torque of the vehicle
        public float MaxEngineRPM = 3000.0f; // The maximum engine RPM of the vehicle
        public float MinEngineRPM = 1000.0f; // The minimum engine RPM of the vehicle
        float EngineRPM = 0.0f; // The current engine RPM of the vehicle

        [Range(1, 4)][SerializeField] float speedModifier = 4f; // The speed modifier of the vehicle
        // Variables for controlling navigation and collision
        [Header("NAVIGATION AND COLLISSION")]
        public bool ReachedBusStop = false; // Variable to control whether or not it has reached the bus stop
        bool reachedBusStop{
            get{return ReachedBusStop;}
            set{ReachedBusStop = value;}
        }
        RouteManager routeManager{
            get{return _RouteManager;}
            set{_RouteManager = value;}
        } // The base reference to its starting waypoint

        public RouteManager _RouteManager;

        int direction = 1; // Control which waypoint to navigate towards. Might be removed in the future

        [SerializeField] GameObject collisionDetector; // The collision detectors of the vehicle
        bool BackingUpIntiated = false; // Whether or not the vehicle is backing up
        public bool TrafficLightClear = false; // Whether or not the traffic light is clear

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = Vector3.zero;
            //currentRoute = waypoints.routes[currentRouteIndex];
            if (entityType == EntityTypes.Bus){
                busController = this.GetComponent<BusController>();
            } 

            routeManager = GetComponent<RouteManager>();
            rb.inertiaTensorRotation = Quaternion.identity;
            //currentWaypoint = routeManager.currentWaypoint;

            //StartCoroutine(MovementSM());
        }

        void Update(){
            busController.UpdateBusController();
        }

        void FixedUpdate(){
            // Current movement state keeps track of whether it is supposed to listen for waypoints, or be stopped
            switch(currentMovementState){
                case MovementState.Moving:
                    if(CheckIfCar()){
                        breaks = true;
                        // TODO: backing up needs more work
                        currentMovementState = MovementState.BackingUp;
                        break;
                    }
                    breaks = false;
                    rb.drag = rb.velocity.magnitude / 250;
                    NavigateTowardsWaypoint();
                    //MoveTowardsWaypoint();
                    //RotateTowardsWaypoint();
                    //Update wheel meshes
                    

                    EngineRPM = (rearWheels[0].wheels.rpm + rearWheels[1].wheels.rpm)/2 * GearRatio[CurrentGear];
                    ShiftGears();

                    //audio.pitch = Mathf.Abs(EngineRPM) + 1;
                    // limit audio                    
                    break;
                case MovementState.WaitingAtPoint:
                    switch(routeManager.currentWaypoint.waypointType){
                        case Waypoint.WaypointType.BusStop:
                            if(entityType == EntityTypes.Bus && !reachedBusStop){
                                reachedBusStop = true;
                                breaks = true;
                                routeManager.currentWaypoint.busStop.deathZone.SetActive(false);
                                busController.StopBus();
                                busController.screens.GiveInformation(); // change the bus screen texture
                                routeManager.SetRoute();
                            }
                            // Add logic to tell the bus that it is at a bus stop
                            // - Open doors and set state
                            break;
                        case Waypoint.WaypointType.TrafficLight:
                            switch(routeManager.currentWaypoint.TrafficState){
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
                case MovementState.BackingUp:
                    // Needs more work
                    // Drive if nothing is in front
                    
                    if(!CheckIfCar()){
                        currentMovementState = MovementState.Moving;
                        BackingUpIntiated = false;
                        acceleration = tempAcc;
                        StopCoroutine(DriveBackwards());
                        break;
                    }
                    if(!BackingUpIntiated){
                        StartCoroutine(DriveBackwards());
                    }
                    
                    break;
            }

            // After the program has checked where it is in accordance to its surroundings, it will apply the forces to the wheels
            Move();
        }

        // TODO: needs more work
        IEnumerator DriveBackwards(){
            BackingUpIntiated = true;
            acceleration = acceleration*-1f;
            speedModifier = 3f;
            yield return new WaitForSeconds(4f);

            breaks=false;
        }

        void  ShiftGears (){
            // this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
            // the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
            int AppropriateGear = CurrentGear;

            if (EngineRPM >= MaxEngineRPM) {
                for (int i= 0; i < GearRatio.Length; i++) {
                    if (rearWheels[0].wheels.rpm * GearRatio[i] < MaxEngineRPM) {
                        AppropriateGear = i;
                        break;
                    }
                }
                
                CurrentGear = AppropriateGear;
            }
            
            if (EngineRPM <= MinEngineRPM) {
                AppropriateGear = CurrentGear;
                
                for (int j = GearRatio.Length - 1; j >= 0; j--) {
                    if (rearWheels[0].wheels.rpm * GearRatio[j] > MinEngineRPM) {
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
            foreach (Wheel wheel in frontWheels) {
                wheel.wheels.steerAngle = maxSteering * steering;
                wheel.wheels.brakeTorque = currentBreakForce;

                UpdateWheel(wheel.wheels, wheel.transforms);
            }
            // Apply forces to the rear wheels
            foreach (Wheel wheel in rearWheels) {
                wheel.wheels.motorTorque = EngineTorque / GearRatio[CurrentGear] * acceleration * speedModifier;
                wheel.wheels.brakeTorque = currentBreakForce;

                UpdateWheel(wheel.wheels, wheel.transforms);
            }
        }

        // Script to move the vehicle towards the waypoint
        void NavigateTowardsWaypoint (){
            // now we just find the relative position of the waypoint from the car transform,
            // that way we can determine how far to the left and right the waypoint is.
            Vector3 RelativeWaypointPosition =
                        transform.InverseTransformPoint( new Vector3 (
                                                                    routeManager.currentWaypoint.GetPosition().x, 
                                                                    transform.position.y, 
                                                                    routeManager.currentWaypoint.GetPosition().z
                                                                )
                                                        );
            
            // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
            steering = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
            steering = (float)Math.Round(steering, 2);
            // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
            if ( Mathf.Abs( steering ) < 0.2f ) {
                acceleration = (float)Math.Round(RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs(steering), 2);
                speedModifier = 3f;
            } else {
                if(!BackingUpIntiated) {
                    speedModifier = 1f;
                } 
            }
            
            // this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the
            // next in the list.
            if ( RelativeWaypointPosition.magnitude < 20 ) {
                switch(routeManager.currentWaypoint.waypointType) {
                    case Waypoint.WaypointType.TrafficLight:
                        switch(routeManager.currentWaypoint.TrafficState){
                            case Waypoint.TrafficLightState.Red:
                                currentMovementState = MovementState.WaitingAtPoint;
                                break;
                            case Waypoint.TrafficLightState.Green:
                                if(routeManager.currentWaypoint.EvaluateTrafficLight()){
                                    AdvanceToNextWaypoint();
                                }
                                //AdvanceToNextWaypoint();
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



        // Function to advance to the next waypoint
        public void AdvanceToNextWaypoint(){
            // Control variables to control whether or not to advance
            bool shouldBranch = false;
            reachedBusStop = false;
            if(routeManager.currentWaypoint.busStop != null){
                routeManager.currentWaypoint.busStop.deathZone.SetActive(true);
            }

            // Determines if the current waypoint has branches
            //  - Only relevant for cars, as bus follow a set route
            if (routeManager.currentWaypoint.branches != null && routeManager.currentWaypoint.branches.Count > 0){
                shouldBranch = UnityEngine.Random.Range(0f,1f) <= routeManager.currentWaypoint.branchRatio;
            }

            // If the vehicle has a set path (i.e. bus route), it will follow that path
            if (routeManager.m_Path != null){
                routeManager.currentWaypoint = routeManager.GetNextWaypoint(routeManager.currentWaypoint);
            } else if (shouldBranch) { // Should the bus branch?
                routeManager.currentWaypoint = routeManager.currentWaypoint.branches[UnityEngine.Random.Range(0, routeManager.currentWaypoint.branches.Count)];
            } else { // If the vehicle has no path to follow, and shouldn't branch, it will just continue onwards
                if(direction == 1){
                    routeManager.currentWaypoint = routeManager.currentWaypoint.nextWaypoint;
                } else if (direction == -1){
                    routeManager.currentWaypoint = routeManager.currentWaypoint.previousWaypoint;
                }
            }
        }

        bool CheckIfCar(){
            // Checks if there is a vehicle infront of the current vehicle
            return collisionDetector.GetComponent<CollisionDetector>().CheckForVehicleInfront();
        }

        void UpdateWheel(WheelCollider col, Transform trans) 
        {
            //Get wheel collider state
            Vector3 position;
            Quaternion rotation;
            col.GetWorldPose(out position, out rotation);

            // Set wheel transform state
            trans.position = position;
            trans.rotation = rotation;
        }
    }
}
