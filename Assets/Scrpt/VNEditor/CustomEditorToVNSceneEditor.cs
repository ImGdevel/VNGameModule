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

        // gameObjectsList �ʵ带 ��Ȱ��ȭ�Ͽ� �߰��� ����
        GUI.enabled = false;
        EditorGUILayout.PropertyField(gameObjectsList, true);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();

        // ����Ʈ�� ��� �߰��ϴ� ��ư ����
        if (GUILayout.Button("Add New Scene")) {
            VNSceneEditor script = (VNSceneEditor)target;
            script.AddNewScene();
        }

        // ����Ʈ�� ��� �߰��ϴ� ��ư ����
        if (GUILayout.Button("Clear Scene")) {
            VNSceneEditor script = (VNSceneEditor)target;
            script.ClearScene();
        }
    }
}