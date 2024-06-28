using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public class MyEditorWindow : EditorWindow
{
    private MyEditorSettings settings;
    private string settingsPath;

    [MenuItem("Window/My Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<MyEditorWindow>("My Editor Window");
    }

    private void OnEnable()
    {
        settingsPath = GetScriptDirectory() + "/MyEditorSettings.asset";
        LoadSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnGUI()
    {
        if (settings == null) {
            LoadSettings();
        }
        
        settings.mySetting = EditorGUILayout.TextField("My Setting", settings.mySetting);
        DrawToolbar();
        if (GUILayout.Button("Save Settings")) {
            SaveSettings();
        }
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Custom Button", EditorStyles.toolbarButton)) {
            Debug.Log("Custom Button Clicked");
        }

        GUILayout.EndHorizontal();
    }

    private void LoadSettings()
    {

        settings = AssetDatabase.LoadAssetAtPath<MyEditorSettings>(settingsPath);
        if (settings == null) {
            Debug.LogWarning("Settings file not found, creating default settings.");
            settings = ScriptableObject.CreateInstance<MyEditorSettings>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void SaveSettings()
    {
        if (settings != null) {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else {
            Debug.LogWarning("Settings object is null, cannot save settings.");
        }
    }

    private string GetScriptDirectory()
    {
        // 현재 스크립트의 경로를 가져옴
        MonoScript script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);
        return Path.GetDirectoryName(scriptPath);
    }

}

/*
 public class MyEditorWindow : EditorWindow
{
    private MyEditorSettings settings;
    private string settingsPath;

    [MenuItem("Window/My Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<MyEditorWindow>("My Editor Window");
    }

    private void OnEnable()
    {
        // 스크립트 파일 경로 설정
        settingsPath = GetScriptDirectory() + "/EditorSettings.json";
        LoadSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnGUI()
    {
        if (settings == null) {
            settings = new MyEditorSettings();
        }

        settings.mySetting = EditorGUILayout.TextField("My Setting", settings.mySetting);
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsPath)) {
            string json = File.ReadAllText(settingsPath);
            settings = JsonUtility.FromJson<MyEditorSettings>(json);
        }
        else {
            settings = new MyEditorSettings();
        }
        AssetDatabase.Refresh();
    }

    private void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsPath, json);
        File.SetLastWriteTime(settingsPath, DateTime.Now); // 타임스탬프 업데이트
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private string GetScriptDirectory()
    {
        // 현재 스크립트의 경로를 가져옴
        MonoScript script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);
        return Path.GetDirectoryName(scriptPath);
    }
}

 
 */