using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if HellishBattle
/// If Hellish Battle Installed
#else
namespace MeetAndTalk.Localization
{
    public class LocalizationManager : ScriptableObject
    {
        public const string k_LocalizationManagerPath = "Assets/Meet and Talk/Resources/Languages.asset";

        private static MeetAndTalk.Localization.LocalizationManager _instance;
        public static MeetAndTalk.Localization.LocalizationManager Instance
        {
            get { return _instance; }
        }

        [SerializeField]
        public List<SystemLanguage> lang = new List<SystemLanguage>();
        [SerializeField]
        public SystemLanguage selectedLang = SystemLanguage.English;

#if UNITY_EDITOR
        internal static MeetAndTalk.Localization.LocalizationManager GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MeetAndTalk.Localization.LocalizationManager>(k_LocalizationManagerPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<MeetAndTalk.Localization.LocalizationManager>();

                settings.lang = new List<SystemLanguage>() { SystemLanguage.Polish, SystemLanguage.Spanish };
                settings.selectedLang = SystemLanguage.English;

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

        public void SaveLocalization(SystemLanguage lang)
        {
            selectedLang = lang;
            PlayerPrefs.SetInt("SelectedLocalization", (int)lang);
        }
        public void LoadLocalization()
        {
            int _lang = PlayerPrefs.GetInt("SelectedLocalization");
            selectedLang = (SystemLanguage)_lang;
        }

        public string LocalizationLocalizationName(SystemLanguage lang)
        {
            switch (lang)
            {
                case SystemLanguage.English:
                    return "English";
                case SystemLanguage.Japanese:
                    return "日本語";
                case SystemLanguage.ChineseSimplified:
                    return "中国语 简体字";
                case SystemLanguage.ChineseTraditional:
                    return "中国语 繁體字";
                case SystemLanguage.Chinese:
                    return "中国语";
                case SystemLanguage.Afrikaans:
                    return "Afrikane";
                case SystemLanguage.Arabic:
                    return "عربى";
                case SystemLanguage.Basque:
                    return "Euskara";
                case SystemLanguage.Belarusian:
                    return "Беларуская";
                case SystemLanguage.Bulgarian:
                    return "български";
                case SystemLanguage.Catalan:
                    return "Català";
                case SystemLanguage.Czech:
                    return "Český jazyk";
                case SystemLanguage.Danish:
                    return "Dansk";
                case SystemLanguage.Dutch:
                    return "Nederlands";
                case SystemLanguage.Estonian:
                    return "Eestlane";
                case SystemLanguage.Faroese:
                    return "Føroyskt";
                case SystemLanguage.Finnish:
                    return "Suomi";
                case SystemLanguage.French:
                    return "Français";
                case SystemLanguage.German:
                    return "Deutsch";
                case SystemLanguage.Greek:
                    return "Ελληνικά";
                case SystemLanguage.Hebrew:
                    return "עברית";
                case SystemLanguage.Hungarian:
                    return "Magyar nyelv";
                case SystemLanguage.Icelandic:
                    return "íslenska";
                case SystemLanguage.Indonesian:
                    return "Bahasa Indonesia";
                case SystemLanguage.Italian:
                    return "Italiano";
                case SystemLanguage.Korean:
                    return "조선말";
                case SystemLanguage.Latvian:
                    return "Latviešu";
                case SystemLanguage.Lithuanian:
                    return "lietuvių kalba";
                case SystemLanguage.Norwegian:
                    return "Norsk";
                case SystemLanguage.Polish:
                    return "Polski";
                case SystemLanguage.Portuguese:
                    return "Português";
                case SystemLanguage.Romanian:
                    return "Limba română";
                case SystemLanguage.Russian:
                    return "Pусский язык";
                case SystemLanguage.SerboCroatian:
                    return "Cрпскохрватски";
                case SystemLanguage.Slovak:
                    return "Slovenčina";
                case SystemLanguage.Slovenian:
                    return "Slovenščina";
                case SystemLanguage.Spanish:
                    return "Español";
                case SystemLanguage.Swedish:
                    return "Svenska";
                case SystemLanguage.Thai:
                    return "ภาษาไทย";
                case SystemLanguage.Turkish:
                    return "Türkçe";
                case SystemLanguage.Ukrainian:
                    return "Yкраїнська мова";
                case SystemLanguage.Vietnamese:
                    return "Tiếng Việt";
                case SystemLanguage.Unknown:
                default:
                    return lang.ToString();
            }
        }

        public LocalizationEnum SelectedLang()
        {
            for (int i = 0; i < lang.Count; i++)
            {
                if (lang[i].ToString() == selectedLang.ToString())
                {
                    return (LocalizationEnum)(i + 1);
                }
            }
            return (LocalizationEnum)0;
        }
    }

#if UNITY_EDITOR
    static class LocalizationManagerIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider LocalizationManagerProvider()
        {
            var provider = new SettingsProvider("Project/Meet and Talk/Localization", SettingsScope.Project)
            {
                label = "Localization",
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.HelpBox("    English is the primary language and you don't need to add it as an additional language", MessageType.Info, true);
                    var settings = MeetAndTalk.Localization.LocalizationManager.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("lang"), new GUIContent("Available Language"));
                    EditorGUILayout.PropertyField(settings.FindProperty("selectedLang"), new GUIContent("Base Language"));
                //EditorGUILayout.LabelField("Language");


