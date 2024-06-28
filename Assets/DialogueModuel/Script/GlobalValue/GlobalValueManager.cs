
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.UIElements;

namespace MeetAndTalk.GlobalValue
{
    public class GlobalValueManager : ScriptableObject
    {
        public const string k_LocalizationManagerPath = "Assets/Meet and Talk/Resources/GlobalValue.asset";

        private static GlobalValueManager _instance;
        public static GlobalValueManager Instance
        {
            get { return _instance; }
        }

        public List<GlobalValueInt> IntValues;
        public List<GlobalValueFloat> FloatValues;
        public List<GlobalValueBool> BoolValues;
        public List<GlobalValueString> StringValues;

        [HideInInspector] public bool intB = true, floatB = true, boolB = true, stringB = true;

#if UNITY_EDITOR
        internal static GlobalValueManager GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<GlobalValueManager>(k_LocalizationManagerPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<GlobalValueManager>();

                settings.IntValues = new List<GlobalValueInt>();
                settings.FloatValues = new List<GlobalValueFloat>();
                settings.BoolValues = new List<GlobalValueBool>();
                settings.StringValues = new List<GlobalValueString>();

                AssetDatabase.CreateAsset(settings, k_LocalizationManagerPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif
        #region Global Value Function

        public void LoadFile()
        {
            for (int i = 0; i < IntValues.Count; i++) { IntValues[i].Value = PlayerPrefs.GetInt($"Int_{IntValues[i].ValueName}", IntValues[i].BaseValue); }
            for (int i = 0; i < FloatValues.Count; i++) { FloatValues[i].Value = PlayerPrefs.GetFloat($"Float_{FloatValues[i].ValueName}", FloatValues[i].BaseValue); }
            for (int i = 0; i < BoolValues.Count; i++) { BoolValues[i].Value = PlayerPrefs.GetInt($"Bool_{BoolValues[i].ValueName}", (BoolValues[i].BaseValue) ? 1 : 0) == 1 ? true : false; }
            for (int i = 0; i < StringValues.Count; i++) { StringValues[i].Value = PlayerPrefs.GetString($"String_{StringValues[i].ValueName}", StringValues[i].BaseValue); }
        }
        public void SaveFile()
        {
            for (int i = 0; i < IntValues.Count; i++) { PlayerPrefs.SetInt($"Int_{IntValues[i].ValueName}", IntValues[i].Value); }
            for (int i = 0; i < FloatValues.Count; i++) { PlayerPrefs.SetFloat($"Float_{FloatValues[i].ValueName}", FloatValues[i].Value); }
            for (int i = 0; i < BoolValues.Count; i++) { PlayerPrefs.SetInt($"Bool_{BoolValues[i].ValueName}", BoolValues[i].Value ? 1 : 0); }
            for (int i = 0; i < StringValues.Count; i++) { PlayerPrefs.SetString($"String_{StringValues[i].ValueName}", StringValues[i].Value); }
        }

        public T Get<T>(GlobalValueType type, string name)
        {
            T tmp = default(T);

            if (type == GlobalValueType.Int)
            {
                for (int i = 0; i < IntValues.Count; i++)
                {
                    if (IntValues[i].ValueName == name) { tmp = (T)Convert.ChangeType(IntValues[i].Value, typeof(T)); }
                }
            }

            if (type == GlobalValueType.Float)
            {
                for (int i = 0; i < FloatValues.Count; i++)
                {
                    if (FloatValues[i].ValueName == name) { tmp = (T)Convert.ChangeType(FloatValues[i].Value, typeof(T)); }
                }
            }

            if (type == GlobalValueType.Bool)
            {
                for (int i = 0; i < BoolValues.Count; i++)
                {
                    if (BoolValues[i].ValueName == name) { tmp = (T)Convert.ChangeType(BoolValues[i].Value, typeof(T)); }
                }
            }

            if (type == GlobalValueType.String)
            {
                for (int i = 0; i < StringValues.Count; i++)
                {
                    if (StringValues[i].ValueName == name) { tmp = (T)Convert.ChangeType(StringValues[i].Value, typeof(T)); }
                }
            }

            return tmp;
        }

        public string Get(string name)
        {

            for (int i = 0; i < IntValues.Count; i++)
            {
                if (IntValues[i].ValueName == name) { return IntValues[i].Value.ToString(); }
            }

            for (int i = 0; i < FloatValues.Count; i++)
            {
                if (FloatValues[i].ValueName == name) { return FloatValues[i].Value.ToString(); }
            }

            for (int i = 0; i < BoolValues.Count; i++)
            {
                if (BoolValues[i].ValueName == name) { return BoolValues[i].Value.ToString(); }
            }

            for (int i = 0; i < StringValues.Count; i++)
            {
                if (StringValues[i].ValueName == name) { return StringValues[i].Value.ToString(); }
            }
            return "Empty Value";
        }

        public bool IfTrue(string valueName, GlobalValueIFOperations operations, string value)
        {
            for (int i = 0; i < IntValues.Count; i++)
            {
                if (IntValues[i].ValueName == valueName)
                {
                    if (operations == GlobalValueIFOperations.Equal) { return (IntValues[i].Value == (int)Convert.ChangeType(value, typeof(int))); }
                    if (operations == GlobalValueIFOperations.Lesser) { return (IntValues[i].Value < (int)Convert.ChangeType(value, typeof(int))); }
                    if (operations == GlobalValueIFOperations.Greater) { return (IntValues[i].Value > (int)Convert.ChangeType(value, typeof(int))); }
                    if (operations == GlobalValueIFOperations.LesserOrEqual) { return (IntValues[i].Value <= (int)Convert.ChangeType(value, typeof(int))); }
                    if (operations == GlobalValueIFOperations.GreaterOrEqual) { Debug.Log((int)Convert.ChangeType(IntValues[i].Value, typeof(int))); return (IntValues[i].Value >= (int)Convert.ChangeType(value, typeof(int))); }
                }
            }
            for (int i = 0; i < FloatValues.Count; i++)
            {
                if (FloatValues[i].ValueName == valueName)
                {
                    if (operations == GlobalValueIFOperations.Equal) { return (FloatValues[i].Value == (float)Convert.ChangeType(value, typeof(float))); }
                    if (operations == GlobalValueIFOperations.Lesser) { return (FloatValues[i].Value < (float)Convert.ChangeType(value, typeof(float))); }
                    if (operations == GlobalValueIFOperations.Greater) { return (FloatValues[i].Value > (float)Convert.ChangeType(value, typeof(float))); }
                    if (operations == GlobalValueIFOperations.LesserOrEqual) { return (FloatValues[i].Value <= (float)Convert.ChangeType(value, typeof(float))); }
                    if (operations == GlobalValueIFOperations.GreaterOrEqual) { return (FloatValues[i].Value >= (float)Convert.ChangeType(value, typeof(float))); }
                }
            }
            for (int i = 0; i < BoolValues.Count; i++)
            {
                if (BoolValues[i].ValueName == valueName)
                {
                    return BoolValues[i].Value;
                }
            }
            return false;
        }


        public void Set(string name, string value)
        {

            for (int i = 0; i < IntValues.Count; i++)
            {
                if (IntValues[i].ValueName == name) { IntValues[i].Value = (int)Convert.ChangeType(value, typeof(int)); }
            }

            for (int i = 0; i < FloatValues.Count; i++)
            {
                if (FloatValues[i].ValueName == name) { FloatValues[i].Value = (float)Convert.ChangeType(value, typeof(float)); }
            }

            for (int i = 0; i < BoolValues.Count; i++)
            {
                if (BoolValues[i].ValueName == name) { BoolValues[i].Value = (bool)Convert.ChangeType(value, typeof(bool)); }
            }

            for (int i = 0; i < StringValues.Count; i++)
            {
                if (StringValues[i].ValueName == name) { StringValues[i].Value = (string)Convert.ChangeType(value, typeof(string)); }
            }

            SaveFile();
        }

        public void Set(string name, GlobalValueOperations operators, string value)
        {
            for (int i = 0; i < IntValues.Count; i++)
            {
                if (IntValues[i].ValueName == name) 
                { 
                    int typeValue = (int)Convert.ChangeType(value, typeof(int));
                    string newValue = "";

                    if(operators == GlobalValueOperations.Add) { newValue = ((int)(IntValues[i].Value + typeValue)).ToString(); }
                    if(operators == GlobalValueOperations.Subtract) { newValue = ((int)(IntValues[i].Value - typeValue)).ToString(); }
                    if(operators == GlobalValueOperations.Multiply) { newValue = ((int)(IntValues[i].Value * typeValue)).ToString(); }
                    if(operators == GlobalValueOperations.Divide) { newValue = ((int)(IntValues[i].Value / typeValue)).ToString(); }
                    if(operators == GlobalValueOperations.Set) { newValue = ((int)(typeValue)).ToString(); }

                    Set(IntValues[i].ValueName, newValue);
                }
            }

            for (int i = 0; i < FloatValues.Count; i++)
            {
                if (FloatValues[i].ValueName == name)
                {
                    float typeValue = (float)Convert.ChangeType(value, typeof(float));
                    string newValue = "";

                    if (operators == GlobalValueOperations.Add) { newValue = ((float)(FloatValues[i].Value + typeValue)).ToString(); }
                    if (operators == GlobalValueOperations.Subtract) { newValue = ((float)(FloatValues[i].Value - typeValue)).ToString(); }
                    if (operators == GlobalValueOperations.Multiply) { newValue = ((float)(FloatValues[i].Value * typeValue)).ToString(); }
                    if (operators == GlobalValueOperations.Divide) { newValue = ((float)(FloatValues[i].Value / typeValue)).ToString(); }
                    if (operators == GlobalValueOperations.Set) { newValue = ((float)(typeValue)).ToString(); }

                    Set(FloatValues[i].ValueName, newValue);
                }
            }

            for (int i = 0; i < BoolValues.Count; i++)
            {
                if (BoolValues[i].ValueName == name)
                {
                    bool typeValue = false;
                    if (value == "True" || value == "true" || value == "T" || value == "t" || value == "1") { typeValue = true; }
                    string newValue = (string)Convert.ChangeType(typeValue, typeof(string));

                    Set(BoolValues[i].ValueName, newValue);
                }
            }
        }

        #endregion
    }

#if UNITY_EDITOR
    static class GlobalValueManagerIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider GlobalValueManagerProvider()
        {
            var provider = new SettingsProvider("Project/Meet and Talk/Global Value", SettingsScope.Project)
            {
                label = "Glabal Value Manager",
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.HelpBox("Tu dam jaki?opis", MessageType.Info, true);
                    var settings = GlobalValueManager.GetSerializedSettings();

                    ShowArray(settings, "IntValues", "Int");
                    ShowArray(settings, "FloatValues", "Float");
                    ShowArray(settings, "BoolValues", "Bool");
                    ShowArray(settings, "StringValues", "String");

                    if (GUI.changed)
                    {
                        // Jeœli tak, zastosuj zmiany tylko do IntValues
                        settings.ApplyModifiedPropertiesWithoutUndo();
                        EditorUtility.SetDirty(settings.targetObject);
                    }
                },
                keywords = new HashSet<string>(new[] { "Global Value", "Value", "Global", "Dialogue Value" })
            };

            return provider;
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

                if(i < count - 1)
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
    class GlobalValueManagerProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            public static GUIContent IntValues = new GUIContent("IntValues");
            public static GUIContent FloatValues = new GUIContent("FloatValues");
            public static GUIContent BoolValues = new GUIContent("BoolValues");
            public static GUIContent StringValues = new GUIContent("StringValues");
        }

