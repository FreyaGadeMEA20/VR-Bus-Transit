using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BusSeatAssigner : MonoBehaviour
{
    public GameObject player;
    GameObject getOffButton;
    Seat currentSeat;
    [SerializeField] LayerMask seatedLayer;
    public bool PlayerSeated => currentSeat != null;

    public void GetPlayer(GameObject _player){
        player = _player;
    }
    
    // Assigns the player to the given seat
    public void AssignSeat(Seat seat)
    {
        Debug.Log("Assigning seat");
        currentSeat = seat;
        //TODO: Tune position and rotation
        // Seat the player - ROTATION OF THE SEAT IS IMPORTANT
        player.transform.position = seat.seatingArea.transform.position;
        
        Quaternion newRot;

        if(seat.seatingArea.transform.rotation.w<0){
            newRot = new Quaternion(0,seat.seatingArea.transform.rotation.w,0,0);
        }else{
            newRot = new Quaternion(0,0,0,0);
        }
        player.transform.rotation = newRot;

        // Disable the player's movement
        player.layer = LayerMask.NameToLayer("SeatedPlayer");
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 0;
        
        player.GetComponent<DynamicMoveProvider>().useGravity = false;

        // Enable "get off" button
        //getOffButton = seat.GetComponent<Seat>().EnableGetOffButton();
    }

    // Unassigns the player from the given seat
    public void UnassignSeat()
    {
        // Move the player to the closest bus exit  - ROTATION OF THE AREA IS IMPORTANT
        player.transform.position = currentSeat.exitArea.transform.position;Quaternion newRot;

        if(currentSeat.exitArea.transform.rotation.w<0){
            newRot = new Quaternion(0,currentSeat.exitArea.transform.rotation.w,0,0);
        }else{
            newRot = new Quaternion(0,0,0,0);
        }
        Debug.Log("New rotation: " + newRot);
        player.transform.rotation = newRot;

        // Enable the player's movement
        player.GetComponent<DynamicMoveProvider>().moveSpeed = 3;
        player.GetComponent<DynamicMoveProvider>().useGravity = true;

        // Disable "get off" button
        //currentSeat.GetComponent<Seat>().DisableGetOffButton();

        currentSeat = null;
    }
}
