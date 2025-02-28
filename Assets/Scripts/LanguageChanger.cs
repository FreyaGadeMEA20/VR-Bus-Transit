using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    private bool active = false;
    public void SetLanguage(int _localeID) {
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
