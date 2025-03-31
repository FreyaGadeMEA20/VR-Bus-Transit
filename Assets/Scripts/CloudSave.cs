using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

public class CloudSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetupAndSignIn();
        Debug.Log("CloudSave started. Setting up and signing in.");
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    async void SetupAndSignIn()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
    }

    public async void SaveData()
    {

    }

    public async void LoadData()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method is called whenever a new scene is loaded
        Debug.Log($"Scene loaded: {scene.name}");
        // You can add any additional logic here if needed
        SaveData();

        LoadData();

    }

    void OnApplicationQuit()
    {
        // Save any final data before quitting
        SaveData();
        Debug.Log("Application is quitting, final data saved.");
        LoadData();
        Debug.Log("Last played data loaded: " + System.DateTime.Now.ToString());

        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
}
}
