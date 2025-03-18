using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using TMPro;
public class RelayConnectHost : MonoBehaviour
{
    /// <summary>
    /// The textbox displaying the status of the server.
    /// </summary>
    public TextMeshProUGUI ServerStatus;

    /// <summary>
    /// The textbox displaying the join code.
    /// </summary>
    public TextMeshProUGUI JoinCodeText;

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
        
        JoinCodeText.text = joinCode;
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

        OnRegion();
    }

    /// <summary>
    /// Event handler for when the Get Regions button is clicked.
    /// </summary>
    public async void OnRegion()
    {
        Debug.Log("Host - Getting regions.");
        var allRegions = await RelayService.Instance.ListRegionsAsync();
        regions.Clear();
        regionOptions.Clear();
        foreach (var region in allRegions)
        {
            Debug.Log(region.Id + ": " + region.Description);
            regionOptions.Add(region.Id);
            regions.Add(region);
        }
        UpdateUI();
        OnAllocate();
    }

    /// <summary>
    /// Event handler for when the Allocate button is clicked.
    /// </summary>
    public async void OnAllocate()
    {
        Debug.Log("Host - Creating an allocation.");

        // Determine region to use (user-selected or auto-select/QoS)
        string region = GetRegionOrQosDefault();
        Debug.Log($"The chosen region is: {region}");

        // Important: Once the allocation is created, you have ten seconds to BIND
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4, region);
        hostAllocationId = allocation.AllocationId;
        allocationRegion = allocation.Region;

        Debug.Log($"Host Allocation ID: {hostAllocationId}, region: {allocationRegion}");
        textDisplay = $"Successfully created the server at region: {allocationRegion}.";

        UpdateUI();
        OnJoinCode();
    }

    // was too lazy to remove this, and only wanted them to connect to the closest server.
    // if this does not work for, check SimpleRelay.cs from Unity Relay Sample.
    string GetRegionOrQosDefault()
    {
        return null;
    }

    /// <summary>
    /// Event handler for when the Get Join Code button is clicked.
    /// </summary>
    public async void OnJoinCode()
    {
        Debug.Log("Host - Getting a join code for my allocation. I would share that join code with the other players so they can join my session.");

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocationId);
            Debug.Log("Host - Got join code: " + joinCode);
            textDisplay = $"Host - Successfully established the server with join code: {joinCode}.";
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            error = true;
        }

        UpdateUI();
        if(!error) OnJoin();
    }

    /// <summary>
    /// Event handler for when the Join button is clicked.
    /// </summary>
    public async void OnJoin()
    {
        Debug.Log("Player - Joining host allocation using join code.");

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
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
