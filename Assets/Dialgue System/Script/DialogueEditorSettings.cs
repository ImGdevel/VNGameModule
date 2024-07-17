#define MeetAndTalk

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace DialogueSystem.Settings
{
    public class DialogueEditorSettings : ScriptableObject
    {

        [SerializeField] public GameObject DialoguePrefab;
        [SerializeField] public EditorTheme Theme;
        // Auto Save Option
        [SerializeField] public bool AutoSave = true;
        [SerializeField] public float AutoSaveInterval = 15f;
        // Logs Option
        [SerializeField] public bool AutoSaveLogs = true;
        [SerializeField] public bool ManualSaveLogs = true;
        [SerializeField] public bool LoadLogs = true;
        // Validation
        [SerializeField] public bool ShowErrors = true;
        [SerializeField] public bool ShowWarnings = true;

        private static DialogueEditorSettings _instance;
        public static DialogueEditorSettings Instance
        {
            get { return _instance; }
        }

#if UNITY_EDITOR
        private static string scriptFilePath;

        public static string k_CampaingPath {
            get {
                if (string.IsNullOrEmpty(scriptFilePath)) {
                    scriptFilePath = GetScriptFilePath();
                }
                var path = Path.Combine(Path.GetDirectoryName(scriptFilePath), "../Resources/DialogueEditorSettings.asset");
                return Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "Assets");
            }
        }

        private static string GetScriptFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null)
        {
            return path;
        }

        internal static DialogueEditorSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<DialogueEditorSettings>(k_CampaingPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<DialogueEditorSettings>();


                AssetDatabase.CreateAsset(settings, k_CampaingPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif
    }

#if UNITY_EDITOR
    static class MeetAndTalkSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CampaignManagerProvider()
        {
            var provider = new SettingsProvider("Project/Dialogue System", SettingsScope.Project)
            {
                label = "Dialogue System",
                guiHandler = (searchContext) =>
                {
                    var settings = DialogueEditorSettings.GetSerializedSettings();

                    EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty("Theme"), new GUIContent("Editor Theme"));

                    EditorGUILayout.LabelField("Defualt Prefabs", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty("DialoguePrefab"), new GUIContent("Dialogue Prefab"));

                    EditorGUILayout.LabelField("Auto Save Option", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty("AutoSave"), new GUIContent("Auto Save"));
                    settings.FindProperty("AutoSaveInterval").floatValue = EditorGUILayout.Slider("Auto Save Interval", settings.FindProperty("AutoSaveInterval").floatValue, 5f, 60f);

                    EditorGUILayout.LabelField("Logs Options", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty("AutoSaveLogs"), new GUIContent("Auto Save Logs"));
                    EditorGUILayout.PropertyField(settings.FindProperty("ManualSaveLogs"), new GUIContent("Manual Save Logs"));
                    EditorGUILayout.PropertyField(settings.FindProperty("LoadLogs"), new GUIContent("Load Logs"));

                    EditorGUILayout.LabelField("Editor Validation", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty("ShowErrors"), new GUIContent("Show Errors"));
                    EditorGUILayout.PropertyField(settings.FindProperty("ShowWarnings"), new GUIContent("Show Warnings"));

                    settings.ApplyModifiedProperties();
                }
            };

            return provider;
        }
    }

    class DialogueSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
        }

        private static string scriptFilePath;

        public static string k_CampaingPath {
            get {
                if (string.IsNullOrEmpty(scriptFilePath)) {
                    scriptFilePath = GetScriptFilePath();
                }
                var path = Path.Combine(Path.GetDirectoryName(scriptFilePath), "../Resources/DialogueEditorSettings.asset");
                return Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "Assets");
            }
        }

        private static string GetScriptFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null)
        {
            return path;
        }

        public DialogueSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_CampaingPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CustomSettings = DialogueEditorSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
        }
    }


#endif

    public enum EditorTheme
    {
        Base = 0
    }
}