#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using MeetAndTalk.Nodes;
using MeetAndTalk.Localization;
using MeetAndTalk.Settings;
using System.IO;

namespace MeetAndTalk.Editor
{
    [ExecuteInEditMode]
    public class DialogueEditorWindow : EditorWindow
    {
        // ���� ���� ���� ��ȭ �����̳ʸ� �����ϴ� ����
        private DialogueContainerSO currentDialogueContainer;

        // �׷��� ��� ����/�ε� ������ �����ϴ� ����
        private DialogueGraphView graphView;
        private DialogueSaveAndLoad saveAndLoad;

        // ���ö������̼� �� UI ��ҵ�
        private LocalizationEnum languageEnum = LocalizationEnum.English;
        private Label nameOfDialogueContainer;
        private ToolbarMenu toolbarMenu;
        private ToolbarMenu toolbarTheme;

        // �ڵ� ���� ������ ������ ������Ʈ �ð�
        private bool AutoSave = true;
        private float lastUpdateTime = 0f;

        // ���� ���� UI ���
        private Box _box;
        private HelpBox _infoBox;
        private bool _NoEditInfo = false;

        // LanguageEnum ������Ƽ
        public LocalizationEnum LanguageEnum { get => languageEnum; set => languageEnum = value; }

        // ������ ���� �� ȣ��Ǵ� �ݹ� �޼���
        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);
            LoadSettings();
            if (item is DialogueContainerSO && !Application.isPlaying) {
                DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
                window.titleContent = new GUIContent("Dialogue Editor", EditorGUIUtility.FindTexture("d_Favorite Icon"));
                window.currentDialogueContainer = item as DialogueContainerSO;
                window.minSize = new Vector2(500, 250);
                window.Load();
                
            }
            else if (Application.isPlaying) {
                EditorUtility.DisplayDialog("Can't Open a Dialogue", "Dialogue Editor can only be opened when the project is not on!\nTurn off Play Mode to open the Editor", "I understand");
            }

