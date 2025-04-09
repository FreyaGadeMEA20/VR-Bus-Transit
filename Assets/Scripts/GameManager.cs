using System;
using System.Collections;
using System.Collections.Generic;
using Movement;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the logic for the different stages of the VR-Bus-Transit project/// </summary>
public class GameManager : MonoBehaviour
{
    /* The bus is set to have two different states
     *  - Bus Arriving is to keep track of the players actions before they reach the bus stop
     *      - Checking their phone, arriving at information signs, arriving at bus stop, checking in
     *  - Bus Driving is to keep track of the bus' movement after it has been told to begin driving
     *      - Checked in, stop button pressed, waiting on player
     */
    // The different states of the experience
    public enum GameState
    {
        START,
        CHECKED_PHONE,
        REACHED_BUS_STOP,
        CHECKED_IN,
        SAT_DOWN,
        PRESSED_STOP_BUTTON,
        CHECKED_OUT,
    }

    // Current state of the game
    [SerializeField] GameState currentState;

    // -- Objects for checking states -- //
    public GameObject phone; //phone.
    bool lookedAtPhone = false; // control variable for task 1
    [HideInInspector] public GameObject busStopSign; // Tracks which bus stop the player should go to
    bool lookedAtSign = false; // control variable for task 2
    
    [Separator("Information regarding the bus")]
    public BusController BusToGetOn; // the bus the player needs to get on
    public BusLineSO BusLine; // the bus line the player need to take
    public Waypoint _finalDestination; // where the player needs to get off
    [SerializeField] BusController[] Buses; // list of all the buses
    [SerializeField] BusController busAtStop; // the bus at the bus stop the player should reside at

    [Separator("Information regarding the button input")]
    public InputActionProperty handSwitch; // the input action for the hand switch. Should be changed to one that tracks both buttons on controller
    public bool buttonPressed = false; // control variable for phone updating

    // Countdown timer variables
    [Separator("Countdown Timer")]
    private float countdownTimer = 0f; // the countdown timer to control the visual timer
    public float Timer = 8f; // timer the countdown should reach
    public event OnVariableChangeDelegate OnVariableChange; // event manager everything will listen to, to check what the timer is at
    public delegate void OnVariableChangeDelegate(float newVal); // together with above. It is used for timer on phone

    private bool isCountingDown = false; // control variable for the countdown timer
    public bool inCorrectStopZone = false; // control variable for the bus stop zone for task 3

    public bool CanBusDrive = false; // control variable for the bus driving

    public static GameManager Instance { get; internal set; } // Singleton instance. Everything calls for this
    public event Action<GameState> OnStateChange; // event manager for the state change, used for other scriptsd to know when task is completed

    [Separator("Recentering")]
    [SerializeField] Transform target; // the target to recenter the player to. To mitigate offset at start
    public Transform head, origin; // player.
    
    Coroutine updateTimeCoroutine; // keeps track if a coroutine is running for the update of time

    bool rotateSkybox = false; // control variable for rotating the skybox

    // makes sure it is singleton
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial state
        currentState = GameState.START;

        // Set the initial bus to get on
        if(Buses.Length > 0)
            BusToGetOn = Buses[UnityEngine.Random.Range(0, Buses.Length)];

        // determines final destination and bus line
        BusLine = BusToGetOn.vehicleMovement._RouteManager.busLine;
        _finalDestination = BusToGetOn.vehicleMovement._RouteManager.ChooseRandomBusStop();

        Recenter(); //makes sure the player is where they should be

        Timer = 8f; // Sets the initial timer used for the first state

