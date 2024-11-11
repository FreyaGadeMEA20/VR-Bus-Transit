using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityProgressBar;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] Material phoneScreen;
    [SerializeField] PhoneScreen[] phoneScreens;
    
    public InputActionProperty pullOutPhone;
    bool animationPlaying = false;

    [SerializeField] RectTransform rejseplanenScreen;
    [SerializeField] RectTransform progressSlider;
    [SerializeField] RectTransform checkMark;

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

    void Start()
    {
        GameManager.Instance.OnStateChange += SwitchToScreen;
        pullOutPhone = GameManager.Instance.handSwitch;
        pullOutPhone.action.performed += ctx => StartCoroutine(TogglePhone());
        GameManager.Instance.OnVariableChange += PhoneSlider;

        // Test data   
        FillRejseplanen("Ørneskolen", "Nørreport St.", "150S", "Nørreport St.");
    }

    void FillRejseplanen(string from, string to, string bus, string getOff)
    {
        rejseplanScreen.from.text = from;
        rejseplanScreen.to.text = to;
        rejseplanScreen.bus.text = bus;
        rejseplanScreen.getOff.text = getOff;
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
        progressSlider.GetComponent<UnityProgressBar.ProgressBar>().Value = newVal;
        Debug.Log("Slider value: " + progressSlider.GetComponent<UnityProgressBar.ProgressBar>().Value);

        if(newVal >=4){
            StartCoroutine(TogglePhone());
            progressSlider.gameObject.SetActive(false);
            checkMark.gameObject.SetActive(true);
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
