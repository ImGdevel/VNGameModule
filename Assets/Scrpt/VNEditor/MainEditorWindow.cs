using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MainEditorWindow : EditorWindow
{
    [MenuItem("Window/Visual Novel Editor")]
    public static void ShowWindow()
    {
        GetWindow<MainEditorWindow>("Visual Novel Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Visual Novel Editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Scenario Editor")) {
            ScenarioEditorWindow.ShowWindow();
        }

        if (GUILayout.Button("Open Character Manager")) {
            CharacterManagerWindow.ShowWindow();
        }

        if (GUILayout.Button("Open Background Manager")) {
            BackgroundManagerWindow.ShowWindow();
        }

        if (GUILayout.Button("Preview Game")) {
            // Preview functionality
        }
    }
}
