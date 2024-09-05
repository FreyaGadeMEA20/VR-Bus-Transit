using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
    // The different states the 
    public enum GameState
    {
        AT_SCHOOL,
        CHECKED_PHONE,
        LOOKED_AT_SIGN,
        BUS_STATE,
    }

    // Current state of the game
    [SerializeField] GameState currentState;

    // -- Objects for checking states -- //
    public GameObject phone;
    
    [SerializeField] BusController Bus;

    [SerializeField] ProxCheckerScript SignScript;
    [SerializeField] GameObject SignBeam;
    [SerializeField] ProxCheckerScript BusStopScript;
    [SerializeField] GameObject StopBeam;
    [SerializeField] CarSpawner carSpawer;
    [SerializeField] GameObject busDeath;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial state
        currentState = GameState.AT_SCHOOL;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for state transitions
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

        try {
            Bus = GameObject.Find("BusController").GetComponent<BusController>(); 
            if(Bus.HasCheckedIn)
            {
                WaypointMover wp = GameObject.FindWithTag("Bus").transform.parent.gameObject.GetComponent<WaypointMover>();
            }
            currentState = GameState.BUS_STATE;
        } catch (NullReferenceException e){
            Debug.LogWarning(e+"Bus has not spawned");
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
}
