using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BusSeatAssigner : MonoBehaviour
{
    public GameObject player;
    Seat currentSeat;
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
        Debug.Log("Assigning seat");
        //TODO: Tune position and rotation
        // Seat the player - ROTATION OF THE SEAT IS IMPORTANT
        player.transform.position = currentSeat.seatingArea.transform.position;
        
        Quaternion newRot;

        if(currentSeat.seatingArea.transform.rotation.w<0){
            newRot = new Quaternion(0,currentSeat.seatingArea.transform.rotation.w,0,0);
        }else{
            newRot = new Quaternion(0,0,0,0);
        }
        player.transform.rotation = newRot;

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
        player.transform.position = currentSeat.exitArea.transform.position;Quaternion newRot;

        if(currentSeat.exitArea.transform.rotation.w < 0){
            newRot = new Quaternion(0,currentSeat.exitArea.transform.rotation.w,0,0);
        }else{
            newRot = new Quaternion(0,0,0,0);
        }
        Debug.Log("New rotation: " + newRot);
        player.transform.rotation = newRot;

        player.layer = LayerMask.NameToLayer("Default");
        // Enable the player's movement
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 3;
        player.GetComponent<DynamicMoveProvider>().useGravity = true;

        // Disable "get off" button
        //currentSeat.GetComponent<Seat>().DisableGetOffButton();

        currentSeat = null;
        StartCoroutine(FadeToBlack.Instance.FadeIn());
        

    }

    IEnumerator Cooldown(){
        seatCooldown = 3;
        yield return new WaitForSeconds(seatCooldown);
        seatCooldown = 0;
    }
}
