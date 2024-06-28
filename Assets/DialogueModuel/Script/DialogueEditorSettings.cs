#define MeetAndTalk

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace MeetAndTalk.Settings
{
    public class DialogueEditorSettings : ScriptableObject
    {
        public const string k_CampaingPath = "Assets/Resources/DialogueEditorSettings.asset";

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

        private static DialogueEditorSettings _instance;
        public static DialogueEditorSettings Instance
        {
            get { return _instance; }
        }

    }



    public enum MeetAndTalkTheme
    {
        Dark = 0, PureDark = 1,
    }
}