using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VNSceneEditor))]
public class CustomEditorToVNSceneEditor : Editor
{
    SerializedProperty gameObjectsList;

    void OnEnable()
    {
        gameObjectsList = serializedObject.FindProperty("sceneList");
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
        if (GUILayout.Button("Add New Scene")) {
            VNSceneEditor script = (VNSceneEditor)target;
            script.AddNewScene();
        }

        // 리스트에 요소 추가하는 버튼 생성
        if (GUILayout.Button("Clear Scene")) {
            VNSceneEditor script = (VNSceneEditor)target;
            script.ClearScene();
        }
    }
}