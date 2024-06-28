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

namespace MeetAndTalk.Editor
{
    [ExecuteInEditMode]
    public class DialogueEditorWindow : EditorWindow
    {
        // 현재 편집 중인 대화 컨테이너를 저장하는 변수
        private DialogueContainerSO currentDialogueContainer;

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

        // 에셋이 열릴 때 호출되는 콜백 메서드
        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);

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

        // 에디터 윈도우가 활성화될 때 호출되는 메서드
        private void OnEnable()
        {
            AutoSave = Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").AutoSave;
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

            // 저장 버튼 생성 및 이벤트 핸들러 등록
            ToolbarButton saveBtn = new ToolbarButton() {
                text = "Save",
                name = "save_btn"
            };
            saveBtn.clicked += () => {
                Save();
                if (Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").ManualSaveLogs) Debug.Log("Manual Save");
            };
            toolbar.Add(saveBtn);

            // 로드 버튼 생성 및 이벤트 핸들러 등록
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
            foreach (MeetAndTalkTheme theme in (MeetAndTalkTheme[])Enum.GetValues(typeof(MeetAndTalkTheme))) {
                toolbarTheme.menu.AppendAction(theme.ToString(), new Action<DropdownMenuAction>(x => ChangeTheme(theme, toolbarTheme)));
            }
            toolbar.Add(toolbarTheme);

            // 툴바 구분자 추가
            ToolbarSpacer sep_3 = new ToolbarSpacer();
            toolbar.Add(sep_3);

            // 자동 저장 토글 버튼 생성 및 이벤트 핸들러 등록
            Toggle autoSaveToggle = new Toggle("   Auto Save") {
                value = AutoSave,
                name = "autosave_toogle"
            };
            autoSaveToggle.RegisterValueChangedCallback(evt => {
                AutoSave = evt.newValue;
                Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").AutoSave = evt.newValue;
                Save();
            });
            toolbar.Add(autoSaveToggle);

            ToolbarSpacer sep_2 = new ToolbarSpacer();
            sep_2.style.flexGrow = 1;
            toolbar.Add(sep_2);

            // 임포트 버튼 생성 및 이벤트 핸들러 등록
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

            rootVisualElement.Add(toolbar);

            // 버전 라벨 생성
            Label version = new Label() {
                name = "version_text",
                text = "Meet and Talk - Free Version"
            };
            version.pickingMode = PickingMode.Ignore;
            rootVisualElement.Add(version);
        }

        // 매 프레임 호출되는 메서드
        private void OnGUI()
        {
            if (AutoSave && EditorApplication.timeSinceStartup - lastUpdateTime >= Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").AutoSaveInterval && !Application.isPlaying) {
                lastUpdateTime = (float)EditorApplication.timeSinceStartup;
                Save();
                if (Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").AutoSaveLogs) Debug.Log($"Auto Save [{DateTime.Now.ToString("HH:mm:ss")}]");
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
                ChangeTheme(Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").Theme, toolbarTheme);
                nameOfDialogueContainer.text = "" + currentDialogueContainer.name;
                saveAndLoad.Load(currentDialogueContainer);

                if (Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").LoadLogs) Debug.Log($"Load {currentDialogueContainer.name}");
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
        private void ChangeTheme(MeetAndTalkTheme _theme, ToolbarMenu _toolbarMenu)
        {
            toolbarTheme.text = _theme.ToString() + "";
            Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").Theme = _theme;

            rootVisualElement.styleSheets.Remove(rootVisualElement.styleSheets[rootVisualElement.styleSheets.count - 1]);
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{_theme.ToString()}Theme"));

            graphView.UpdateTheme(_theme.ToString());
        }
    }
}
#endif
