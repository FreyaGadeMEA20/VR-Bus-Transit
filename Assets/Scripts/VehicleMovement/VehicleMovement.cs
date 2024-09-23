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
        /* [Range(0,1)] */[SerializeField] float wayPointAcc = 0f;
        [Range(-1,1)][SerializeField] float wayPointSteer = 0f;
        [SerializeField] bool WaypointBreak = false;

        [SerializeField] float currentAcceleration = 0f;
        [SerializeField] float currentBreakForce = 0f;
        [SerializeField] float currentSteering = 0f;

        public float[] GearRatio;
        public int CurrentGear = 0;
        public float EngineTorque = 600.0f;
        public float MaxEngineRPM = 3000.0f;
        public float MinEngineRPM = 1000.0f;
        float EngineRPM = 0.0f;

        [Header("NAVIGATION AND COLLISSION")]
        Vector3 destination;
        bool reachedDestination{
            get{return ReachedDestination;}
            set{ReachedDestination = value;}
        }
        public bool ReachedDestination = false;
        RouteManager routeManager;
        [SerializeField] Waypoint currentWaypoint;
        
        int direction = 1;
        Rigidbody rb;

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
            currentWaypoint = routeManager.currentWaypoint;

            //StartCoroutine(MovementSM());
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
                    rb.drag = rb.velocity.magnitude / 250;
                    NavigateTowardsWaypoint();
                    //MoveTowardsWaypoint();
                    //RotateTowardsWaypoint();

                    EngineRPM = (rearWheels[0].rpm + rearWheels[1].rpm)/2 * GearRatio[CurrentGear];
                    ShiftGears();

                    //audio.pitch = Mathf.Abs(EngineRPM) + 1;
                    // limit audio                    
                    break;
                case MovementState.Waiting:
                    WaypointBreak = true;
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

        // Possible TODO: Seperate the forces, and change how the steering is applied to the vehicle
        public void ApplyForces(float speed, float steering, bool breaking){
            currentAcceleration = wayPointAcc * acceleration * EngineRPM;
            currentSteering = wayPointSteer * maxSteering;
            
            currentBreakForce = (breaking || WaypointBreak) ? breakingForce : 0f;
        }

        /// <summary>
        /// Moves the vehicle by the internal forces applied to the wheels
        /// </summary>
        public void Move(){
            // Apply forces to the front wheels
            foreach (WheelCollider wheel in frontWheels){
                wheel.steerAngle = maxSteering * wayPointSteer;
                wheel.brakeTorque = currentBreakForce;
                //wheel.motorTorque = currentAcceleration;
            }
            // Apply forces to the rear wheels
            foreach (WheelCollider wheel in rearWheels){
                wheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * wayPointAcc;
                wheel.brakeTorque = currentBreakForce;
            }
        }

        void NavigateTowardsWaypoint (){
		// now we just find the relative position of the waypoint from the car transform,
		// that way we can determine how far to the left and right the waypoint is.
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint( new Vector3(
                                                                                    currentWaypoint.GetPosition().x, 
                                                                                    transform.position.y, 
                                                                                    currentWaypoint.GetPosition().z ) );
		
		// by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
		wayPointSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
		
		// now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
		if ( Mathf.Abs( wayPointSteer ) < 0.5f ) {
			wayPointAcc = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs( wayPointSteer );
		}else{
			wayPointAcc = 0.0f;
		}
		
		// this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the
		// next in the list.
		if ( RelativeWaypointPosition.magnitude < 20 ) {
            switch(currentWaypoint.waypointType){
                case Waypoint.WaypointType.BusStop:
                    currentMovementState = MovementState.Waiting;

                    // Add logic to tell the bus that it is at a bus stop
                    // - Open doors and set state
                    break;
                case Waypoint.WaypointType.TrafficLight:
                    Debug.Log("Traffic light reached");
                    switch(currentWaypoint.TrafficState){
                        case Waypoint.TrafficLightState.Red:
                            currentMovementState = MovementState.Waiting;
                            break;
                        case Waypoint.TrafficLightState.Green:
                            currentMovementState = MovementState.Moving;
                            break;
                    }
                    break;
                case Waypoint.WaypointType.Nothing:
                    if (currentWaypoint.nextWaypoint == null){
                        reachedDestination = true;
                    } else {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    break;
            }
            bool shouldBranch = false;
            // currentWaypoint.branches
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0){
                Debug.Log("Branching");
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

            
            Debug.Log("Waypoint Reached");
			/* if ( currentWaypoint >= waypoints.Count ) {
				currentWaypoint = 0;
			} */
		}
		
	}

        public void SetDestination(Vector3 destination){
            this.destination = destination;
        }
    }
}
