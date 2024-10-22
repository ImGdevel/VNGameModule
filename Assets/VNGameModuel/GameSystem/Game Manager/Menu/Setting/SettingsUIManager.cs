using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MenuModal
{
    [SerializeField] List<SettingOption> SettingOptions;

    private Settings settings;

    private void OnEnable()
    {
        OpenMenu();
    }

    public override void OpenMenu()
    {
        settings = SettingsManager.GameSetting;
        ShowSettingPanel(SettingOptions[0]);
    }

    private void OnDisable()
    {
        CloseMenu();
    }

    public override void CloseMenu()
    {
        SettingsManager.Instance.ApplySetting(settings);
    }

    public void ShowSettingPanel(SettingOption settingPanel)
    {
        foreach (SettingOption settingOption in SettingOptions) {
            GameObject panel = settingOption.gameObject;
            if (settingOption == settingPanel) {
                SettingOption setting = settingOption.GetComponent<SettingOption>();
                setting.LoadSettingsToUI(settings);
                panel.SetActive(true);
            }
            else {
                panel.SetActive(false);
            }
        }
    }

    public void SaveSettings()
    {
        foreach (SettingOption settingOption in SettingOptions) {
            settingOption.ApplyUIToSettings(settings);
        }
    }
}
