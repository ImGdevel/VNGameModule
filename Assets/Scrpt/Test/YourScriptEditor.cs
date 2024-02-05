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

        // gameObjectsList �ʵ带 ��Ȱ��ȭ�Ͽ� �߰��� ����
        GUI.enabled = false;
        EditorGUILayout.PropertyField(gameObjectsList, true);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();

        // ����Ʈ�� ��� �߰��ϴ� ��ư ����
        if (GUILayout.Button("Add GameObject Entry")) {
            YourScript script = (YourScript)target;
            script.AddGameObjectEntry();
        }
    }
}