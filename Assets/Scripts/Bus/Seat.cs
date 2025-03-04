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
    
    // Disables the seat sign 
    public void DisableScreen(){
        panel.SetActive(false);
    }

    // Enables the seat sign
    public void Enable(){
        panel.SetActive(true);
    }
}
