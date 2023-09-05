using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] List<SettingOption> SettingOptions;

    private Settings settings;

    private void Start() {
        LoadSettings();
        ShowSettingMenu();
    }

    public void ShowSettingMenu() {
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

    private void LoadSettings() {
        settings = SettingsManager.GetSettings;
    }

    private void SaveSettings(Settings settings) {
        foreach (SettingOption settingOption in SettingOptions) {
            settingOption.ApplyUIToSettings(settings);
        }
    }
}
