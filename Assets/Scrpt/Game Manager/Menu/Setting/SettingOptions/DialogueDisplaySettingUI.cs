using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueDisplaySettingUI : SettingOption
{
    [SerializeField] private Slider typingSpeedSlider; // Typing Speed를 조절하는 슬라이더
    [SerializeField] private Slider delaySlider; // Dialogue Delay를 조절하는 슬라이더
    [SerializeField] private Slider transparencySlider; // Dialogue Box Transparency를 조절하는 슬라이더

    [SerializeField] private TMP_Text typingSpeedText; // Typing Speed 값을 나타내는 텍스트
    [SerializeField] private TMP_Text delayText; // Dialogue Delay 값을 나타내는 텍스트
    [SerializeField] private TMP_Text transparencyText; // Dialogue Box Transparency 값을 나타내는 텍스트

    public override void LoadSettingsToUI(Settings settings) {
        typingSpeedSlider.value = settings.dialogueSettings.typingSpeed;
        UpdateValueText(typingSpeedText, typingSpeedSlider.value);

        delaySlider.value = settings.dialogueSettings.dialogueDelay;
        UpdateValueText(delayText, delaySlider.value);

        transparencySlider.value = settings.dialogueSettings.dialogueBoxTransparency;
        UpdateValueText(transparencyText, transparencySlider.value);
    }

    public override void ApplyUIToSettings(Settings settings) {
        settings.dialogueSettings.typingSpeed = typingSpeedSlider.value;
        settings.dialogueSettings.dialogueDelay = delaySlider.value;
        settings.dialogueSettings.dialogueBoxTransparency = transparencySlider.value;
    }

    public void SetTypingSpeed(float value) {
        UpdateValueText(typingSpeedText, value);
    }

    public void SetDelay(float value) {
        UpdateValueText(delayText, value);
    }

    public void SetTransparency(float value) {
        UpdateValueText(transparencyText, value);
    }

    void UpdateValueText(TMP_Text textMeshPro, float value) {
        if (textMeshPro != null) {
            textMeshPro.text = value.ToString();
        }
    }
}
