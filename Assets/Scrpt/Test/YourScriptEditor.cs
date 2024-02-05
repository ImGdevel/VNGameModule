using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(YourScript))]
public class YourScriptEditor : Editor
{
    SerializedProperty gameObjectsList;

    void OnEnable()
    {
        gameObjectsList = serializedObject.FindProperty("gameObjectsList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // gameObjectsList 필드를 비활성화하여 추가를 막음
        GUI.enabled = false;
        EditorGUILayout.PropertyField(gameObjectsList, true);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();

        // 리스트에 요소 추가하는 버튼 생성
        if (GUILayout.Button("Add GameObject Entry")) {
            YourScript script = (YourScript)target;
            script.AddGameObjectEntry();
        }
    }
}