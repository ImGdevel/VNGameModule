using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] List<SettingOption> SettingOptions;

    private Settings settings;

    public void OpenSettingMenu() {
        settings = SettingsManager.GameSetting;
        ShowSettingPanel(SettingOptions[0]);
    }

    public void ShowSettingPanel(SettingOption settingPanel) {
        foreach (SettingOption settingOption in SettingOptions) {
            GameObject panel = settingOption.gameObject;
            if(settingOption == settingPanel) {
                SettingOption setting = settingOption.GetComponent<SettingOption>();
                setting.LoadSettingsToUI(settings);
                panel.SetActive(true);
            }
            else {
                panel.SetActive(false);
            }
        }
    }

    private void SaveSettings() {
        foreach (SettingOption settingOption in SettingOptions) {
            settingOption.ApplyUIToSettings(settings);
        }
    }

    private void OnDisable() {
        SettingsManager.Instance.ApplySetting(settings);
    }
}
