using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueDisplaySettingUI : SettingOption
{
    [SerializeField] private Slider typingSpeedSlider; // Typing Speed�� �����ϴ� �����̴�
    [SerializeField] private Slider delaySlider; // Dialogue Delay�� �����ϴ� �����̴�
    [SerializeField] private Slider transparencySlider; // Dialogue Box Transparency�� �����ϴ� �����̴�

    [SerializeField] private TMP_Text typingSpeedText; // Typing Speed ���� ��Ÿ���� �ؽ�Ʈ
    [SerializeField] private TMP_Text delayText; // Dialogue Delay ���� ��Ÿ���� �ؽ�Ʈ
    [SerializeField] private TMP_Text transparencyText; // Dialogue Box Transparency ���� ��Ÿ���� �ؽ�Ʈ

    public override void LoadSettingsToUI(Settings settings)
    {
        typingSpeedSlider.value = settings.dialogueSettings.typingSpeed;
        UpdateValueText(typingSpeedText, typingSpeedSlider.value);

        delaySlider.value = settings.dialogueSettings.dialogueDelay;
        UpdateValueText(delayText, delaySlider.value);

        transparencySlider.value = settings.dialogueSettings.dialogueBoxTransparency;
        UpdateValueText(transparencyText, transparencySlider.value);
    }

    public override void ApplyUIToSettings(Settings settings)
    {
        settings.dialogueSettings.typingSpeed = typingSpeedSlider.value;
        settings.dialogueSettings.dialogueDelay = delaySlider.value;
        settings.dialogueSettings.dialogueBoxTransparency = transparencySlider.value;
    }

    public void SetTypingSpeed(float value)
    {
        UpdateValueText(typingSpeedText, value);
    }

    public void SetDelay(float value)
    {
        UpdateValueText(delayText, value);
    }

    public void SetTransparency(float value)
    {
        UpdateValueText(transparencyText, value);
    }

    void UpdateValueText(TMP_Text textMeshPro, float value)
    {
        if (textMeshPro != null) {
            textMeshPro.text = value.ToString();
        }
    }
}
