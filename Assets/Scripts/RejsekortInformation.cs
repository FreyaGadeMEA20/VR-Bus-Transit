using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RejsekortInformation : MonoBehaviour
{
    bool HasChckedIn = false;
    BusController Bus;
    float Balance = 100;

    public static RejsekortInformation Instance { get; internal set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AssignBus(BusController bus){
        Bus = bus;
    }

    public BusController GetBus(){
        return Bus;
    }
    public void DeductBalance(float amount){
        Balance -= amount;
    }

    public void AddBalance(float amount){
        Balance += amount;
    }

    public void CheckIn(){
        HasChckedIn = true;
    }

    public void CheckOut(){
        HasChckedIn = false;
    }

    public bool GetCheckedIn(){
        return HasChckedIn;
    }

    public float GetBalance(){
        return Balance;
    }
}
