using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] Material phoneScreen;
    [SerializeField] PhoneScreen[] phoneScreens;

    [Serializable]
    public class PhoneScreen
    {
        public Texture screen;
        public GameManager.GameState state;
    }

    void Start()
    {
        GameManager.Instance.OnStateChange += SwitchToScreen;
    }

    public void SwitchToScreen(GameManager.GameState state)
    {
        foreach (PhoneScreen screen in phoneScreens)
        {
            if (screen.state == state)
            {
                phoneScreen.mainTexture = screen.screen;
            }
        }
    }
}
