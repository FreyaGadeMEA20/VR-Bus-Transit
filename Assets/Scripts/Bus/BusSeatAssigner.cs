using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BusSeatAssigner : MonoBehaviour
{
    public GameObject player;
    Seat currentSeat;
    [SerializeField] Transform head;
    [SerializeField] Transform origin;
    Camera mainCamera;
    
    [SerializeField] LayerMask seatedLayer;
    public bool PlayerSeated => currentSeat != null;

    XROrigin xrOrigin;
    List<StopScript> stops;

    float seatCooldown;
    void Start() {
        mainCamera = Camera.main;
        stops = new List<StopScript>(FindObjectsOfType<StopScript>());
        head = GameManager.Instance.head;
        origin = GameManager.Instance.origin;
    }
    
    public void GetPlayer(GameObject _player){
        player = _player;
        
        xrOrigin = player.GetComponentInChildren<XROrigin>();
    }
    
    // Assigns the player to the given seat
    public void AssignSeat(Seat seat)
    {
        if(PlayerSeated){
            return;
        }

        if(seatCooldown > 0){
            return;
        }

        currentSeat = seat;
        
        StartCoroutine(SitOnSeat());
    }

    IEnumerator SitOnSeat(){
        DisableSeatSigns();
        StartCoroutine(FadeToBlack.Instance.FadeOut());
        yield return new WaitForSeconds(FadeToBlack.Instance.fadeDuration);
        //Debug.Log("Assigning seat");
        //TODO: Tune position and rotation
        // Seat the player - ROTATION OF THE SEAT IS IMPORTANT
        
        MoveToSeat(currentSeat.seatingArea);
        //xrOrigin.gameObject.GetComponent<DynamicMoveProvider>().useGravity = false;
        
        // Disable the player's movement
        player.layer = LayerMask.NameToLayer("SeatedPlayer");
        xrOrigin.gameObject.layer = LayerMask.NameToLayer("SeatedPlayer");
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 0;
        
        player.GetComponent<DynamicMoveProvider>().useGravity = false;
        
        StartCoroutine(FadeToBlack.Instance.FadeIn());

        yield return new WaitForSeconds(4);
        
        foreach (var stop in stops)
        {
            stop.Active = true;
        }
        // Enable "get off" button
        //getOffButton = seat.GetComponent<Seat>().EnableGetOffButton();
    }


    // Unassigns the player from the given seat
    public void UnassignSeat()
    {
        StartCoroutine(GetOffSeat());
        StartCoroutine(Cooldown());
    }

    IEnumerator GetOffSeat(){
        foreach (var stop in stops)
        {
            stop.Active = false;
        }
        StartCoroutine(FadeToBlack.Instance.FadeOut());
        yield return new WaitForSeconds(FadeToBlack.Instance.fadeDuration);
        // Move the player to the closest bus exit - ROTATION OF THE AREA IS IMPORTANT
        
        MoveOffSeat(currentSeat.exitArea);
        //Recenter(currentSeat.exitArea);
        //xrOrigin.gameObject.GetComponent<DynamicMoveProvider>().useGravity = true;

        player.layer = LayerMask.NameToLayer("Default");
        xrOrigin.gameObject.layer = LayerMask.NameToLayer("Default");
        // Enable the player's movement

        // Disable "get off" button
        //currentSeat.GetComponent<Seat>().DisableGetOffButton();

        currentSeat = null;
        StartCoroutine(FadeToBlack.Instance.FadeIn());
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 3;
        player.GetComponent<DynamicMoveProvider>().useGravity = true;
        
        yield return new WaitForSeconds(3);

        EnableSeatSigns();
    }

    // TODO: Teleport anchor to the target
    public void MoveToSeat(GameObject target){
        // Get the target rotation
        Quaternion targetRotation = target.transform.rotation;

        // Create a TeleportRequest with the target position and rotation
        TeleportRequest teleportRequest = new TeleportRequest()
        {
            destinationPosition = target.transform.position,
            destinationRotation = targetRotation
        };

        //Quaternion cameraRotation = Quaternion.Euler(0f, mainCamera.transform.localEulerAngles.y, 0f);
        TeleportationProvider teleportationProvider = player.GetComponent<TeleportationProvider>();
        teleportationProvider.QueueTeleportRequest(teleportRequest);

        // Get the current camera rotation
        Quaternion cameraRotation = Quaternion.Euler(0f, mainCamera.transform.localEulerAngles.y, 0f);

        // Rotate the XR Rig's XROrigin to face the correct direction after teleportation, considering camera rotation
        player.transform.rotation = targetRotation;// * Quaternion.Inverse(cameraRotation);
    }

    public void MoveOffSeat(GameObject target){
        player.transform.position = target.transform.position;
        Debug.Log(target.transform.position);

        xrOrigin.MoveCameraToWorldLocation(target.transform.position);
        xrOrigin.MatchOriginUpCameraForward(target.transform.up, target.transform.forward);

        Quaternion newRot;

        if(target.transform.rotation.w < 0){
            newRot = new Quaternion(0, target.transform.rotation.w, 0, 0);
        }else{
            newRot = new Quaternion(0, 0, 0, 0);
        }
        player.transform.rotation = newRot;
    }

    public void Recenter(GameObject target)
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;
        origin.position = target.transform.position + offset;
        
        Vector3 targetForward = target.transform.forward;
        targetForward.y = 0;
        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);
        
        origin.RotateAround(head.position, Vector3.up, angle);
    }

    IEnumerator Cooldown(){
        seatCooldown = 3;
        yield return new WaitForSeconds(seatCooldown);
        seatCooldown = 0;
    }

    void DisableSeatSigns(){
        foreach (var seat in FindObjectsOfType<Seat>())
        {
            seat.DisableScreen();
        }
    }

    void EnableSeatSigns(){
        foreach (var seat in FindObjectsOfType<Seat>())
        {
            seat.Enable();
        }
    }
}
