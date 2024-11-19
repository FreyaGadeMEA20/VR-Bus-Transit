using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using System;

public class BusController : MonoBehaviour
{
    // -- Outside Attributes --
    DoorController doors;
    BusScreenController screens;
    BusSeatAssigner seatAssigner{
        get{return SeatAssigner;}
        set{SeatAssigner = value;}
    }

    public BusSeatAssigner SeatAssigner;
    public VehicleMovement vehicleMovement;

    // What is the state of the bus
    // NOT BASED ON THE VEHICLE MOVEMENT, ONLY FOR WAITING AT STOPS
    public enum BusState {
        DRIVING,
        WAIT,
        STOP_BUTTON_PRESSED,
    }
    [SerializeField] BusState busState;

    // Are the doors open?
    bool doorsOpen = false;

    // Are we allowed to drive?
    private bool driveAllowed{
        get{return DriveAllowed;}
        set{DriveAllowed = value;}
    }
    public bool DriveAllowed;

    // Is the bus stop button pressed?
    private bool busStopped{
        get{return BusStopped;}
        set{BusStopped = value;}
    }
    public bool BusStopped;
    
    // Is this the first time the bus has stopped?
    public bool firstTime = true;

    // Has the player checked in?
    private bool hasCheckedIn{
        get{return HasCheckedIn;}
        set{HasCheckedIn = value;}
    }
    public bool HasCheckedIn;

    private bool correctBus{
        get{return CorrectBus;}
        set{CorrectBus = value;}
    }
    public bool CorrectBus;

    // Start is called before the first frame update
    void Awake() {
        // Gets and assigns all the relevant information
        busState = BusState.STOP_BUTTON_PRESSED;
        vehicleMovement = GetComponent<VehicleMovement>();
        doors = GetComponent<DoorController>();
        screens = GetComponentInChildren<BusScreenController>();
        seatAssigner = GetComponentInChildren<BusSeatAssigner>();
    }

    // Update is called once per frame. Run in VehicleMovement, so it runs alongside it
    public void UpdateBusController() {
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

    // drive state. Essentially a neutral state, as VehicleMovement is handling the driving
    private void UpdateDriveState() {
        return;
    }

    // runs the coroutine to open the doors, and lets that control it the waiting at bus stop
    void UpdateWaitState() {
        if (!doorsOpen && doors != null)
            StartCoroutine(BusStopAnimations());
    }

    // Coroutine to control how the bus behaves when it stops
    IEnumerator BusStopAnimations() {
        doors.OpenDoors(); // open the doors

        vehicleMovement.rb.isKinematic = true; // stop the bus from moving

        doorsOpen = true;  // set the doors to be open

        screens.ApplyNextTexture(); // change the bus screen texture
        
        if(busStopped && seatAssigner.PlayerSeated){
            seatAssigner.UnassignSeat(); // unassign the player from the seat
        }

        busStopped = false;

        // Wait for 15 seconds before continuing
        yield return new WaitForSeconds(15);

        // CAN BE CHANGED TO LOOK AT IF THE PLAYER HAS CHECKED IN --
        // If the player has entered the bus:
        if(seatAssigner.player != null) {
            // Continously check if the player has sat down, or they have exited the bus
            while(!seatAssigner.PlayerSeated || seatAssigner.player == null){
                yield return new WaitForSeconds(1);

                // TELL THE PLAYER TO SIT DOWN
            }
        }

        // Runs the coroutine to close the doors and drive the bus
        StartCoroutine(CloseDoorsAndDrive());
    }

    // Coroutine to close the doors and drive the bus
    IEnumerator CloseDoorsAndDrive() {
        doorsOpen = false; // set the doors to be closed

        // wait for 2 seconds before visually closing the doors
        yield return new WaitForSeconds(2); 
        doors.CloseDoors();
        vehicleMovement.rb.isKinematic = false; // allow the bus to move again
        
        // wait for 3 seconds before driving the bus
        yield return new WaitForSeconds(3);

        driveAllowed = true;

        // Tells the rest of the bus to drive
        busState = BusState.DRIVING;
        vehicleMovement.AdvanceToNextWaypoint();
    }

    void UpdateStopState() {
        // Stops the bus when the vehicle has reached a bus stop
        if(vehicleMovement.ReachedBusStop) {
            busState = BusState.WAIT;
        }
    }
    
    // Stops the bus. Gets run when stop button is pressed
    public void StopBus() {
        busStopped = true; // set the bus to be stopped
        Debug.Log("Bus stopped"); // log that the bus has stopped
        screens.ApplyStopTexture(); // change the bus screen texture to the stop texture
        busState = BusState.STOP_BUTTON_PRESSED; // set the bus state to be stop button pressed
    }

    public void WrongBus(){
        FadeToBlack.Instance.FadeOutAndLoadScene("WrongBus");
    }
}
