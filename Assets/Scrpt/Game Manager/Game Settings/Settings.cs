using System;
using UnityEngine;

[System.Serializable]
public class Settings
{
    public GraphicsSettings graphicsSettings;
    public SoundSettings soundSettings;
    public DialogueSettings dialogueSettings;
    public LanguageSettings languageSettings;
    public ControlSettings controlSettings;

    public override string ToString()
    {
        return $"Graphics Settings:\n{graphicsSettings}\n\n" +
               $"Sound Settings:\n{soundSettings}\n\n" +
               $"Dialogue Settings:\n{dialogueSettings}\n\n" +
               $"Language Settings:\n{languageSettings}\n\n" +
               $"Control Settings:\n{controlSettings}";
    }

    private static int instanceCount = 0; // Ŭ���� �ν��Ͻ� ������ �����ϱ� ���� ���� ����

    public Settings()
    {
        instanceCount++; // Ŭ������ �����ڰ� ȣ��� ������ �ν��Ͻ� ���� ����
    }

    ~Settings() // �Ҹ��� (�ı���)
    {
        instanceCount--; // Ŭ������ �ν��Ͻ��� �Ҹ�� ������ �ν��Ͻ� ���� ����
    }

    public static int GetInstanceCount()
    {
        return instanceCount; // ���� Ŭ���� �ν��Ͻ� ���� ��ȯ
    }
}

[System.Serializable]
public class GraphicsSettings
{
    public Resolution Resolution {
        set {
            resolution = new();
            resolution.width = value.width;
            resolution.height = value.height;
        }
        get {
            return new Resolution {
                width = resolution.width,
                height = resolution.height,
            };
        }
    }

    public ScreenResolution resolution;
    public FullScreenMode fullScreenMode; //���� fullScreen��� FullScreenMode���
    public int qualityLevel;

    public override string ToString()
    {
        return $"Resolution: {Resolution.width}x{Resolution.height}\n" +
               $"FullScreenMode: {fullScreenMode}\n" +
               $"Quality Level: {qualityLevel}";
    }
}

[System.Serializable]
public class ScreenResolution
{
    public int width;
    public int height;
}

[System.Serializable]
public class SoundSettings
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float dialogVolume;
    public float UIVolume;

    public override string ToString()
    {
        return $"Master Volume: {masterVolume}\n" +
               $"Music Volume: {musicVolume}\n" +
               $"SFX Volume: {sfxVolume}\n" +
               $"Dialog Volume: {dialogVolume}\n" +
               $"UI Volume: {UIVolume}";
    }
}

[System.Serializable]
public class DialogueSettings
{
    public float typingSpeed;
    public float dialogueDelay;
    public float dialogueBoxTransparency;

    public override string ToString()
    {
        return $"Typing Speed: {typingSpeed}\n" +
               $"Dialogue Delay: {dialogueDelay}\n" +
               $"Dialogue Box Transparency: {dialogueBoxTransparency}";
    }
}

[System.Serializable]
public class LanguageSettings
{
    public string selectedLanguage;

    public override string ToString()
    {
        return $"Selected Language: {selectedLanguage}";
    }
}

[System.Serializable]
public class ControlSettings
{
    public KeyCode SkipKeyCode;
    public KeyCode NextDialogKeyCode;
    public KeyCode AutoDialogKeyCode;
    public KeyCode HideUIKeyCode;
    public KeyCode QuickSaveKeyCode;
    public KeyCode SaveKeyCode;
    public KeyCode LoadKeyCode;
    public KeyCode ShowLogKeyCode;

    public override string ToString()
    {
        return $"Skip Key: {SkipKeyCode}\n" +
               $"Next Dialog Key: {NextDialogKeyCode}\n" +
               $"Auto Dialog Key: {AutoDialogKeyCode}\n" +
               $"Hide UI Key: {HideUIKeyCode}\n" +
               $"Quick Save Key: {QuickSaveKeyCode}\n" +
               $"Save Key: {SaveKeyCode}\n" +
               $"Load Key: {LoadKeyCode}\n" +
               $"Show Log Key: {ShowLogKeyCode}";
    }
}
