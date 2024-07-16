#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using DialogueSystem.Nodes;
using DialogueSystem.Localization;
using DialogueSystem.Settings;
using System.IO;

namespace DialogueSystem.Editor
{
    [ExecuteInEditMode]
    public class DialogueEditorWindow : EditorWindow
    {
        // 현재 편집 중인 대화 컨테이너를 저장하는 변수
        private DialogueScript currentDialogueContainer;

        // 그래프 뷰와 저장/로드 도구를 저장하는 변수
        private DialogueGraphView graphView;
        private DialogueSaveAndLoad saveAndLoad;

        // 로컬라이제이션 및 UI 요소들
        private LocalizationEnum languageEnum = LocalizationEnum.English;
        private Label nameOfDialogueContainer;
        private ToolbarMenu toolbarMenu;
        private ToolbarMenu toolbarTheme;

        // 자동 저장 설정과 마지막 업데이트 시간
        private bool AutoSave = true;
        private float lastUpdateTime = 0f;

        // 검증 관련 UI 요소
        private Box _box;
        private HelpBox _infoBox;
        private bool _NoEditInfo = false;

        // LanguageEnum 프로퍼티
        public LocalizationEnum LanguageEnum { get => languageEnum; set => languageEnum = value; }

        // DialogueEditor 설정 정보
        private const string settingsPath = "Assets/Resources/DialogueEditorSettings.asset";

        // 에셋이 열릴 때 호출되는 콜백 메서드
        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);
            LoadSettings();
            if (item is DialogueScript && !Application.isPlaying) {
                DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
                window.titleContent = new GUIContent("Dialogue Editor", EditorGUIUtility.FindTexture("d_Favorite Icon"));
                window.currentDialogueContainer = item as DialogueScript;
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
            settings = AssetDatabase.LoadAssetAtPath<DialogueEditorSettings>(settingsPath);

            if (settings == null) {
                Debug.LogWarning("Settings file not found, creating default settings.");
                settings = ScriptableObject.CreateInstance<DialogueEditorSettings>();
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
            }
        }

        // 에디터 윈도우가 활성화될 때 호출되는 메서드
        private void OnEnable()
        {
            AutoSave = Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSave;
            ContructGraphView();
            GenerateToolbar();
            Load();
        }


        // 에디터 윈도우가 비활성화될 때 호출되는 메서드
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        // 그래프 뷰를 생성하는 메서드
        private void ContructGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }

        // 툴바를 생성하는 메서드
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("Themes/DarkTheme");
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();


            ToolbarSpacer sep = new ToolbarSpacer();
            toolbar.Add(sep);

            nameOfDialogueContainer = new Label("");
            toolbar.Add(nameOfDialogueContainer);
            nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

            ToolbarSpacer sep1 = new ToolbarSpacer();
            sep1.style.flexGrow = 1;
            toolbar.Add(sep1);

            // 로컬라이제이션 메뉴 생성
            toolbarMenu = new ToolbarMenu();
            toolbarMenu.name = "localization_enum";
            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum))) {
                toolbarMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language, toolbarMenu)));
            }
            toolbar.Add(toolbarMenu);

            // 테마 메뉴 생성
            toolbarTheme = new ToolbarMenu();
            toolbarTheme.name = "theme_enum";
            foreach (EditorTheme theme in (EditorTheme[])Enum.GetValues(typeof(EditorTheme))) {
                toolbarTheme.menu.AppendAction(theme.ToString(), new Action<DropdownMenuAction>(x => ChangeTheme(theme, toolbarTheme)));
            }

            // 임포트 버튼 생성 및 이벤트 핸들러 등록
            /*
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
            */

            // 익스포트 버튼 생성 및 이벤트 핸들러 등록
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


            // 저장 버튼 생성 및 이벤트 핸들러 등록
            ToolbarButton saveBtn = new ToolbarButton() {
                text = "Apply",
                name = "save_btn"
            };
            saveBtn.clicked += () => {
                Save();
                if (Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").ManualSaveLogs) Debug.Log("Manual Save");
            };
            toolbar.Add(saveBtn);


            rootVisualElement.Add(toolbar);

        }

        // 매 프레임 호출되는 메서드
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

        // 대화 데이터를 로드하는 메서드
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

        // 대화 데이터를 저장하는 메서드
        private void Save()
        {
            if (currentDialogueContainer != null && !Application.isPlaying) {
                saveAndLoad.Save(currentDialogueContainer);
            }
        }

        // 로컬라이제이션 변경 메서드
        private void Language(LocalizationEnum _language, ToolbarMenu _toolbarMenu)
        {
            toolbarMenu.text = _language.ToString() + "";
            languageEnum = _language;
            graphView.LanguageReload();
        }

        // 테마 변경 메서드
        private void ChangeTheme(EditorTheme _theme, ToolbarMenu _toolbarMenu)
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
