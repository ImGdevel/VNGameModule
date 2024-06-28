using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    private MyEditorSettings settings;

    void Start()
    {
        LoadSettings();
        Debug.Log("Loaded setting: " + settings.mySetting);
    }

    private void LoadSettings()
    {
        string settingsPath = Path.Combine(Application.dataPath, "EditorSettings.json");
        if (File.Exists(settingsPath)) {
            string json = File.ReadAllText(settingsPath);
            settings = JsonUtility.FromJson<MyEditorSettings>(json);
        }
        else {
            settings = new MyEditorSettings();
        }
    }
}
