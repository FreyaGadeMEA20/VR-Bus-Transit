using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopScript : MonoBehaviour
{
    public BusScreenController busScreenController;
    [SerializeField] BusController busController;
    public bool Active = false;

    void Start()
    {
        busController = GetComponentInParent<BusController>();
    }

    public void StopButton()
    {
        if (Active)
        {
            StartCoroutine(DelayedStopButton());
        }
    }

    private IEnumerator DelayedStopButton()
    {
        yield return new WaitForSeconds(1f); // Wait for 3 seconds

        busController.StopBus();
        busController.StopButtonPressed = true; // Set the stop button to be pressed
        busScreenController.ApplyStopTexture();
    }
}
