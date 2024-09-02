using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRManager : MonoBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] GameObject item;

    public InputActionProperty handSwitch;
    Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        handAnimator = hand.GetComponent<Animator>();
        item.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool hSwitch = false; 
        if(handSwitch.action.ReadValue<float>() > 0.8f){
            hSwitch = !hSwitch;
        }

        switch(hSwitch){
            case true:
                ShowGameObject();
                break;
            case false:
                HideGameObject();
                break;
        }
    }

    void ShowGameObject(){
        // Animate hand to hold object
        handAnimator.SetTrigger("HoldObject");

        // Activate gameobject
        item.SetActive(true);

    }

    void HideGameObject(){
        // Return hand to other animation
        handAnimator.SetTrigger("FreeHand");

        // Deactivate gameobject
        item.SetActive(false);
    }
}
