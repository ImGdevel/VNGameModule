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
        soundSetting = new SoundSettings();
    }

    public override void LoadSettingsToUI(Settings settings) {
        soundSetting = settings.soundSettings;

        for (int i = 0; i < volumeElements.Count; i++) {
            Slider slider = volumeElements[i].GetComponentInChildren<Slider>();
            if (slider != null) {
                slider.value = GetVolume(i);
                UpdateValueText(i);
            }
        }
    }

    public override void ApplyUIToSettings(Settings settings) {
        settings.soundSettings = soundSetting;
    }

    public void SetVolume(int sliderIndex) {
        if (soundSetting != null && sliderIndex >= 0 && sliderIndex < volumeElements.Count) {
            Slider slider = volumeElements[sliderIndex].GetComponentInChildren<Slider>();
            float volume = slider.value;

            switch (sliderIndex) {
                case 0:
                    soundSetting.masterVolume = volume;
                    break;
                case 1:
                    soundSetting.musicVolume = volume;
                    break;
                case 2:
                    soundSetting.sfxVolume = volume;
                    break;
                case 3:
                    soundSetting.dialogVolume = volume;
                    break;
                case 4:
                    soundSetting.UIVolume = volume;
                    break;
                    // 다른 볼륨 슬라이더의 처리를 추가할 수 있습니다.
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
                    // 다른 볼륨 슬라이더의 처리를 추가할 수 있습니다.
            }
        }
        return 0f;
    }

    void UpdateValueText(int sliderIndex) {
        if (sliderIndex >= 0 && sliderIndex < volumeElements.Count) {
            TMP_Text textMeshPro = volumeElements[sliderIndex].GetComponentInChildren<TMP_Text>();
            if (textMeshPro != null) {
                textMeshPro.text = GetVolume(sliderIndex).ToString();
            }
        }
    }
}
