using System;
using System.Collections;
using System.Collections.Generic;
using SerializedTuples;
using SerializedTuples.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] Animator phoneAnimator;
    [SerializeField] Material phoneScreen;
    [SerializeField] PhoneScreen[] phoneScreens;
    
    public InputActionProperty pullOutPhone;
    bool animationPlaying = false;

    [SerializeField] RectTransform rejseplanenScreen;
    [SerializeField] RectTransform progressSlider;
    [SerializeField] RectTransform checkMark;
    [SerializeField] AudioSource taskCompleteSound;

    [Serializable]
    public class PhoneScreen
    {
        public RectTransform screen;
        public GameManager.GameState state;
    }

    [Serializable]
    public class RejseplanScreen{
        public TextMeshProUGUI from;
        public TextMeshProUGUI to;
        public TextMeshProUGUI bus;
        public TextMeshProUGUI getOff;

    }
    public RejseplanScreen rejseplanScreen;

    [SerializedTupleLabels("From", "To", "Bus Number", "Get off")]
    public SerializedTuple<string, string, string, string> RejseplanInfo;

    void Start()
    {
        GameManager.Instance.OnStateChange += SwitchToScreen;
        pullOutPhone = GameManager.Instance.handSwitch;
        pullOutPhone.action.performed += ctx => StartCoroutine(TogglePhone());
        GameManager.Instance.OnVariableChange += PhoneSlider;

        phoneAnimator.SetTrigger("HoldObject");

        // Test data   
        StartCoroutine(FillRejseplanen());
    }



    IEnumerator FillRejseplanen()
    {
        yield return new WaitForSeconds(1);

        rejseplanScreen.from.text = GameManager.Instance.BusLine.BusLineID.v2;
        rejseplanScreen.to.text = GameManager.Instance.BusLine.BusLineID.v3;
        rejseplanScreen.bus.text = GameManager.Instance.BusLine.BusLineID.v1.ToString();
        rejseplanScreen.getOff.text = GameManager.Instance._finalDestination.busStop.BusStopName;
    }

    public void SwitchToScreen(GameManager.GameState state)
    {
        foreach (PhoneScreen screen in phoneScreens)
        {
            screen.screen.gameObject.SetActive(false);
            if (screen.state == state)
            {
                screen.screen.gameObject.SetActive(true);
            }
        }
    }

    void PhoneSlider(float newVal)
    {
        if(newVal <=0){
            progressSlider.gameObject.SetActive(false);
            checkMark.gameObject.SetActive(false);
        } else {
            progressSlider.gameObject.SetActive(true);
        }
        float clampedValue = Mathf.Lerp(1, 4, newVal/GameManager.Instance.Timer);
        Debug.Log(clampedValue + "    " + newVal);
        progressSlider.GetComponent<UnityProgressBar.ProgressBar>().Value = clampedValue;
        //Debug.Log($"Slider value: + {progressSlider.GetComponent<UnityProgressBar.ProgressBar>().Value:0.0}");

        if(newVal >= GameManager.Instance.Timer){
            StartCoroutine(TogglePhone());
            progressSlider.gameObject.SetActive(false);
            checkMark.gameObject.SetActive(true);
            taskCompleteSound.Play();
        }
    }

    IEnumerator TogglePhone()
    {
        if(animationPlaying)
        {
            yield break;
        }
        animationPlaying = true;
        Vector2 targetPosition;
        if (rejseplanenScreen.anchoredPosition == Vector2.zero)
        {
            GameManager.Instance.buttonPressed = false;
            targetPosition = new Vector2(64, 0);
        }
        else
        {
            GameManager.Instance.buttonPressed = true;
            targetPosition = Vector2.zero;
        }

        Vector2 startPosition = rejseplanenScreen.anchoredPosition;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            rejseplanenScreen.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rejseplanenScreen.anchoredPosition = targetPosition;
        animationPlaying = false;
        
        checkMark.gameObject.SetActive(false);
    }
}
