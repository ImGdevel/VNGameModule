using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    bool isChange;

    public static List<string> SupportedLanguages { get; internal set; }

    public void ChangeLocalization(int index)
    {
        if (isChange) {
            return;
        }
        StartCoroutine(ChangeLanguage(index));
    }

    IEnumerator ChangeLanguage(int index)
    {
        isChange = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isChange = false;
    }
}


