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
        Active = true;
        StartCoroutine(DelayedStopButton());
    }

    private IEnumerator DelayedStopButton()
    {
        yield return new WaitForSeconds(1f);
        if(Active){
            busController.StopBus();
            busController.StopButtonPressed = true; // Set the stop button to be pressed
            busScreenController.ApplyStopTexture();
        }
    }

    public void LeaveCollider(){
        Active = false;
    }
}
