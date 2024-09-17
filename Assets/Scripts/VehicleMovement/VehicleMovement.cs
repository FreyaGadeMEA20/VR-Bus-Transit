using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Movement{
    public class VehicleMovement : MonoBehaviour
    {
        [Header("VEHICLE TYPE AND ITS PROPERTIES")]
        public EntityTypes entityType = EntityTypes.Car;
        public enum EntityTypes {
            Car,
            Bus
        }

        public MovementState currentMovementState = MovementState.Moving;
        public enum MovementState{
            Moving,
            Waiting
        }

        [SerializeField] private BusController busController;

        [Header("WHEELS")]
        [SerializeField] WheelCollider[] frontWheels;
        [SerializeField] WheelCollider[] rearWheels;

        [Header("MOVEMENT VARIABLES")]
        [SerializeField] float acceleration = 1000f;
        [SerializeField] float maxSteering = 20f;
        [SerializeField] float breakingForce = 300f;
        [Range(0,1)][SerializeField] float wayPointAcc = 0f;
        [Range(0,1)][SerializeField] float wayPointSteer = 0f;
        [SerializeField] bool WaypointBreak = false;

        float currentAcceleration = 0f;
        float currentBreakForce = 0f;
        float currentSteering = 0f;

        [Header("NAVIGATION AND COLLISSION")]
        Vector3 destination;
        bool reachedDestination{
            get{return ReachedDestination;}
            set{ReachedDestination = value;}
        }
        public bool ReachedDestination = false;
        Waypoint currentWaypoint;
        [SerializeField] GameObject waypointDetector;
        [SerializeField] float safeDistance = 2f;
        

        // Start is called before the first frame update
        void Start()
        {
            //waypoints = GameObject.Find("Vehicle Route Manager").GetComponent<RouteManager>();
            //carSpawner = GameObject.Find("Spawn Manager").GetComponent<CarSpawner>();

            //currentRoute = waypoints.routes[currentRouteIndex];
            if (entityType == EntityTypes.Bus){
                busController = this.GetComponent<BusController>();
            } 

            StartCoroutine(MovementSM());
        }

        private IEnumerator MovementSM(){
            while(true){
                switch (currentMovementState){
                    case MovementState.Moving:
                        //Move(1,0,0);

                        //Debug.Log("Applying Forces");
                        //ApplyForces(1,0,false);
                        
                        break;

                    case MovementState.Waiting:
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
                        */

                        currentMovementState = MovementState.Moving;
                        break;
                }

                yield return null;
            }
        }

        void FixedUpdate(){
            // Current movement state keeps track of whether it is supposed to listen for waypoints, or be stopped
            switch(currentMovementState){
                case MovementState.Moving:
                    //MoveTowardsWaypoint();
                    //RotateTowardsWaypoint();
                    
                    break;
            }
            // After the program has checked where it is in accordance to its surroundings, it will apply the forces to the wheels
            Move();
        }

        // Possible TODO: Seperate the forces, and change how the steering is applied to the vehicle
        public void ApplyForces(float speed, float steering, bool breaking){
            currentAcceleration = (speed * wayPointAcc) * acceleration;
            currentSteering = (steering * wayPointSteer) * maxSteering;
            
            currentBreakForce = (breaking || WaypointBreak) ? breakingForce : 0f;
        }

        /// <summary>
        /// Moves the vehicle by the internal forces applied to the wheels
        /// </summary>
        public void Move(){
            // Apply forces to the front wheels
            foreach (WheelCollider wheel in frontWheels){
                wheel.steerAngle = currentSteering;
                wheel.brakeTorque = currentBreakForce;
                //wheel.motorTorque = currentAcceleration;
            }
            // Apply forces to the rear wheels
            foreach (WheelCollider wheel in rearWheels){
                wheel.motorTorque = currentAcceleration;
                wheel.brakeTorque = currentBreakForce;
            }
        }

        public void SetDestination(Vector3 destination){
            this.destination = destination;
        }
    }
}