                /// Generate Localization Enum
                if (GUILayout.Button("Generate C# Enum"))
                    {
                        List<string> enumEntries = new List<string>();
                        enumEntries.Add(SystemLanguage.English.ToString());
                        MeetAndTalk.Localization.LocalizationManager tmp = Resources.Load<MeetAndTalk.Localization.LocalizationManager>("Languages");
                        for (int i = 0; i < tmp.lang.Count; i++)
                        {
                            enumEntries.Add(tmp.lang[i].ToString());
                        }
                        string filePathAndName = "Assets/Meet and Talk/Resources/LocalizationEnum.cs";

                        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
                        {
                            streamWriter.WriteLine("namespace MeetAndTalk.Localization");
                            streamWriter.WriteLine("{");
                            streamWriter.WriteLine("public enum LocalizationEnum");
                            streamWriter.WriteLine("{");
                            for (int i = 0; i < enumEntries.Count; i++)
                            {
                                streamWriter.WriteLine("\t" + enumEntries[i] + ",");
                            }
                            streamWriter.WriteLine("}");
                            streamWriter.WriteLine("}");
                        }
                        AssetDatabase.Refresh();
                    }


                    settings.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = new HashSet<string>(new[] { "Language" })
            };

            return provider;
        }
    }
    class LocalizationManagerProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            public static GUIContent lang = new GUIContent("lang");
            public static GUIContent selectedLang = new GUIContent("selectedLang");
        }

        const string k_LocalizationManagerPath = "Assets/Meet and Talk/Resources/Languages.asset";
        public LocalizationManagerProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_LocalizationManagerPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CustomSettings = MeetAndTalk.Localization.LocalizationManager.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("lang"), Styles.lang);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("selectedLang"), Styles.selectedLang);
        }
    }
#endif

    [System.Serializable]
    public class StringLocalizationList
    {
        public string key = "";
        public string englishText = "";
        public List<string> stringList = new List<string>();
    }

    [System.Serializable]
    public class AudioLocalizationList
    {
        public string key = "";
        public AudioClip englishText;
        public List<AudioClip> audioList = new List<AudioClip>();
    }

    [System.Serializable]
    public class TextureLocalizationList
    {
        public string key = "";
        public Texture2D englishText;
        public List<Texture2D> textureList = new List<Texture2D>();
    }

    [System.Serializable]
    public class SpriteLocalizationList
    {
        public string key = "";
        public Sprite englishText;
        public List<Sprite> spriteList = new List<Sprite>();
    }

}
#endif