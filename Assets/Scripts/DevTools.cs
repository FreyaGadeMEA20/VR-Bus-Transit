using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevTools : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Number of scenes in Build Settings: {SceneManager.sceneCountInBuildSettings}");
    }

    // Update is called once per frame
    void Update()
    {
        // Check for right arrow key to go to the next scene
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadNextScene();
        }

        // Check for left arrow key to go to the previous scene
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadPreviousScene();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings; // Loop back to the first scene
        Debug.Log($"Current Scene Index: {currentSceneIndex}, Next Scene Index: {nextSceneIndex}");
        SceneManager.LoadScene(nextSceneIndex);
    }

    void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = (currentSceneIndex - 1 + SceneManager.sceneCountInBuildSettings) % SceneManager.sceneCountInBuildSettings; // Loop back to the last scene
        Debug.Log($"Current Scene Index: {currentSceneIndex}, Previous Scene Index: {previousSceneIndex}");
        SceneManager.LoadScene(previousSceneIndex);
    }
}
