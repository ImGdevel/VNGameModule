using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterManagerWindow : EditorWindow
{
    public static void ShowWindow()
    {
        GetWindow<CharacterManagerWindow>("Character Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Manager", EditorStyles.boldLabel);

        // Add GUI elements to manage characters
        // Example: Add new character, list existing characters, etc.
    }
}
