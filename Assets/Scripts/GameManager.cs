using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] BusController Bus;

    public InputActionProperty handSwitch;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial state
        currentState = GameState.START;

        // possibly coinflip to determine which station to start at
    }

    // Update is called once per frame
    void Update()
    {
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

    // Countdown timer variables
    private float countdownTimer = 3f;
    private bool isCountingDown = true;
    void UpdateSchoolState() {
        

        // Check if the GameObject is visible in the viewport
        /* if (IsGameObjectVisibleInViewport(phone)) {
            // The GameObject is visible, do something

            if (isCountingDown) {
                countdownTimer -= Time.deltaTime;
                Debug.Log($"GameObject has been visible in the viewport for {(countdownTimer-3)*-1} seconds");

                if (countdownTimer <= 0f) {
                    // Countdown finished, do something
                    Debug.Log("Countdown finished");
                    currentState = GameState.CHECKED_PHONE;
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
        } */
    }

    void UpdatePhoneState(){
        /* if(SignScript.CheckPlayerProximity()){
            if (isCountingDown) {
                countdownTimer -= Time.deltaTime;
                Debug.Log($"Player has been by the sign for {(countdownTimer-3)*-1} seconds");

                if (countdownTimer <= 0f) {
                    
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
        } */
    }

    void UpdateBusStopState(){

    }

    void UpdateCheckInState(){

    }

    void UpdateSitDownState(){

    }

    void UpdateStopButtonState(){

    }

    void UpdateCheckOutState(){

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
