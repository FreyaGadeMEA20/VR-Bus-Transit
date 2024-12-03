using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BusSeatAssigner : MonoBehaviour
{
    public GameObject player;
    Seat currentSeat;
    [SerializeField] Transform head;
    [SerializeField] Transform origin;
    
    [SerializeField] LayerMask seatedLayer;
    public bool PlayerSeated => currentSeat != null;

    float seatCooldown;

    public void GetPlayer(GameObject _player){
        player = _player;
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
        StartCoroutine(FadeToBlack.Instance.FadeOut());
        yield return new WaitForSeconds(FadeToBlack.Instance.fadeDuration);
        //Debug.Log("Assigning seat");
        //TODO: Tune position and rotation
        // Seat the player - ROTATION OF THE SEAT IS IMPORTANT
        
        Recenter(currentSeat.seatingArea);
        
        // Disable the player's movement
        player.layer = LayerMask.NameToLayer("SeatedPlayer");
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 0;
        
        player.GetComponent<DynamicMoveProvider>().useGravity = false;
        
        StartCoroutine(FadeToBlack.Instance.FadeIn());
        // Enable "get off" button
        //getOffButton = seat.GetComponent<Seat>().EnableGetOffButton();
    }


    // Unassigns the player from the given seat
    public void UnassignSeat()
    {
        StartCoroutine(GetOffSeat());
    }

    IEnumerator GetOffSeat(){
        StartCoroutine(FadeToBlack.Instance.FadeOut());
        yield return new WaitForSeconds(FadeToBlack.Instance.fadeDuration);
        // Move the player to the closest bus exit - ROTATION OF THE AREA IS IMPORTANT
        Recenter(currentSeat.exitArea);

        player.layer = LayerMask.NameToLayer("Default");
        // Enable the player's movement
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 3;
        player.GetComponent<DynamicMoveProvider>().useGravity = true;

        // Disable "get off" button
        //currentSeat.GetComponent<Seat>().DisableGetOffButton();

        currentSeat = null;
        StartCoroutine(FadeToBlack.Instance.FadeIn());
        

    }

    public void Recenter(GameObject target){
        //player.transform.position = target.transform.position;
        
        //XROrigin xrOrigin = player.GetComponent<XROrigin>();
        //xrOrigin.MoveCameraToWorldLocation(target.transform.position);
        //xrOrigin.MatchOriginUpCameraForward(target.transform.up, target.transform.forward);

        Quaternion newRot;

        if(target.transform.rotation.w < 0){
            newRot = new Quaternion(0, target.transform.rotation.w, 0, 0);
        }else{
            newRot = new Quaternion(0, 0, 0, 0);
        }
        player.transform.rotation = newRot;
    }

    IEnumerator Cooldown(){
        seatCooldown = 3;
        yield return new WaitForSeconds(seatCooldown);
        seatCooldown = 0;
    }
}
