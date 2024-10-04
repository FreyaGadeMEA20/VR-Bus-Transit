using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class TeleportController : MonoBehaviour
{
    [SerializeField] InputActionAsset actionAsset;
    [SerializeField] UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    [SerializeField] UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider provider;

    bool _isActive = false;

    bool _readyToTeleport = false;
    
    // Start...
    void Start()
    {
        rayInteractor.enabled = false;

        var activate = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Activate");   
        activate.Enable();
        activate.performed += OnTeleportActivate;

        var cancel = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Activate");   
        cancel.Enable();
        cancel.canceled += OnTeleportCancel;
    }

    void Update(){
        if(!_isActive)
            return;

        //if(_thumbStick.triggered)
        //    return;

        if(!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)){
            Debug.Log("YIPEE");
            rayInteractor.enabled=false;
            _isActive = false;
        }

        if(_readyToTeleport){
            UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest request  = new UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest(){
                destinationPosition = hit.point,
                //destinationRotation = ,
                //matchOrientation = ,
                //requestTime = ,
            };

            provider.QueueTeleportRequest(request);

            rayInteractor.enabled = false;
            _isActive = false;
        }
    }

    private void OnTeleportActivate(InputAction.CallbackContext context){
        Debug.Log("Hello");
        rayInteractor.enabled = true;
        _isActive = true;
        _readyToTeleport = false;
    }
    private void OnTeleportCancel(InputAction.CallbackContext context){
        Debug.Log("ByeBye");
        _readyToTeleport=true;
    }
}