using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] Animator animator;
    
    public void OpenDoors(){
        animator.ResetTrigger("CloseDoor");
        animator.SetTrigger("OpenDoor");

        door.SetActive(false);
    }

    public void CloseDoors(){
        animator.ResetTrigger("OpenDoor");
        animator.SetTrigger("CloseDoor");

        door.SetActive(true);
    }
}
