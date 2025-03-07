using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using System;

// BusController is the main controller for the bus.
// It controls everything related to the bus, except for movement
// Screens, doors, everything related to the gamemanager and the bus
public class BusController : MonoBehaviour
{
    // -- Outside Attributes --
    [Header("Other Controllers")]
    public DoorController doors;
    public BusScreenController screens;
    BusSeatAssigner seatAssigner{
        get{return SeatAssigner;}
        set{SeatAssigner = value;}
    }

    public BusSeatAssigner SeatAssigner;
    public VehicleMovement vehicleMovement;

    public enum BusState {
        DRIVING,
        WAIT,
        STOP_BUTTON_PRESSED,
    }
    // What is the state of the bus
    // NOT BASED ON THE VEHICLE MOVEMENT, ONLY FOR WAITING AT STOPS
    [Header("Bus State")]
    BusState busState;
    public BusState _BusState{
        get{return busState;}
        set{busState = value;}
    }
    
    [Header("Bus Information")]
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

    // Is this the bus the player is supposed to take?
    private bool correctBus{
        get{return CorrectBus;}
        set{CorrectBus = value;}
    }
    public bool CorrectBus;

    // Has the stop button been pressed?
    bool stopButtonPressed = false;
    public bool StopButtonPressed{
        get{return stopButtonPressed;}
        set{stopButtonPressed = value;}
    }


    // Start is called before the first frame update
    void Awake() {
        // Gets and assigns all the relevant information
        busState = BusState.STOP_BUTTON_PRESSED; // tells the bus to stop at the next bus stop

        // Gets the vehicle movement, doors, screens and seat assigner
        vehicleMovement = GetComponent<VehicleMovement>();
        doors = GetComponent<DoorController>();
        screens = GetComponentInChildren<BusScreenController>();
        seatAssigner = GetComponentInChildren<BusSeatAssigner>();

        // Applies the first texture to the bus screen
        screens.GiveInformation();
        screens.ApplyNextTexture();
    }

    // Update is called once per frame. Run in VehicleMovement, so it runs alongside it, and lessens amount of update calls
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
        if (!doorsOpen && doors != null){
            StartCoroutine(BusStopAnimations());
            
            foreach(var stop in vehicleMovement._RouteManager.busStops){
                stop.busStop.busStop.BusPassedStop(vehicleMovement._RouteManager.busLine);
                stop.busStop.busStop.UpdateTime();
            }
            
            // For baking time
            //Debug.Log($"I ran {index} times");
            //vehicleMovement._RouteManager.busLine.BakeTime(index);
            //index++;
        }
    }
    int index = 0;
    // Coroutine to control how the bus behaves when it stops
    IEnumerator BusStopAnimations() {
        doors.OpenDoors(); // open the doors

        vehicleMovement.rb.isKinematic = true; // stop the bus from moving

        doorsOpen = true;  // set the doors to be open

        
        if(busStopped && seatAssigner.PlayerSeated && stopButtonPressed){
            seatAssigner.UnassignSeat(); // unassign the player from the seat
            stopButtonPressed = false; // set the stop button to be unpressed
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

        // wait for 2 seconds before visually closing the doors
        yield return new WaitForSeconds(2); 
        if(!seatAssigner.player)
            doors.CloseDoors();

        screens.ApplyNextTexture(); // change the bus screen texture

        vehicleMovement.rb.isKinematic = false; // allow the bus to move again
        
        // wait for 3 seconds before driving the bus
        yield return new WaitForSeconds(3);

        driveAllowed = true;

        // Tells the rest of the bus to drive
        busState = BusState.DRIVING;
        doorsOpen = false; // set the doors to be closed
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
        Debug.Log("Stop button pressed"); // log that the bus has stopped
        screens.ApplyStopTexture(); // change the bus screen texture to the stop texture
        busState = BusState.STOP_BUTTON_PRESSED; // set the bus state to be stop button pressed
    }
}
