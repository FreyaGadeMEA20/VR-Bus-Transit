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
    public GameObject phone;
    bool lookedAtPhone = false;
    [HideInInspector] public GameObject busStopSign;
    [SerializeField] bool lookedAtSign = false;
    [Separator("Information regarding the bus")]
    public BusLineSO BusLine;
    public Waypoint _finalDestination;
    [SerializeField] BusController[] Buses;
    public BusController BusToGetOn;

    [Separator("Information regarding the button input")]
    public InputActionProperty handSwitch;
    public bool buttonPressed = false;

    // Countdown timer variables
    [Separator("Countdown Timer")]
    private float countdownTimer = 0f;
    public event OnVariableChangeDelegate OnVariableChange;
    public delegate void OnVariableChangeDelegate(float newVal);
    public float CoutndownTimer
    {
        get
        {
            return countdownTimer;
        }
        set
        {
            if (countdownTimer == value) return;
            countdownTimer = value;
            if (OnVariableChange != null)
                OnVariableChange(countdownTimer);
        }
    }
    private bool isCountingDown = false;
    public bool inCorrectStopZone = false;

    public static GameManager Instance { get; internal set; }
    public event Action<GameState> OnStateChange;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        BusLine = BusToGetOn.vehicleMovement._RouteManager.busLine;
        _finalDestination = BusToGetOn.vehicleMovement._RouteManager.ChooseRandomWaypoint();

        // Set the initial final destination
    }

    Coroutine updateTimeCoroutine;

    bool rotateSkybox = false;

    // Update is called once per frame
    void Update()
    {
        if(Mathf.RoundToInt(Time.time % 60) == 0){
            if(updateTimeCoroutine != null){
                StopCoroutine(updateTimeCoroutine);
            }

            updateTimeCoroutine = StartCoroutine(UpdateTime());
        }
        if(rotateSkybox){
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.4f);
        }

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
        yield return new WaitForSeconds(1);

        rotateSkybox = true;

        foreach(var busStop in FindObjectsOfType<BusStop>()){
            busStop.UpdateTime();
        }
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
//            countdownTimer += Time.deltaTime;
            //Debug.Log($"Player has been by the sign for {(countdownTimer):#.0} seconds");
            
            // for phone visuals vvv
            //Mathf.Lerp(0,100, countdownTimer);

            // once countdown is finished, change state and reset imer
            if (countdownTimer >= 4f) {
                countdownTimer = 0f;
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

        if(IsGameObjectVisibleInViewport(busStopSign) && !lookedAtSign){
            lookedAtSign = true;
        }

        if(!lookedAtSign){
            return;
        }

        // Apply information to phone

        // Counts down to see how long the player has been by the bus stop
        if (isCountingDown) {
            OnVariableChange(countdownTimer+=Time.deltaTime);
            Debug.Log($"Player has been by the sign for {(countdownTimer-3)*-1:#.0} seconds");
            
            // for phone visuals vvv
            //Mathf.Lerp(0,100, countdownTimer);

            // once countdown is finished, change state and reset imer
            if (countdownTimer >= 4f) {
                countdownTimer = 0f;
                isCountingDown = false;
                ChangeState(GameState.REACHED_BUS_STOP);
            }
        } else {
            // reset the countdown if it is not counting down
            countdownTimer = 0f;
            isCountingDown = true;
        }
        
    }

    void UpdateBusStopState(){
        if(!RejsekortInformation.Instance.GetCheckedIn())
            return;
        
        // add a check to see if it is the correct bus that has been checked in at
        if(BusToGetOn.Equals(RejsekortInformation.Instance.GetBus())){//BusToGetOn.vehicleMovement._RouteManager.busLine.Equals(BusLine)){
            ChangeState(GameState.CHECKED_IN);
        } else {
            Debug.LogWarning("Wrong bus");
            StartCoroutine(FadeToBlack.Instance.FadeOutAndLoadScene(1));
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
            Debug.Log("Wrong stop");
            return;
        }

        Debug.LogWarning("WOWZERS");
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
}
