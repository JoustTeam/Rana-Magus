using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

//changes the language to the whole project to the chosen language

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;
    public void ChangeLocale(int localeID)
    {
        if(active == true)
        {
            return;
        }
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }
}