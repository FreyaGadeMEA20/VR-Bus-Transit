using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSeatAssigner : MonoBehaviour
{
    [SerializeField] GameObject player;
    GameObject getOffButton;
    
    Seat currentSeat;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    // Assigns the player to the given seat
    public void AssignSeat(Seat seat)
    {
        currentSeat = seat;
        //TODO: Tune position and rotation
        // Seat the player - ROTATION OF THE SEAT IS IMPORTANT
        player.transform.position = seat.seatingArea.transform.position;
        player.transform.rotation = seat.seatingArea.transform.rotation;

        // Disable the player's movement

        // Enable "get off" button
        getOffButton = seat.GetComponent<Seat>().EnableGetOffButton();
    }

    // Unassigns the player from the given seat
    public void UnassignSeat()
    {
        // Move the player to the closest bus exit  - ROTATION OF THE AREA IS IMPORTANT
        player.transform.position = currentSeat.exitArea.transform.position;
        player.transform.rotation = currentSeat.exitArea.transform.rotation;

        // Enable the player's movement

        // Disable "get off" button
        currentSeat.GetComponent<Seat>().DisableGetOffButton();

        currentSeat = null;
    }
}
