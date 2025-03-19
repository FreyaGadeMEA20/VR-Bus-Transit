using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using TMPro;
public class RelayConnectJoin : MonoBehaviour
{
    /// <summary>
    /// The textbox displaying the status of the server.
    /// </summary>
    public TextMeshProUGUI ServerStatus;

    /// <summary>
    /// The textbox displaying the join code.
    /// </summary>
    public TMP_InputField JoinCodeText;

    /// <summary>
    /// The button that allows them to continue on.
    /// </summary>
    public Button StartButton;

    Guid hostAllocationId;
    Guid playerAllocationId;
    string allocationRegion = "";
    string joinCode = "n/a";
    string playerId = "Not signed in";
    string textDisplay = "Not signed in";
    List<Region> regions = new List<Region>();
    List<string> regionOptions = new List<string>();

    bool StartAllowed = false;
    bool error = false;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Services Initialized.");

        UpdateUI();
    }

    void UpdateUI()
    {
        ServerStatus.text = textDisplay;
    }

    void UpdateWithError(){
        ServerStatus.text = "Error. Try again or check internet connection.";
    }

    /// <summary>
    /// Making one function to handle all the button clicks.
    /// </summary>
    public void ConnectToServer() {
        Debug.Log("Connecting to server.");
        OnSignIn();
    }


    /// <summary>
    /// Event handler for when the Sign In button is clicked.
    /// </summary>
    public async void OnSignIn()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerId = AuthenticationService.Instance.PlayerId;

        Debug.Log($"Signed in. Player ID: {playerId}");
        textDisplay = $"Signed in. Player ID: {playerId}";
        UpdateUI();
        
        joinCode = JoinCodeText.text;

        if(!error) OnJoin(joinCode);
    }


    /// <summary>
    /// Event handler for when the Join button is clicked.
    /// </summary>
    public async void OnJoin(string _joinCode)
    {
        Debug.Log("Player - Joining host allocation using join code.");

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(_joinCode);
            playerAllocationId = joinAllocation.AllocationId;
            Debug.Log("Player Allocation ID: " + playerAllocationId);
            textDisplay = "Successfully joined the server.";
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            error = true;
        }

        UpdateUI();
        if(!error) StartAllowed = true;

        if(error) UpdateWithError();
        else UpdateUI();
    }
}
