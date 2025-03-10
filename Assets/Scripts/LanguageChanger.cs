using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;
/* Script made to change the language of the application
 * Takes a given id and uses the localization package to change the language
*/
public class LanguageChanger : MonoBehaviour
{
    // Variables
    private bool active = false; // Prevents multiple calls
    [SerializeField] int id;     // ID of the language - used to make sure the correct default language is set
    [SerializeField] GameObject outline; // Outline to show which language is selected
    void Start()
    {
        if(id == 0) SetLanguage(0); // Set the default language - only runs if the id is 0 (Danish)
    }

    // Function to change the language
    // Assigned to a button
    public void SetLanguage(int _localeID) {
        // Deactivate all outlines
        foreach(var obj in FindObjectsOfType<LanguageChanger>()) {
            obj.outline.SetActive(false);
        }
        
        // Activate the outline of the selected language
        outline.SetActive(true);

        // Prevent multiple calls
        if(active) return;
        
        // Change the language
        StartCoroutine(SetLocale(_localeID));
    }

    // Coroutine to change the language
    IEnumerator SetLocale(int _localeID) {
        active = true; // prevents multiple calls
        yield return LocalizationSettings.InitializationOperation; // Wait for the localization package to initialize
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID]; // Set the language
        active = false; // Set active to false to allow for another call
    }
}
