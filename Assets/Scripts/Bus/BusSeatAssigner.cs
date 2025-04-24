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
    }
    
    public void GetPlayer(GameObject _player){
        player = _player;
        Debug.Log(player);

        head = GameManager.Instance.head;
        origin = GameManager.Instance.origin;
        
        if(player != null) xrOrigin = player.GetComponentInChildren<XROrigin>();
    }
    
    // Assigns the player to the given seat
    public void AssignSeat(Seat seat)
    {
        if(!RejsekortInformation.Instance.GetCheckedIn()){
            return;
        }
        //add check for if player has checked in
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
        /*foreach (var hands in GameObject.FindGameObjectsWithTag("Hands"))
        {
            hands.gameObject.layer = LayerMask.NameToLayer("SeatedPlayer");
        }*/
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 0;
        
        player.GetComponent<DynamicMoveProvider>().useGravity = false;
        
        StartCoroutine(FadeToBlack.Instance.FadeIn());

        yield return new WaitForSeconds(4);
        // Enable "get off" button
        //getOffButton = seat.GetComponent<Seat>().EnableGetOffButton();
        foreach (var stop in stops)
        {
            stop.Active = true;
        }
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
        /*foreach (var hands in GameObject.FindGameObjectsWithTag("Hands"))
        {
            hands.gameObject.layer = LayerMask.NameToLayer("Default");
        }*/
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
    // Might need to also use this in MoveOffSeat. Depends on if it is too much of a recurring issue.
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

    // Moves the player off the seat to a target position
    public void MoveOffSeat(GameObject target){
        // Sets the player's position to the target position
        player.transform.position = target.transform.position + new Vector3(0, 0, 0);
        

        // Moves the camera to the target position
        //xrOrigin.MoveCameraToWorldLocation(target.transform.position + new Vector3(0, 0, 0));
        // Makes the camera look in the same direction as the target position
        xrOrigin.MatchOriginUpCameraForward(target.transform.up, target.transform.forward);

        // Makes the player look in the same direction as the target position
        Quaternion newRot;
        // Depending which way the target position is turned, some different behaviour is needed
        if(target.transform.rotation.w < 0){
            newRot = new Quaternion(0, target.transform.rotation.w, 0, 0);
        }else{
            newRot = new Quaternion(0, 0, 0, 0);
        }

        // Rotates the player to match the rotation
        player.transform.rotation = newRot;
    }

    // Old function for recentering the player on the target position
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

    // Cooldown to prevent them sitting back down immediately
    IEnumerator Cooldown(){
        seatCooldown = 3;
        yield return new WaitForSeconds(seatCooldown);
        seatCooldown = 0;
    }

    // Disables the seat signs, as they are not needed when seated
    void DisableSeatSigns(){
        foreach (var seat in FindObjectsOfType<Seat>())
        {
            if(seat.panel != null) seat.DisableScreen();
        }
    }

    // Enables the seat signs
    void EnableSeatSigns(){
        foreach (var seat in FindObjectsOfType<Seat>())
        {
            if(seat.panel != null) seat.Enable();
        }
    }
}
