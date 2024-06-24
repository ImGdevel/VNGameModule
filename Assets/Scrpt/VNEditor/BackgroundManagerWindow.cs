using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BackgroundManagerWindow : EditorWindow
{
    public static void ShowWindow()
    {
        GetWindow<BackgroundManagerWindow>("Background Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Background Manager", EditorStyles.boldLabel);

        // Add GUI elements to manage backgrounds
        // Example: Add new background, list existing backgrounds, etc.
    }
}
