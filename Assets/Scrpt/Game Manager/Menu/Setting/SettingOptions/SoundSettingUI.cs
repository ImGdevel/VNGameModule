using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettingUI : SettingOption
{
    [SerializeField] List<GameObject> volumeElements;

    SoundSettings soundSetting;

    private void Start() {
        for (int i = 0; i < volumeElements.Count; i++) {
            int sliderIndex = i;

            Slider slider = volumeElements[i].GetComponentInChildren<Slider>();
            TMP_Text textMeshPro = volumeElements[i].GetComponentInChildren<TMP_Text>();

            if (slider != null && textMeshPro != null) {
                slider.onValueChanged.AddListener(delegate { SetVolume(sliderIndex); });
            }
            else {
                Debug.LogWarning("Slider or TextMesh Pro component not found in GameObject " + i);
            }
        }
    }

    public override void LoadSettingsToUI(Settings settings) {
        soundSetting = settings.soundSettings;
        for (int i = 0; i < volumeElements.Count; i++) {
            Slider slider = volumeElements[i].GetComponentInChildren<Slider>();
            if (slider != null) {
                slider.value = GetVolume(i) * 100;
                UpdateValueText(i);
            }
        }
    }

    public override void ApplyUIToSettings(Settings settings) {
        settings.soundSettings = soundSetting;
    }

    // UI에 값을 표시할 때 0 ~ 100으로 변환
    private void UpdateValueText(int sliderIndex) {
        if (sliderIndex >= 0 && sliderIndex < volumeElements.Count) {
            TMP_Text textMeshPro = volumeElements[sliderIndex].GetComponentInChildren<TMP_Text>();
            if (textMeshPro != null) {
                float normalizedValue = GetVolume(sliderIndex) * 100;
                textMeshPro.text = normalizedValue.ToString("F0");
            }
        }
    }

    // 사용할 때는 0 ~ 1로 변환
    public void SetVolume(int sliderIndex) {
        if (soundSetting != null && sliderIndex >= 0 && sliderIndex < volumeElements.Count) {
            Slider slider = volumeElements[sliderIndex].GetComponentInChildren<Slider>();
            float normalizedValue = slider.value / 100f;

            switch (sliderIndex) {
                case 0:
                    soundSetting.masterVolume = normalizedValue;
                    break;
                case 1:
                    soundSetting.musicVolume = normalizedValue;
                    break;
                case 2:
                    soundSetting.sfxVolume = normalizedValue;
                    break;
                case 3:
                    soundSetting.dialogVolume = normalizedValue;
                    break;
                case 4:
                    soundSetting.UIVolume = normalizedValue;
                    break;
            }

            UpdateValueText(sliderIndex);
        }
    }

    float GetVolume(int sliderIndex) {
        if (soundSetting != null) {
            switch (sliderIndex) {
                case 0:
                    return soundSetting.masterVolume;
                case 1:
                    return soundSetting.musicVolume;
                case 2:
                    return soundSetting.sfxVolume;
                case 3:
                    return soundSetting.dialogVolume;
                case 4:
                    return soundSetting.UIVolume;
            }
        }
        return 0f;
    }
}
