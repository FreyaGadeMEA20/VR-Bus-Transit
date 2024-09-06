using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleMovement{
    public class VehicleMovement : MonoBehaviour
    {
        [SerializeField] WheelCollider[] frontWheels;
        [SerializeField] WheelCollider[] rearWheels;

        [SerializeField] float acceleration = 1000f;
        [SerializeField] float maxSteering = 20f;
        [SerializeField] float breakingForce = 300f;
        float currentAcceleration = 0f;
        float currentBreakForce = 0f;
        float currentSteering = 0f;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate(){
            currentAcceleration = Input.GetAxis("Vertical") * acceleration;
            currentSteering = Input.GetAxis("Horizontal") * maxSteering;
            currentBreakForce = Input.GetKey(KeyCode.Space) ? breakingForce : 0f;

            Move(currentAcceleration, currentSteering);
        }

        /// <summary>
        /// Moves the vehicle by setting the speed and steering.
        /// </summary>
        /// <param name="speed">The speed at which the vehicle should move.</param>
        /// <param name="steering">The steering angle of the vehicle.</param>
        
        public void Move(float speed, float steering){
            Debug.Log("Accelerating: "+speed);
            Debug.Log("Steering: "+steering);
            Debug.Log("Breaking: "+currentBreakForce);
            foreach (WheelCollider wheel in frontWheels){
                wheel.steerAngle = currentSteering;
                //wheel.motorTorque = currentAcceleration;
                wheel.brakeTorque = currentBreakForce;
            }

            foreach (WheelCollider wheel in rearWheels){
                wheel.motorTorque = currentAcceleration;
                wheel.brakeTorque = currentBreakForce;
            }
        }
    }
}
