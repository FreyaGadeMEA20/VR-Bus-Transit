using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

public class BusController : MonoBehaviour
{
    DoorController doors;
    BusScreenController screens;

    WaypointMover WPM;
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

    // Start is called before the first frame update
    void Start()
    {
        busState = BusState.STOP_BUTTON_PRESSED;
        vehicleMovement = GetComponent<VehicleMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vehicleMovement.ApplyForces(1,1,false);
    }

    public void StopNextStop(){
        // Update the screens so the bus will stop at next stop
        screens.ApplyStopTexture();

        // Set state to bus stopped

    }

    void UpdateWaitState(){
        if (!animationPlayed && doors != null)
            StartCoroutine(PlayOpenAnim());

        if(hasCheckedIn)
            StartCoroutine(Drive());
    }

    IEnumerator PlayOpenAnim(){
        doors.OpenDoors();

        animationPlayed = true;

        yield return new WaitForSeconds(1);
    }

    IEnumerator Drive(){
        animationPlayed = false;

        yield return new WaitForSeconds(2);
        
        doors.CloseDoors();

        yield return new WaitForSeconds(3);

        driveAllowed = true;

        WPM.hasCheckedIn = true;
    }

    void UpdateStopState(){
        if(firstTime)
            return;

        if(!busStopped)
            return;
        
        if(!hasCheckedIn){
            busState = BusState.WAIT;
        }
    }

    public void StopBus(){
        busStopped = true;
        busState = BusState.STOP_BUTTON_PRESSED;
    }

    public void GetWPM(WaypointMover wpm, DoorController door){
        WPM = wpm;

        doors = door;
    }
}
