using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

public class LanguageSettingUI : SettingOption
{
    public TMP_Dropdown languageDropdown; // UI Dropdown 컴포넌트

    private void Start()
    {
        Debug.Log("언어설정");
        InitializeDropdown(); // 먼저 Dropdown을 초기화합니다.
    }

    // Dropdown을 초기화하고 지원되는 언어 목록을 설정
    private void InitializeDropdown()
    {
        if (languageDropdown == null) {
            Debug.LogError("Language Dropdown is not assigned.");
            return;
        }

        List<string> supportedLanguages = LocalizationSettings.AvailableLocales.Locales.ConvertAll(locale => locale.name);

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(supportedLanguages);

        int currentLanguageIndex = supportedLanguages.IndexOf(LocalizationSettings.SelectedLocale.name);
        languageDropdown.value = currentLanguageIndex;

        // Dropdown의 선택 이벤트에 언어 변경 함수 연결
        languageDropdown.onValueChanged.AddListener(OnLanguageDropdownValueChanged);
    }

    // 사용자가 언어를 선택했을 때 호출되는 함수
    private void OnLanguageDropdownValueChanged(int index)
    {
        string selectedLanguage = languageDropdown.options[index].text;

        // 선택한 언어로 언어 변경
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(selectedLanguage);

        // 언어 변경을 저장 (SettingsManager를 사용하여 설정을 저장)
        //Settings settings = SettingsManager.GetSettings;
        //settings.languageSettings.selectedLanguage = selectedLanguage;
        //SettingsManager.SaveSettings(settings);
    }

    public override void LoadSettingsToUI(Settings settings)
    {
        // 저장된 언어 설정을 UI에 적용
        int selectedLanguageIndex = languageDropdown.options.FindIndex(option => option.text == settings.languageSettings.selectedLanguage);
        languageDropdown.value = selectedLanguageIndex;
    }

    public override void ApplyUIToSettings(Settings settings)
    {
        // 사용자가 선택한 언어로 언어 설정을 변경
        int selectedLanguageIndex = languageDropdown.value;
        string selectedLanguage = languageDropdown.options[selectedLanguageIndex].text;
        settings.languageSettings.selectedLanguage = selectedLanguage;

        // 변경된 언어 설정을 저장 (SettingsManager를 사용하여 설정을 저장)
        //SettingsManager.SaveSettings(settings);
    }
}
