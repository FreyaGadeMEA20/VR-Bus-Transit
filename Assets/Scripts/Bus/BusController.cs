using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using System;

public class BusController : MonoBehaviour
{
    DoorController doors;
    BusScreenController screens;

    public Movement.VehicleMovement vehicleMovement;

    // What is the state of the bus
    // NOT BASED ON THE VEHICLE MOVEMENT, ONLY FOR WAITING AT STOPS
    public enum BusState {
        DRIVING,
        WAIT,
        STOP_BUTTON_PRESSED,
    }
    [SerializeField] BusState busState;

    bool animationPlayed = false;

    private bool driveAllowed{
        get{return DriveAllowed;}
        set{DriveAllowed = value;}
    }
    public bool DriveAllowed;

    private bool busStopped{
        get{return BusStopped;}
        set{BusStopped = value;}
    }
    public bool BusStopped;
    
    public bool firstTime = true;

    private bool hasCheckedIn{
        get{return HasCheckedIn;}
        set{HasCheckedIn = value;}
    }

    public bool HasCheckedIn;

    [Range(-1,1)] [SerializeField] float speed = 0;
    [Range(-1,1)] [SerializeField] float steering = 0;
    [SerializeField] bool brake = true;
    bool startDriving = true;
    bool stopDriving = true;

    // Start is called before the first frame update
    void Start()
    {
        busState = BusState.STOP_BUTTON_PRESSED;
        vehicleMovement = GetComponent<VehicleMovement>();
        doors = GetComponent<DoorController>();
        screens = GetComponent<BusScreenController>();
        
    }

    void Update(){
        switch(busState){
            case BusState.DRIVING:
                UpdateDriveState();
                break;
            case BusState.WAIT:
                UpdateWaitState();
                break;
            case BusState.STOP_BUTTON_PRESSED:
                UpdateStopState();
                break;
        }
    }

    private void UpdateDriveState()
    {
        if(startDriving){
            startDriving = false;
            brake = false;
            StartCoroutine(IncrementSpeed());
        }
    }

    void FixedUpdate()
    {
        vehicleMovement.ApplyForces(speed,steering,brake);
    }

    IEnumerator IncrementSpeed(){
        stopDriving = true;
        while(speed < 1){
            speed += 0.25f;
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator DecrementSpeed(){
        startDriving = true;
        while(speed > 0.1){
            speed -= 0.25f;
            if(speed < 0) // Prevent unintentional negative speed
                speed = 0;
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void StopNextStop(){
        // Update the screens so the bus will stop at next stop
        screens.ApplyStopTexture();

        // Set state to bus stopped

    }

    void UpdateWaitState(){
        if(stopDriving){
            stopDriving = false;
            brake = true;   
            StartCoroutine(DecrementSpeed());
        }
        if (!animationPlayed && doors != null)
            StartCoroutine(PlayOpenAnim());

        if (hasCheckedIn)
            StartCoroutine(CloseDoorsAndDrive());
    }

    IEnumerator PlayOpenAnim(){
        doors.OpenDoors();

        animationPlayed = true;

        yield return new WaitForSeconds(1);
    }

    IEnumerator CloseDoorsAndDrive(){
        animationPlayed = false;

        yield return new WaitForSeconds(2);
        
        doors.CloseDoors();

        yield return new WaitForSeconds(3);

        driveAllowed = true;

        busState = BusState.DRIVING;
    }

    void UpdateStopState(){
        if(firstTime)
            return;

        if(!busStopped)
            return;

        // Add logic to stop the bus
        
        if(!hasCheckedIn){
            busState = BusState.WAIT;
        }
    }

    
    public void StopBus(){
        busStopped = true;
        busState = BusState.STOP_BUTTON_PRESSED;
    }

    /* public void GetWPM(WaypointMover wpm, DoorController door){
        WPM = wpm;

        //doors = door;
    } */
}
