using UnityEngine;
using UnityEditor;
using DialogueSystem;
using DialogueSystem.GlobalValue;
using DialogueSystem.Event;
using DialogueSystem.Localization;

#if UNITY_EDITOR

[CustomEditor(typeof(DialogueScript))]
public class DialogueContainerSOEditor : Editor
{
    bool NodeLink = false;
    bool StartNode = false;
    bool EndNode = false;
    bool DialogueNode = false;
    bool DialogueChoiceNode = false;
    bool DialogueTimerChoiceNode = false;
    bool DialogueEventNode = false;
    bool RandomNode = false;
    bool IFNode = false;

    public override void OnInspectorGUI()
    {
        EditorUtility.SetDirty(target);
        DialogueScript _target = (DialogueScript)target;

        // Base Info
        EditorGUI.indentLevel = 0;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_target.name, EditorStyles.boldLabel);

        if (GUILayout.Button("Import File", EditorStyles.miniButtonLeft))
        {
            string path = EditorUtility.OpenFilePanel("Import Dialogue Localization File", Application.dataPath, "csv");
            if (path.Length != 0)
            {
                _target.ImportText(path, _target);
                serializedObject.ApplyModifiedProperties();
            }
        }
        if (GUILayout.Button("Export File", EditorStyles.miniButtonRight))
        {
            string path = EditorUtility.SaveFilePanel("Export Dialogue Localization File", Application.dataPath, _target.name, "csv");
            if (path.Length != 0)
            {
                _target.GenerateCSV(path, _target);
                serializedObject.ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Connection Between Nodes", EditorStyles.boldLabel);

        #region Node Link
        EditorGUILayout.BeginVertical("HelpBox");
        int count = _target.NodeLinkDatas.Count;

        // Foldout
        Rect rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        GUIContent foldoutContent = new GUIContent($"Node Link [{count}]");
        NodeLink = EditorGUILayout.Foldout(NodeLink, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (NodeLink)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Node Link Between", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                EditorGUILayout.TextField("Base Node", _target.NodeLinkDatas[i].BaseNodeGuid);
                EditorGUILayout.TextField("Target Node", _target.NodeLinkDatas[i].TargetNodeGuid);
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

        #region Start Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.StartNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"Start Node [{count}]");
        StartNode = EditorGUILayout.Foldout(StartNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (StartNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.StartNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                _target.StartNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.StartNodeDatas[i].Position);
                _target.StartNodeDatas[i].startID = EditorGUILayout.TextField("ID:", _target.StartNodeDatas[i].startID);
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region End Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.EndNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"End Node [{count}]");
        EndNode = EditorGUILayout.Foldout(EndNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (EndNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.EndNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.EndNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.EndNodeDatas[i].Position);
                _target.EndNodeDatas[i].EndNodeType = (EndNodeType)EditorGUILayout.EnumPopup("End Enum", _target.EndNodeDatas[i].EndNodeType);
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Dialogue Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.DialogueNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"Dialogue Node [{count}]");
        DialogueNode = EditorGUILayout.Foldout(DialogueNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (DialogueNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.DialogueNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.DialogueNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.DialogueNodeDatas[i].Position);
                _target.DialogueNodeDatas[i].Character = (DialogueCharacter)EditorGUILayout.ObjectField("Character", _target.DialogueNodeDatas[i].Character, typeof(DialogueCharacter), false);
                _target.DialogueNodeDatas[i].CharacterPos = (CharacterPosition)EditorGUILayout.EnumPopup("Avatar Display", _target.DialogueNodeDatas[i].CharacterPos);
                _target.DialogueNodeDatas[i].AvatarType = (CharacterType)EditorGUILayout.EnumPopup("Avatar Emotion", _target.DialogueNodeDatas[i].AvatarType);

                _target.DialogueNodeDatas[i].Duration = EditorGUILayout.FloatField("Display Time", _target.DialogueNodeDatas[i].Duration);

                for (int j = 0; j < _target.DialogueNodeDatas[0].TextType.Count; j++)
                {
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField(_target.DialogueNodeDatas[0].TextType[j].languageEnum.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();
                    _target.DialogueNodeDatas[i].AudioClips[j].LanguageGenericType = (AudioClip)EditorGUILayout.ObjectField("Audio Clips", _target.DialogueNodeDatas[i].AudioClips[j].LanguageGenericType, typeof(AudioClip), false);
                    _target.DialogueNodeDatas[i].TextType[j].LanguageGenericType = EditorGUILayout.TextField("Displayed String", _target.DialogueNodeDatas[i].TextType[j].LanguageGenericType);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Choice Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.DialogueChoiceNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"Choice Node [{count}]");
        DialogueChoiceNode = EditorGUILayout.Foldout(DialogueChoiceNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (DialogueChoiceNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.DialogueChoiceNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.DialogueChoiceNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.DialogueChoiceNodeDatas[i].Position);
                _target.DialogueChoiceNodeDatas[i].Character = (DialogueCharacter)EditorGUILayout.ObjectField("Character", _target.DialogueChoiceNodeDatas[i].Character, typeof(DialogueCharacter), false);
                _target.DialogueChoiceNodeDatas[i].AvatarPos = (CharacterPosition)EditorGUILayout.EnumPopup("Avatar Display", _target.DialogueChoiceNodeDatas[i].AvatarPos);
                _target.DialogueChoiceNodeDatas[i].AvatarType = (CharacterType)EditorGUILayout.EnumPopup("Avatar Emotion", _target.DialogueChoiceNodeDatas[i].AvatarType);
                _target.DialogueChoiceNodeDatas[i].Duration = EditorGUILayout.FloatField("Display Time", _target.DialogueChoiceNodeDatas[i].Duration);

                for (int j = 0; j < _target.DialogueChoiceNodeDatas[0].TextType.Count; j++)
                {
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField(_target.DialogueChoiceNodeDatas[0].TextType[j].languageEnum.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();
                    _target.DialogueChoiceNodeDatas[i].AudioClips[j].LanguageGenericType = (AudioClip)EditorGUILayout.ObjectField("Audio Clips", _target.DialogueChoiceNodeDatas[i].AudioClips[j].LanguageGenericType, typeof(AudioClip), false);
                    _target.DialogueChoiceNodeDatas[i].TextType[j].LanguageGenericType = EditorGUILayout.TextField("Displayed String", _target.DialogueChoiceNodeDatas[i].TextType[j].LanguageGenericType);
                    EditorGUILayout.LabelField("Options: ", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    for (int x = 0; x < _target.DialogueChoiceNodeDatas[i].DialogueNodePorts.Count; x++)
                    {
                        _target.DialogueChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType = EditorGUILayout.TextField($"Option_{x + 1}", _target.DialogueChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Timer Choice Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.TimerChoiceNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"Timer Choice Node [{count}]");
        DialogueTimerChoiceNode = EditorGUILayout.Foldout(DialogueTimerChoiceNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (DialogueTimerChoiceNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.TimerChoiceNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.TimerChoiceNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.TimerChoiceNodeDatas[i].Position);
                _target.TimerChoiceNodeDatas[i].Character = (DialogueCharacter)EditorGUILayout.ObjectField("Character", _target.TimerChoiceNodeDatas[i].Character, typeof(DialogueCharacter), false);
                _target.TimerChoiceNodeDatas[i].AvatarPos = (CharacterPosition)EditorGUILayout.EnumPopup("Avatar Display", _target.TimerChoiceNodeDatas[i].AvatarPos);
                _target.TimerChoiceNodeDatas[i].AvatarType = (CharacterType)EditorGUILayout.EnumPopup("Avatar Emotion", _target.TimerChoiceNodeDatas[i].AvatarType);
                _target.TimerChoiceNodeDatas[i].Duration = EditorGUILayout.FloatField("Display Time", _target.TimerChoiceNodeDatas[i].Duration);
                _target.TimerChoiceNodeDatas[i].time = EditorGUILayout.FloatField("Time to make decision", _target.TimerChoiceNodeDatas[i].time);

                for (int j = 0; j < _target.TimerChoiceNodeDatas[0].TextType.Count; j++)
                {
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField(_target.TimerChoiceNodeDatas[0].TextType[j].languageEnum.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();
                    _target.TimerChoiceNodeDatas[i].AudioClips[j].LanguageGenericType = (AudioClip)EditorGUILayout.ObjectField("Audio Clips", _target.TimerChoiceNodeDatas[i].AudioClips[j].LanguageGenericType, typeof(AudioClip), false);
                    _target.TimerChoiceNodeDatas[i].TextType[j].LanguageGenericType = EditorGUILayout.TextField("Displayed String", _target.TimerChoiceNodeDatas[i].TextType[j].LanguageGenericType);
                    EditorGUILayout.LabelField("Options: ", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    for (int x = 1; x < _target.TimerChoiceNodeDatas[i].DialogueNodePorts.Count; x++)
                    {
                        _target.TimerChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType = EditorGUILayout.TextField($"Option_{x}", _target.TimerChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Event Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.EventNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"Event Node [{count}]");
        DialogueEventNode = EditorGUILayout.Foldout(DialogueEventNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (DialogueEventNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.EventNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.EventNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.EventNodeDatas[i].Position);
                EditorGUILayout.LabelField("Events: ", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int x = 0; x < _target.EventNodeDatas[i].EventScriptableObjects.Count; x++)
                {
                    _target.EventNodeDatas[i].EventScriptableObjects[x].DialogueEvent = (DialogueEvent)EditorGUILayout.ObjectField($"Event_{x}", _target.EventNodeDatas[i].EventScriptableObjects[x].DialogueEvent, typeof(DialogueEvent), false);
                }
                EditorGUI.indentLevel--;
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Random Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.RandomNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"Random Node [{count}]");
        RandomNode = EditorGUILayout.Foldout(RandomNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (RandomNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.RandomNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.RandomNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.RandomNodeDatas[i].Position);

                EditorGUILayout.LabelField("Ports: ", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int x = 0; x < _target.RandomNodeDatas[i].DialogueNodePorts.Count; x++)
                {
                    _target.RandomNodeDatas[i].DialogueNodePorts[x].InputGuid = EditorGUILayout.TextField($"Port {x + 1}", _target.RandomNodeDatas[i].DialogueNodePorts[x].InputGuid);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region IF Node
        EditorGUILayout.BeginVertical("HelpBox");
        count = _target.IfNodeDatas.Count;

        // Foldout
        rect = EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;
        foldoutContent = new GUIContent($"IF Node [{count}]");
        IFNode = EditorGUILayout.Foldout(IFNode, foldoutContent, true);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        if (IFNode)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            // List
            for (int i = 0; i < count; i++)
            {
                int index = i;
                EditorGUILayout.BeginHorizontal();
                // Display Node
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("ID: " + _target.IfNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                _target.IfNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.IfNodeDatas[i].Position);
                _target.IfNodeDatas[i].ValueName = EditorGUILayout.TextField("Global Value Name", _target.IfNodeDatas[i].ValueName);
                _target.IfNodeDatas[i].Operations = (GlobalValueIFOperations)EditorGUILayout.EnumPopup("Operation", _target.IfNodeDatas[i].Operations);
                _target.IfNodeDatas[i].OperationValue = EditorGUILayout.TextField("Operation Value", _target.IfNodeDatas[i].OperationValue);

                EditorGUILayout.EndVertical();
                // Display Node
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(DialogueCharacter))]
public class DialogueCharacterSOInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DialogueCharacter character = (DialogueCharacter)target;

        character.Validate();

        // Name
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField($"Character Name Settings");
        character.UseGlobalValue = EditorGUILayout.Toggle(" Use Global Value as Name", character.UseGlobalValue);
        EditorGUILayout.EndHorizontal();
        // Code Here
        if (character.UseGlobalValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CustomizedName"), new GUIContent(" Dynamic Character Name"));
        }
        else
        {

            for (int i = 0; i < character.characterName.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                character.characterName[i].LanguageGenericType = EditorGUILayout.TextField($" {character.characterName[i].languageEnum} Name", character.characterName[i].LanguageGenericType);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("textColor"), new GUIContent("Character Text Color"), true);
        EditorGUILayout.EndVertical();

        // Name
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField($"Character Sprite Settings");
        EditorGUILayout.EndVertical();
        // Code Here

        for (int i = 0; i < character.Avatars.Count; i++)
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75;

            EditorGUILayout.LabelField(" " + character.Avatars[i].type.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            character.Avatars[i].Sprite = (Sprite)EditorGUILayout.ObjectField($"  Sprite", character.Avatars[i].Sprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.EndVertical();


        serializedObject.ApplyModifiedProperties();
        // Add this line to mark the target object as dirty and ensure changes are saved
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    #region Custom Drawer
    public static bool ShowArray(SerializedObject serializedObject, string PropertyName, string objectName)
    {
        EditorGUILayout.BeginVertical("HelpBox");

        SerializedProperty property = serializedObject.FindProperty(PropertyName);
        int count = property.arraySize;

        // Foldout
        Rect rect = EditorGUILayout.BeginVertical("HelpBox");
        GUIContent foldoutContent = new GUIContent($"{objectName} [{count}]");
        EditorGUILayout.LabelField($"List of {objectName} Value");
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("HelpBox");

        // List
        for (int i = 0; i < count; i++)
        {
            int index = i;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), GUIContent.none);
            if (IconButton("d_P4_DeletedLocal", "", GUILayout.Width(EditorGUIUtility.singleLineHeight * 2f + 2), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f + 2)))
            {
                property.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (i < count - 1)
            {
                EditorGUILayout.Separator();
            }
        }


        EditorGUILayout.EndVertical(); // End of List Vertical

        // Button
        if (IconButton("Toolbar Plus", $"Add New {objectName}"))
        {
            property.arraySize++;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical(); // End of Main Vertical

        return true;
    }
    public static bool IconButton(string iconName, string text, params GUILayoutOption[] options)
    {
        Texture icon = EditorGUIUtility.IconContent(iconName).image;
        GUIContent content = new GUIContent(text, icon);

        return GUILayout.Button(content, options);
    }
    #endregion
}

#endif

