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

    bool doorsOpen = false;

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

    // Start is called before the first frame update
    void Start() {
        busState = BusState.STOP_BUTTON_PRESSED;
        vehicleMovement = GetComponent<VehicleMovement>();
        doors = GetComponent<DoorController>();
        screens = GetComponent<BusScreenController>();
        
    }

    void Update() {
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

    private void UpdateDriveState() {
        return;
    }

    public void StopNextStop() {
        // Update the screens so the bus will stop at next stop
        screens.ApplyStopTexture();

        // Set state to bus stopped

    }

    void UpdateWaitState() {
        if (!doorsOpen && doors != null)
            StartCoroutine(BusStopAnimations());
    }

    IEnumerator BusStopAnimations() {
        doors.OpenDoors();

        doorsOpen = true;

        yield return new WaitForSeconds(15);

        // add a check to see if player is within a certain distance, if so, wait another 6 seconds or smt

        StartCoroutine(CloseDoorsAndDrive());
    }

    IEnumerator CloseDoorsAndDrive() {
        doorsOpen = false;

        yield return new WaitForSeconds(2);
        
        doors.CloseDoors();

        yield return new WaitForSeconds(3);

        driveAllowed = true;

        busState = BusState.DRIVING;
        busStopped = true;
        vehicleMovement.AdvanceToNextWaypoint();
    }

    void UpdateStopState() {
        if(!busStopped)
            return;
        // Add logic to stop the bus
        
        if(vehicleMovement.ReachedBusStop) {
            busState = BusState.WAIT;
        }
    }
    
    public void StopBus() {
        busStopped = true;
        Debug.Log("Bus stopped");
        busState = BusState.STOP_BUTTON_PRESSED;
    }
}
