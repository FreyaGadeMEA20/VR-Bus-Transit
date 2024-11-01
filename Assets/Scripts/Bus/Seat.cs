using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Seat : MonoBehaviour
{
    public GameObject seatingArea;
    public GameObject exitArea;
    public GameObject getOffButton;

    void Awake(){
        seatingArea = gameObject;
    }

    // - POSSIBLE JUST CHANGE TO A STATIC BUTTON -
    
    // Creates a get off button for the player at the seat
    public GameObject EnableGetOffButton(){
        Vector3 PositionOffset = new Vector3(0, 0.5f, 0);
        Quaternion quaternion = new Quaternion(90, 0, -90, 0);

        // Instantiate the button, set its parent to the seating area and set its position and rotation
        GameObject _getOffButton = Instantiate(Resources.Load("Prefabs/GetOffButton"),
            PositionOffset, quaternion, seatingArea.transform) as GameObject;

        // Give it memory of the button        
        getOffButton = _getOffButton;

        // Return the button
        return getOffButton;
    }

    // Disables the get off button for the player at the seat
    public void DisableGetOffButton(){
        Destroy(getOffButton);
    }
}