        const string k_LocalizationManagerPath = "Assets/Meet and Talk/Resources/GlobalValue.asset";
        public GlobalValueManagerProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_LocalizationManagerPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CustomSettings = GlobalValueManager.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("IntValues"), Styles.IntValues);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("FloatValues"), Styles.FloatValues);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("BoolValues"), Styles.BoolValues);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("StringValues"), Styles.StringValues);
        }
    }
#endif

    #region Value Class

    [System.Serializable]
    public enum GlobalValueType
    {
        Int = 0, Float = 1, Bool = 2, String = 3
    }

    public enum GlobalValueOperations
    {
        Add = 0, Subtract = 1, Multiply = 2, Divide = 3, Set = 4
    }
    public enum GlobalValueIFOperations
    {
        Equal = 0, Lesser = 1, Greater = 2, LesserOrEqual = 3, GreaterOrEqual = 4
    }

    [System.Serializable]
    public class GlobalValueInt
    {
        public string ValueName;
        public int Value;
        public int BaseValue;
    }

    [System.Serializable]
    public class GlobalValueString
    {
        public string ValueName;
        public string Value;
        public string BaseValue;
    }

    [System.Serializable]
    public class GlobalValueFloat
    {
        public string ValueName;
        public float Value;
        public float BaseValue;
    }

    [System.Serializable]
    public class GlobalValueBool
    {
        public string ValueName;
        public bool Value;
        public bool BaseValue;
    }

    [System.Serializable]
    public class GlobalValueClass
    {
        public string ValueName;
    }

    [System.Serializable]
    public class GlobalValueOperationClass
    {
        public string ValueName;
        public GlobalValueOperations Operation;
        public string OperationValue;
    }
    #endregion
    #region Custom Property Drawer

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GlobalValueOperationClass))]
    public class GlobalValueOperationClassDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            /* Load Values */
            List<string> tmp = new List<string>();
            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();
            for (int i = 0; i < manager.IntValues.Count; i++) { tmp.Add(manager.IntValues[i].ValueName); }
            for (int i = 0; i < manager.FloatValues.Count; i++) { tmp.Add(manager.FloatValues[i].ValueName); }
            for (int i = 0; i < manager.BoolValues.Count; i++) { tmp.Add(manager.BoolValues[i].ValueName); }

            /* Load Index */
            int index = 0;
            for(int i = 0; i < tmp.Count; i++) {  if (tmp[i] == property.FindPropertyRelative("ValueName").stringValue) { index = i; } }

            Rect valueNameRect = new Rect(position.x, position.y, position.width * 0.4f, EditorGUIUtility.singleLineHeight);
            Rect operationRect = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.4f, EditorGUIUtility.singleLineHeight);
            Rect operationValueRect = new Rect(position.x + position.width * 0.8f, position.y, position.width * 0.2f, EditorGUIUtility.singleLineHeight);

            //SerializedProperty valueNameProperty = property.FindPropertyRelative("ValueName");
            SerializedProperty operationProperty = property.FindPropertyRelative("Operation");
            SerializedProperty operationValueProperty = property.FindPropertyRelative("OperationValue");

            index = EditorGUI.Popup(valueNameRect, index, tmp.ToArray());
            property.FindPropertyRelative("ValueName").stringValue = tmp[index];
            EditorGUI.PropertyField(operationRect, operationProperty, GUIContent.none);
            EditorGUI.PropertyField(operationValueRect, operationValueProperty, GUIContent.none);

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(GlobalValueClass))]
    public class GlobalValueClassDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //float originalLabelWidth = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth = 50;

            /* Load Values */
            List<string> tmp = new List<string>();
            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();
            for (int i = 0; i < manager.StringValues.Count; i++) { tmp.Add(manager.StringValues[i].ValueName); }

            /* Load Index */
            int index = 0;
            for (int i = 0; i < tmp.Count; i++) { if (tmp[i] == property.FindPropertyRelative("ValueName").stringValue) { index = i; } }

            Rect valueNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            index = EditorGUI.Popup(valueNameRect, label.text, index, tmp.ToArray());
            property.FindPropertyRelative("ValueName").stringValue = tmp[index];
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(GlobalValueInt))]
    public class GlobalValueIntDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + 2;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float originalLabelWidth = EditorGUIUtility.labelWidth;

            Rect valueNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);
            Rect BaseValueRect = new Rect(position.x + position.width * 0.5f, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);

            SerializedProperty valueNameProperty = property.FindPropertyRelative("ValueName");
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            SerializedProperty BaseValueProperty = property.FindPropertyRelative("BaseValue");

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueNameRect, valueNameProperty, new GUIContent("Name:"));
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent("Value:"));
            EditorGUI.PropertyField(BaseValueRect, BaseValueProperty, new GUIContent(" Defualt:"));

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(GlobalValueFloat))]
    public class GlobalValueFloatDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float originalLabelWidth = EditorGUIUtility.labelWidth;

            Rect valueNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);
            Rect BaseValueRect = new Rect(position.x + position.width * 0.5f, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);

            SerializedProperty valueNameProperty = property.FindPropertyRelative("ValueName");
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            SerializedProperty BaseValueProperty = property.FindPropertyRelative("BaseValue");

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueNameRect, valueNameProperty, new GUIContent("Name:"));
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent("Value:"));
            EditorGUI.PropertyField(BaseValueRect, BaseValueProperty, new GUIContent(" Defualt:"));

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(GlobalValueBool))]
    public class GlobalValueBoolDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float originalLabelWidth = EditorGUIUtility.labelWidth;

            Rect valueNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);
            Rect BaseValueRect = new Rect(position.x + position.width * 0.5f, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);

            SerializedProperty valueNameProperty = property.FindPropertyRelative("ValueName");
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            SerializedProperty BaseValueProperty = property.FindPropertyRelative("BaseValue");

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueNameRect, valueNameProperty, new GUIContent("Name:"));
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent("Value:"));
            EditorGUI.PropertyField(BaseValueRect, BaseValueProperty, new GUIContent(" Defualt:"));

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(GlobalValueString))]
    public class GlobalValueStringDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float originalLabelWidth = EditorGUIUtility.labelWidth;

            Rect valueNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);
            Rect BaseValueRect = new Rect(position.x + position.width * 0.5f, position.y + EditorGUIUtility.singleLineHeight + 2, position.width * 0.5f, EditorGUIUtility.singleLineHeight);

            SerializedProperty valueNameProperty = property.FindPropertyRelative("ValueName");
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            SerializedProperty BaseValueProperty = property.FindPropertyRelative("BaseValue");

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueNameRect, valueNameProperty, new GUIContent("Name:"));
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent("Value:"));
            EditorGUI.PropertyField(BaseValueRect, BaseValueProperty, new GUIContent(" Defualt:"));

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.EndProperty();
        }
    }
#endif
#endregion
}
