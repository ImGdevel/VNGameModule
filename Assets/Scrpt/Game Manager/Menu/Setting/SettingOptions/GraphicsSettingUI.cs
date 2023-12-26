using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingUI : SettingOption
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // 해상도 설정을 위한 TMP Dropdown 메뉴
    [SerializeField] private TMP_Dropdown fullscreenModeDropdown; // 전체 화면 모드 설정을 위한 TMP Dropdown 메뉴
    [SerializeField] private TMP_Dropdown qualityDropdown; // 그래픽 품질 설정을 위한 TMP Dropdown 메뉴

    GraphicsSettings graphicsSettings;

    public enum FullScreenModeEnum
    {
        FullScreen,
        Windowed,
    }

    private void Start() {
        InitializeResolutionDropdown();
        InitializeFullScreenModeDropdown();
        InitializeQualityDropdown();
    }

    // 해상도 드롭다운을 초기화
    private void InitializeResolutionDropdown() {
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        Resolution[] availableResolutions = SettingsManager.AvailableResolutions;
        foreach (Resolution resolution in availableResolutions) {
            resolutionOptions.Add(resolution.width + "x" + resolution.height);
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.onValueChanged.AddListener(ResolutionDropdownValueChanged);
    }

    // 전체 화면 모드 Dropdown을 초기화하는 메서드
    private void InitializeFullScreenModeDropdown() {
        string[] fullScreenModeOptions = System.Enum.GetNames(typeof(FullScreenModeEnum));

        fullscreenModeDropdown.ClearOptions();
        fullscreenModeDropdown.AddOptions(new List<string>(fullScreenModeOptions));
        fullscreenModeDropdown.onValueChanged.AddListener(OnFullScreenModeDropdownValueChanged);
    }

    private void InitializeQualityDropdown() {
        qualityDropdown.ClearOptions();

        List<string> qualityOptions = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.onValueChanged.AddListener(QualityDropdownValueChanged);
    }

    public int GetResolutionIndex(Resolution resolution) {
        Resolution currentResolution = resolution;
        Resolution[] availableResolutions = SettingsManager.AvailableResolutions;
        for (int i = 0; i < availableResolutions.Length; i++) {
            Resolution availableResolution = availableResolutions[i];
            if (availableResolution.width == currentResolution.width &&
                availableResolution.height == currentResolution.height) {
                return i;
            }
        }
        return 0;
    }

    public override void LoadSettingsToUI(Settings loadSettings) {
        if (loadSettings == null || loadSettings.graphicsSettings == null) {
            Debug.LogWarning("No SerchDatas");
            return;
        }
        graphicsSettings = loadSettings.graphicsSettings;
        // 해상도 설정 로드
        resolutionDropdown.value = GetResolutionIndex(loadSettings.graphicsSettings.Resolution);
        // 그래픽 퀄리티 설정 로드
        qualityDropdown.value = loadSettings.graphicsSettings.qualityLevel;
    }

    public override void ApplyUIToSettings(Settings settings) {
        graphicsSettings = settings.graphicsSettings;
    }

    // 사용자가 해상도를 선택했을 때 호출되는 메서드
    private void ResolutionDropdownValueChanged(int value) {
        Resolution[] availableResolutions = SettingsManager.AvailableResolutions;
        Resolution selectedResolution = availableResolutions[value];

        graphicsSettings.Resolution = selectedResolution;
        ScreenManager.ChangeResolution(selectedResolution);
    }

    // 사용자가 전체 화면 모드를 선택했을 때 호출되는 메서드
    public void OnFullScreenModeDropdownValueChanged(int value) {
        string selectedModeString = fullscreenModeDropdown.options[value].text;
        FullScreenModeEnum selectedMode = (FullScreenModeEnum)System.Enum.Parse(typeof(FullScreenModeEnum), selectedModeString);

        graphicsSettings.fullScreenMode = ConvertFullScreenMode(selectedMode);
        Screen.fullScreenMode = ConvertFullScreenMode(selectedMode);
    }

    // FullScreenModeEnum을 Unity의 FullScreenMode로 변환
    private FullScreenMode ConvertFullScreenMode(FullScreenModeEnum mode) {
        switch (mode) {
            case FullScreenModeEnum.FullScreen:
                return FullScreenMode.FullScreenWindow;
            case FullScreenModeEnum.Windowed:
                return FullScreenMode.Windowed;
            default:
                return FullScreenMode.FullScreenWindow;
        }
    }

    private void QualityDropdownValueChanged(int value) {
        graphicsSettings.qualityLevel = value;
        QualitySettings.SetQualityLevel(value);
    }
}

