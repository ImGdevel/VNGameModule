using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScenarioEditorWindow : EditorWindow
{
    public static void ShowWindow()
    {
        GetWindow<ScenarioEditorWindow>("Scenario Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenario Editor", EditorStyles.boldLabel);

        // Add GUI elements to create and manage scenarios
        // Example: Dialogue input, choices, etc.
    }
}