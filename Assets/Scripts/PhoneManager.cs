using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityProgressBar;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] Material phoneScreen;
    [SerializeField] PhoneScreen[] phoneScreens;
    
    public InputActionProperty pulLOutPhone;
    bool animationPlaying = false;

    [SerializeField] RectTransform rejseplanenScreen;
    [SerializeField] RectTransform progressSlider;
    [SerializeField] RectTransform checkMark;

    [Serializable]
    public class PhoneScreen
    {
        public GameObject screen;
        public GameManager.GameState state;
    }

    void Start()
    {
        GameManager.Instance.OnStateChange += SwitchToScreen;
        pulLOutPhone = GameManager.Instance.handSwitch;
        pulLOutPhone.action.performed += ctx => StartCoroutine(TogglePhone());
        GameManager.Instance.OnVariableChange += PhoneSlider;
    }

    public void SwitchToScreen(GameManager.GameState state)
    {
        foreach (PhoneScreen screen in phoneScreens)
        {
            screen.screen.SetActive(false);
            if (screen.state == state)
            {
                screen.screen.SetActive(true);
            }
        }
    }

    void PhoneSlider(float newVal)
    {
        progressSlider.GetComponent<UnityProgressBar.ProgressBar>().Value = newVal;
        Debug.Log("Slider value: " + progressSlider.GetComponent<UnityProgressBar.ProgressBar>().Value);

        if(newVal >=3){
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
    }
}