            return false;
        }

        private static void LoadSettings()
        {
            DialogueEditorSettings settings;
            var settingsPath = "Assets/Resources/DialogueEditorSettings.asset";
            settings = AssetDatabase.LoadAssetAtPath<DialogueEditorSettings>(settingsPath);

            if (settings == null) {
                Debug.LogWarning("Settings file not found, creating default settings.");
                settings = ScriptableObject.CreateInstance<DialogueEditorSettings>();
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
            }
        }

        // ������ �����찡 Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
        private void OnEnable()
        {
            AutoSave = Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSave;
            ContructGraphView();
            GenerateToolbar();
            Load();
        }



        // ������ �����찡 ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        // �׷��� �並 �����ϴ� �޼���
        private void ContructGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }

        // ���ٸ� �����ϴ� �޼���
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("Themes/DarkTheme");
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            // ���� ��ư ���� �� �̺�Ʈ �ڵ鷯 ���
            ToolbarButton saveBtn = new ToolbarButton() {
                text = "Save",
                name = "save_btn"
            };
            saveBtn.clicked += () => {
                Save();
                if (Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").ManualSaveLogs) Debug.Log("Manual Save");
            };
            toolbar.Add(saveBtn);

            // �ε� ��ư ���� �� �̺�Ʈ �ڵ鷯 ���
            ToolbarButton loadBtn = new ToolbarButton() {
                text = "Load",
                name = "load_btn"
            };
            loadBtn.clicked += () => {
                bool confirmed = EditorUtility.DisplayDialog("Load the Dialogue Save?", "Are you sure you want to load the dialogue saving?\nThis will delete all unsaved dialogue changes", "Confirm", "Cancel");

                if (confirmed) {
                    Load();
                }
            };
            toolbar.Add(loadBtn);

            nameOfDialogueContainer = new Label("");
            toolbar.Add(nameOfDialogueContainer);
            nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

            // ���ö������̼� �޴� ����
            toolbarMenu = new ToolbarMenu();
            toolbarMenu.name = "localization_enum";
            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum))) {
                toolbarMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language, toolbarMenu)));
            }
            toolbar.Add(toolbarMenu);

            // �׸� �޴� ����
            toolbarTheme = new ToolbarMenu();
            toolbarTheme.name = "theme_enum";
            foreach (MeetAndTalkTheme theme in (MeetAndTalkTheme[])Enum.GetValues(typeof(MeetAndTalkTheme))) {
                toolbarTheme.menu.AppendAction(theme.ToString(), new Action<DropdownMenuAction>(x => ChangeTheme(theme, toolbarTheme)));
            }
            toolbar.Add(toolbarTheme);

            // ���� ������ �߰�
            ToolbarSpacer sep_3 = new ToolbarSpacer();
            toolbar.Add(sep_3);

            // �ڵ� ���� ��� ��ư ���� �� �̺�Ʈ �ڵ鷯 ���
            Toggle autoSaveToggle = new Toggle("   Auto Save") {
                value = AutoSave,
                name = "autosave_toogle"
            };
            autoSaveToggle.RegisterValueChangedCallback(evt => {
                AutoSave = evt.newValue;
                Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSave = evt.newValue;
                Save();
            });
            toolbar.Add(autoSaveToggle);

            ToolbarSpacer sep_2 = new ToolbarSpacer();
            sep_2.style.flexGrow = 1;
            toolbar.Add(sep_2);

            // ����Ʈ ��ư ���� �� �̺�Ʈ �ڵ鷯 ���
            ToolbarButton importBtn = new ToolbarButton() {
                text = "Import Text",
                name = "import_btn"
            };
            importBtn.clicked += () => {
                Save();
                string path = EditorUtility.OpenFilePanel("Import Dialogue Localization File", Application.dataPath, "tsv");
                if (path.Length != 0) {
                    //currentDialogueContainer.ImportText(path, currentDialogueContainer);
                }
            };
            importBtn.SetEnabled(false);
            toolbar.Add(importBtn);

            // �ͽ���Ʈ ��ư ���� �� �̺�Ʈ �ڵ鷯 ���
            ToolbarButton exportBtn = new ToolbarButton() {
                text = "Export Text",
                name = "export_btn"
            };
            exportBtn.clicked += () => {
                Save();
                string path = EditorUtility.SaveFilePanel("Export Dialogue Localization File", Application.dataPath, currentDialogueContainer.name, "tsv");
                if (path.Length != 0) {
                    //currentDialogueContainer.GenerateCSV(path, currentDialogueContainer);
                }
            };
            exportBtn.SetEnabled(false);
            toolbar.Add(exportBtn);

            rootVisualElement.Add(toolbar);

            // ���� �� ����
            Label version = new Label() {
                name = "version_text",
                text = "Meet and Talk - Free Version"
            };
            version.pickingMode = PickingMode.Ignore;
            rootVisualElement.Add(version);
        }

        // �� ������ ȣ��Ǵ� �޼���
        private void OnGUI()
        {
            
            if (AutoSave && EditorApplication.timeSinceStartup - lastUpdateTime >= Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSaveInterval && !Application.isPlaying) {
                lastUpdateTime = (float)EditorApplication.timeSinceStartup;
                Save();
                if (Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSaveLogs) Debug.Log($"Auto Save [{DateTime.Now.ToString("HH:mm:ss")}]");
            }
            

            if (!Application.isPlaying) {
                _NoEditInfo = false;
                if (_box != null) {
                    rootVisualElement.Remove(_box);
                    _box = null;
                }

                if (_infoBox != null) {
                    rootVisualElement.Remove(_infoBox);
                    _infoBox = null;
                }
                graphView.ValidateDialogue();
            }

            if (Application.isPlaying && !_NoEditInfo) {
                _box = new Box();
                _box.StretchToParentSize();
                _box.style.backgroundColor = new StyleColor(new Color(0, 0, 0, .5f));
                rootVisualElement.Add(_box);

                _infoBox = new HelpBox("Dialogue Editor cannot be used when Play Mode is on, turn off the game to edit Dialogue", HelpBoxMessageType.Warning);
                _infoBox.StretchToParentSize();

                _infoBox.style.height = rootVisualElement.resolvedStyle.height * .3f;
                _infoBox.style.width = rootVisualElement.resolvedStyle.width * .3f;
                _infoBox.style.left = rootVisualElement.resolvedStyle.width * .35f;
                _infoBox.style.top = rootVisualElement.resolvedStyle.height * .35f;

                rootVisualElement.Add(_infoBox);

                _NoEditInfo = true;
            }
        }

        // ��ȭ �����͸� �ε��ϴ� �޼���
        private void Load()
        {
            if (currentDialogueContainer != null && !Application.isPlaying) {
                Language(LocalizationEnum.English, toolbarMenu);
                ChangeTheme(Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").Theme, toolbarTheme);
                nameOfDialogueContainer.text = "" + currentDialogueContainer.name;
                saveAndLoad.Load(currentDialogueContainer);

                if (Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").LoadLogs) Debug.Log($"Load {currentDialogueContainer.name}");
            }
        }

        // ��ȭ �����͸� �����ϴ� �޼���
        private void Save()
        {
            if (currentDialogueContainer != null && !Application.isPlaying) {
                saveAndLoad.Save(currentDialogueContainer);
            }
        }

        // ���ö������̼� ���� �޼���
        private void Language(LocalizationEnum _language, ToolbarMenu _toolbarMenu)
        {
            toolbarMenu.text = _language.ToString() + "";
            languageEnum = _language;
            graphView.LanguageReload();
        }

        // �׸� ���� �޼���
        private void ChangeTheme(MeetAndTalkTheme _theme, ToolbarMenu _toolbarMenu)
        {
            toolbarTheme.text = _theme.ToString() + "";
            Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").Theme = _theme;

            rootVisualElement.styleSheets.Remove(rootVisualElement.styleSheets[rootVisualElement.styleSheets.count - 1]);
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{_theme.ToString()}Theme"));

            graphView.UpdateTheme(_theme.ToString());
        }
    }
}
#endif