        RenderSettings.skybox.SetFloat("_Rotation", 0f); // Sets the skybox to 0 at the start
    }

    // Recenters the player and camera offset at the target location
    // Done to prevent offset at the start of the experience, from moving around the headset
    public void Recenter()
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;
        origin.position = target.position + offset;
        
        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);
        
        origin.RotateAround(head.position, Vector3.up, angle);
    }


    // Update is called once per frame
    void Update()
    {
        // Checks if the DataGatherer exists, in order to write data to a csv file
        if(DataGatherer.Instance != null){
            DataGatherer.Instance.WriteToCSV(
                Time.time.ToString("F2"), 
                transform.position.ToString(), 
                currentState.ToString(), 
                _finalDestination.name, 
                BusToGetOn.name
            );
        }

        // Update the time every minute
        if(Mathf.RoundToInt(Time.time % 60) == 0){
            if(updateTimeCoroutine != null){
                StopCoroutine(updateTimeCoroutine);
            }

            updateTimeCoroutine = StartCoroutine(UpdateTime());
        }

        // Rotate the skybox
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.15f);
        

        // Check for state transitions
        switch (currentState) {
            case GameState.START:
                // Handle START state logic
                UpdateSchoolState();
                break;

            case GameState.CHECKED_PHONE:
                // Handle PHONE state logic
                UpdatePhoneState();
                break;

            case GameState.REACHED_BUS_STOP:
                // Handle BUS_STOP state logic
                UpdateBusStopState();
                break;

            case GameState.CHECKED_IN:
                // Handle CHECK_IN state logic
                UpdateCheckInState();
                break;

            case GameState.SAT_DOWN:
                // Handle SIT_DOWN state logic
                UpdateSitDownState();
                break;

            case GameState.PRESSED_STOP_BUTTON:
                // Handle STOP_BUTTON state logic
                UpdateStopButtonState();
                break;

            case GameState.CHECKED_OUT:
                // Handle CHECK_OUT state logic
                UpdateCheckOutState();
                break;
        }
    }

    IEnumerator UpdateTime(){
        yield return new WaitForSeconds(1); //time is fictional, so it just waits an extra second
        
        // only updates the time if the bus is driving.
        // this needs to be looked at in the future, because I think it updates based on GLOBAL time, so it gets into -3 mins easily
        if(CanBusDrive){
            foreach(var busStop in FindObjectsOfType<BusStop>()){
                busStop.UpdateTime(true);
            }
        }

        // updates the time on the phone
        PhoneTime.Instance.UpdateTime();
    }

    // In order to let the other scripts know of the state changing, an event is made to make it easier to call
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnStateChange?.Invoke(currentState);
    }

    // First state just makes them check their phone and confirm they have understood the instructions
    void UpdateSchoolState() {
        if(lookedAtPhone){
            OnVariableChange(countdownTimer+=Time.deltaTime);

            // once countdown is finished, change state and reset imer
            if (countdownTimer >= Timer) {
                countdownTimer = 0f;
                Timer = 3f;
                isCountingDown = false;
                ChangeState(GameState.CHECKED_PHONE);
            }
        }
        
        

        // Check if the GameObject is visible in the viewport...
        if (IsGameObjectVisibleInViewport(phone) && !lookedAtPhone) {
            // ...and if the player presses the select button, change the state
            if(handSwitch.action.ReadValue<float>() > 0.8f){
                //ChangeState(GameState.CHECKED_PHONE);
                lookedAtPhone = true;
            }
        }
    }

    void UpdatePhoneState(){
        if(RejsekortInformation.Instance.GetCheckedIn() && !inCorrectStopZone){
            Debug.Log("Player has checked in at the wrong stop");
            // do something

            StartCoroutine(FadeToBlack.Instance.FadeOutAndLoadScene(1));
        }
        // Check if the player is near the designated bus stop
        if(!inCorrectStopZone){
            Debug.Log("Player is not in the correct stop zone");
            return;
        }

        /*if(IsGameObjectVisibleInViewport(busStopSign) && !lookedAtSign){
            lookedAtSign = true;
        }

        if(!lookedAtSign){
            return;
        }*/

        // Apply information to phone

        // Counts down to see how long the player has been by the bus stop
        if (isCountingDown) {
            OnVariableChange(countdownTimer+=Time.deltaTime);

            // once countdown is finished, change state and reset imer
            if (countdownTimer >= 4f) {
                countdownTimer = 0f;
                isCountingDown = false;
                CanBusDrive = true;
                ChangeState(GameState.REACHED_BUS_STOP);
            }
        } else {
            // reset the countdown if it is not counting down
            countdownTimer = 0f;
            isCountingDown = true;
        }
        
    }

    void UpdateBusStopState(){
        // add a check to see if the bus has reached the bus stop
        if(busAtStop != null){
            Debug.Log("Bus has reached the bus stop");
            if(busAtStop._BusState == BusController.BusState.DRIVING && !RejsekortInformation.Instance.GetCheckedIn()){
                StartCoroutine(FadeToBlack.Instance.FadeOutAndLoadScene(2)); // can be made nicer
                Debug.Log("Bus has left the bus stop");
            }
        } else {
            return;
        }

        if(!RejsekortInformation.Instance.GetCheckedIn())
            return;
        
        // add a check to see if it is the correct bus that has been checked in at
        if(BusToGetOn.Equals(RejsekortInformation.Instance.GetBus())){//BusToGetOn.vehicleMovement._RouteManager.busLine.Equals(BusLine)){
            ChangeState(GameState.CHECKED_IN);
            //busAtStop.doors.CloseDoors();
        } else {
            Debug.LogWarning("Wrong bus");
            StartCoroutine(FadeToBlack.Instance.FadeOutAndLoadScene(3));
        }
        
    }

    void UpdateCheckInState(){
        if(BusToGetOn.SeatAssigner.PlayerSeated){
            ChangeState(GameState.SAT_DOWN);
        }
    }

    void UpdateSitDownState(){
        if(!BusToGetOn.SeatAssigner.PlayerSeated){
            // More logic needs to be added, this is just a placeholder
            ChangeState(GameState.PRESSED_STOP_BUTTON);
        }
    }

    void UpdateStopButtonState(){
        if(RejsekortInformation.Instance.GetCheckedIn())
            return;
        
        if(!BusToGetOn.vehicleMovement._RouteManager.currentWaypoint.Equals(_finalDestination)){
            Debug.LogWarning("Wrong stop");
            return;
        }

        Debug.Log("WOWZERS");
        ChangeState(GameState.CHECKED_OUT);
    }

    void UpdateCheckOutState(){
        Debug.Log("Finished");

        // Check how well the player did (if needed)
        //  - Off at correct bus stop?
        //  - Time spent?
    }

    // Check if the GameObject is visible in the viewport
    bool IsGameObjectVisibleInViewport(GameObject gameObject)
    {   
        // Check if the GameObject is active
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

    // to check if the bus has reached the bus stop, we apply the buscontroller that has reached the bus stop
    public void ApplyBusController(BusController applyingBus){
        busAtStop = applyingBus;
    }
}
