using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    private bool active = false;
    [SerializeField] int id;
    [SerializeField] GameObject outline;
    void Start()
    {
        if(id == 0) SetLanguage(0);
    }
    public void SetLanguage(int _localeID) {
        foreach(var obj in FindObjectsOfType<LanguageChanger>()) {
            obj.outline.SetActive(false);
        }
        outline.SetActive(true);
        Debug.Log("Hi");
        if(active) return;
        StartCoroutine(SetLocale(_localeID));
    }

    IEnumerator SetLocale(int _localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }
}
