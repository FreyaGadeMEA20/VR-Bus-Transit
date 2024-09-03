using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusGameManager : MonoBehaviour
{
    /* The bus is set to have two different states
     *  - Bus Arriving is to keep track of the players actions before they reach the bus stop
     *      - Checking their phone, arriving at information signs, arriving at bus stop, checking in
     *  - Bus Driving is to keep track of the bus' movement after it has been told to begin driving
     *      - Checked in, stop button pressed, waiting on player
     */
    public enum BusState {
        BUS_ARRIVING,
        BUS_DRIVING
    }
    [SerializeField] BusState type; // reference to the state

    // The different states the 
    public enum GameState
    {
        AT_SCHOOL,
        CHECKED_PHONE,
        LOOKED_AT_SIGN,
        BUS_STATE,
        WAIT_FOR_PLAYER,
        STOP_BUTTON_PRESSED,
    }

    // Current state of the game
    [SerializeField] GameState currentState;

    // -- Objects for checking states -- //
    public GameObject phone;
    
    [SerializeField] ProxCheckerScript SignScript;
    [SerializeField] GameObject SignBeam;
    [SerializeField] ProxCheckerScript BusStopScript;
    [SerializeField] GameObject StopBeam;
    [SerializeField] CarSpawner carSpawer;
    [SerializeField] GameObject busDeath;

    WaypointMover WPM;

    DoorController doors;

    private bool hasCheckedIn{
        get{return HasCheckedIn;}
        set{HasCheckedIn = value;}
    }

    public bool HasCheckedIn;

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

    bool animationPlayed = false;
    public bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial state
        switch(type){
            case BusState.BUS_DRIVING:
                currentState = GameState.STOP_BUTTON_PRESSED;
                break;
            case BusState.BUS_ARRIVING:
                currentState = GameState.AT_SCHOOL;
                break;                
        }
        //StartCoroutine(fade());
    }

    // Update is called once per frame
    void Update()
    {
        // Check for state transitions
        switch(type){
            case BusState.BUS_DRIVING:
                switch (currentState){
                    case GameState.WAIT_FOR_PLAYER:
                        UpdateWaitState();
                        break;
                    case GameState.BUS_STATE:
                        driveAllowed = false;
                        busStopped = false;
                        break;
                    case GameState.STOP_BUTTON_PRESSED:
                        UpdateStopState();
                        break;
                }
                break;
            case BusState.BUS_ARRIVING:
                switch (currentState) {
                    case GameState.AT_SCHOOL:
                        // Handle SCHOOL state logic
                        UpdateSchoolState();
                        break;

                    case GameState.CHECKED_PHONE:
                        // Handle PHONE state logic
                        UpdatePhoneState();
                        break;

                    case GameState.LOOKED_AT_SIGN:
                        // Handle SIGN state logic
                        UpdateSignState();
                        break;

                    case GameState.BUS_STATE:
                        // Handle BUS state logic
                        UpdateBusState();
                        break;
                }
            break;
        }
    }

    // Countdown timer variables
    private float countdownTimer = 3f;
    private bool isCountingDown = true;
    void UpdateSchoolState() {
        // Check if the GameObject is visible in the viewport
        if (IsGameObjectVisibleInViewport(phone)) {
            // The GameObject is visible, do something

            if (isCountingDown) {
                countdownTimer -= Time.deltaTime;
                Debug.Log($"GameObject has been visible in the viewport for {(countdownTimer-3)*-1} seconds");

                if (countdownTimer <= 0f) {
                    // Countdown finished, do something
                    Debug.Log("Countdown finished");
                    currentState = GameState.CHECKED_PHONE;
                    SignBeam.SetActive(true);
                    // Reset the countdown timer
                    //countdownTimer = 3f;
                    isCountingDown = false;
                }
            }
            else {
                // Reset the countdown timer if the GameObject is not visible
                countdownTimer = 3f;
                isCountingDown = true;
            }
        }
    }

    void UpdatePhoneState(){
        if(SignScript.CheckPlayerProximity()){
            if (isCountingDown) {
                countdownTimer -= Time.deltaTime;
                Debug.Log($"Player has been by the sign for {(countdownTimer-3)*-1} seconds");

                if (countdownTimer <= 0f) {
                    // Countdown finished, do something
                    Debug.Log("Countdown finished");
                    currentState = GameState.LOOKED_AT_SIGN;
                    SignBeam.SetActive(false);
                    StopBeam.SetActive(true);
                    // Reset the countdown timer
                    //countdownTimer = 3f;
                    isCountingDown = false;
                }
            }
            else {
                // reset the countdown if it is not counting down
                countdownTimer = 3f;
                isCountingDown = true;
            }
        } else {
            // Reset the countdown timer if the GameObject is not visible
            countdownTimer = 3f;
            isCountingDown = true;
        }
    }

    void UpdateSignState(){
        if(BusStopScript.CheckPlayerProximity()){
            if (isCountingDown) {
                countdownTimer -= Time.deltaTime;
                Debug.Log($"Player has been by the bus stop for {(countdownTimer-3)*-1} seconds");

                if (countdownTimer <= 0f) {
                    // Countdown finished, do something
                    Debug.Log("Countdown finished");
                    currentState = GameState.BUS_STATE;

                    busDeath.SetActive(false);
                    // Reset the countdown timer
                    //countdownTimer = 3f;
                    isCountingDown = false;
                }
            }
            else {
                // reset the countdown if it is not counting down
                countdownTimer = 3f;
                isCountingDown = true;
            }
        } else {
            // Reset the countdown timer if the GameObject is not visible
            countdownTimer = 3f;
            isCountingDown = true;
        }

        // Set the car waypoint at the crosswalk to 'waiting' so the player can cross safely
    }
    
    void UpdateBusState(){
        if (!carSpawer.canSpawnBus)
            carSpawer.canSpawnBus = true;
            type = BusState.BUS_DRIVING;
            currentState = GameState.BUS_STATE;

        if(hasCheckedIn)
        {
            WaypointMover wp = GameObject.FindWithTag("Bus").transform.parent.gameObject.GetComponent<WaypointMover>();
        }

        // Let the cars move again from the sign as the player has crossed the street
    }

    bool IsGameObjectVisibleInViewport(GameObject gameObject)
    {
        if(!gameObject.activeSelf) {
            countdownTimer = 3f;
            return false;
        }

        // Get the main camera
        Camera mainCamera = Camera.main;

        // Calculate the viewport position of the GameObject
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(gameObject.transform.position);

        // Check if the GameObject is within the viewport bounds
        if (viewportPosition.x >= 0.2
            && viewportPosition.x <= .8
            && viewportPosition.y >= 0.2
            && viewportPosition.y <= .8
            && viewportPosition.z > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        currentState = GameState.BUS_STATE;
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
            currentState = GameState.WAIT_FOR_PLAYER;
        }
    }

    public void StopBus(){
        busStopped = true;
        currentState = GameState.STOP_BUTTON_PRESSED;
    }

    public void GetWPM(WaypointMover wpm, DoorController door){
        WPM = wpm;

        doors = door;
    }
}
