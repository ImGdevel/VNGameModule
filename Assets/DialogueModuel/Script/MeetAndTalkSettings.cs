#define MeetAndTalk

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace MeetAndTalk.Settings
{
    public class MeetAndTalkSettings : ScriptableObject
    {
        public const string k_CampaingPath = "Assets/Meet and Talk/Resources/MeetAndTalkSettings.asset";

        [SerializeField] public GameObject DialoguePrefab;
        [SerializeField] public MeetAndTalkTheme Theme;
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

        private static MeetAndTalkSettings _instance;
        public static MeetAndTalkSettings Instance
        {
            get { return _instance; }
        }

#if UNITY_EDITOR
        internal static MeetAndTalkSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MeetAndTalkSettings>(k_CampaingPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<MeetAndTalkSettings>();


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
            var provider = new SettingsProvider("Project/Meet and Talk", SettingsScope.Project)
            {
                label = "Meet and Talk",
                guiHandler = (searchContext) =>
                {
                    var settings = MeetAndTalkSettings.GetSerializedSettings();

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

    class MeetAndTalkSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            //public static GUIContent chapter = new GUIContent("chapter");
            //public static GUIContent localization = new GUIContent("localization");
            //public static GUIContent selectedLang = new GUIContent("selectedLang");
        }

        const string k_CampaingPath = "Assets/Meet and Talk/Resources/MeetAndTalkSettings.asset";
        public MeetAndTalkSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_CampaingPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CustomSettings = MeetAndTalkSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("chapter"), Styles.chapter);
            //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("localization"), Styles.localization);
            //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("selectedLang"), Styles.selectedLang);
        }
    }


#endif

    public enum MeetAndTalkTheme
    {
        Dark = 0, PureDark = 1,
        //Light = 2, PureLight = 3
    }
}