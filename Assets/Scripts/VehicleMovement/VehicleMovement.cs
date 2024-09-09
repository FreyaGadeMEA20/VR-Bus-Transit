using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Movement{
    public class VehicleMovement : MonoBehaviour
    {
        [Header("WHEELS")]
        [SerializeField] WheelCollider[] frontWheels;
        [SerializeField] WheelCollider[] rearWheels;

        [Header("MOVEMENT VARIABLES")]
        [SerializeField] float acceleration = 1000f;
        [SerializeField] float maxSteering = 20f;
        [SerializeField] float breakingForce = 300f;

        [Header("CURRENT MOVEMENT VARIABLES")]
        [SerializeField] float currentAcceleration = 0f;
        [SerializeField] float currentBreakForce = 0f;
        [SerializeField] float currentSteering = 0f;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate(){
            Move(currentAcceleration, currentSteering, currentBreakForce);
        }

        public void ApplyForces(float speed, float steering, bool breaking){
            currentAcceleration = speed * acceleration;
            currentSteering = steering * maxSteering;
            
            currentBreakForce = breaking ? breakingForce : 0f;
        }

        /// <summary>
        /// Moves the vehicle by setting the speed and steering.
        /// </summary>
        /// <param name="speed">The speed at which the vehicle should move.</param>
        /// <param name="steering">The steering angle of the vehicle.</param>
        
        public void Move(float speed, float steering, float breakF){
            Debug.Log("Steering: "+steering);
            Debug.Log("Breaking: "+breakF);
            foreach (WheelCollider wheel in frontWheels){
                wheel.steerAngle = steering;
                //wheel.motorTorque = currentAcceleration;
                wheel.brakeTorque = breakF;
            }

            foreach (WheelCollider wheel in rearWheels){
                
                Debug.Log("Accelerating: "+speed);
                wheel.motorTorque = currentAcceleration;
                wheel.brakeTorque = breakF;
            }
        }
    }
}
