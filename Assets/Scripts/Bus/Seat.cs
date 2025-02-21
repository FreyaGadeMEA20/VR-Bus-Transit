using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Seat : MonoBehaviour
{
    public GameObject seatingArea;
    public GameObject exitArea;
    public GameObject panel;
    public GameObject associatedButton;

    void Awake(){
        seatingArea = gameObject;
    }

    public void DisableButton(){
        associatedButton.GetComponent<BoxCollider>().enabled = false;
        associatedButton.GetComponent<Image>().color = Color.red;
    }

    // - POSSIBLE JUST CHANGE TO A STATIC BUTTON -
    
    // Creates a get off button for the player at the seat
    public void DisableScreen(){
        panel.SetActive(false);
    }

    public void Enable(){
        panel.SetActive(true);
    }
}
