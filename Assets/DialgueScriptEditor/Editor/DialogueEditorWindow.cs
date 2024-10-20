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

namespace DialogueSystem.Editor
{
    [ExecuteInEditMode]
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueScript currentDialogueContainer;
        private DialogueGraphView graphView;
        private DialogueSaveAndLoad saveAndLoad;

        private LocalizationEnum languageEnum = LocalizationEnum.English;
        private Label nameOfDialogueContainer;
        private ToolbarMenu toolbarMenu;
        private ToolbarMenu toolbarTheme;

        private bool AutoSave = true;
        private float lastUpdateTime = 0f;

        // Validation
        private Box _box;
        private HelpBox _infoBox;
        private bool _NoEditInfo = false;

        public LocalizationEnum LanguageEnum { get => languageEnum; set => languageEnum = value; }

        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);

            if (item is DialogueScript && !Application.isPlaying)
            {
                DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
                window.titleContent = new GUIContent("Dialogue Editor", EditorGUIUtility.FindTexture("d_Favorite Icon"));
                window.currentDialogueContainer = item as DialogueScript;
                window.minSize = new Vector2(500, 250);
                window.Load();
            }
            else if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Can't Open a Dialogue", "Dialogue Editor can only be opened when the project is not on!\nTurn off Play Mode to open the Editor", "I understand");
            }

            return false;
        }

        private void OnEnable()
        {
            AutoSave = Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSave;
            ContructGraphView();
            GenerateToolbar();
            Load();
        }
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }


        private void ContructGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("Themes/BaseTheme");
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            ToolbarSpacer sep = new ToolbarSpacer();
            toolbar.Add(sep);

            nameOfDialogueContainer = new Label("");
            toolbar.Add(nameOfDialogueContainer);
            nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

            ToolbarSpacer sep_2 = new ToolbarSpacer();
            sep_2.style.flexGrow = 1;
            toolbar.Add(sep_2);

            toolbarMenu = new ToolbarMenu();
            toolbarMenu.name = "localization_enum";
            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum)))
            {
                toolbarMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language, toolbarMenu)));
            }
            toolbar.Add(toolbarMenu);

            toolbarTheme = new ToolbarMenu();
            toolbarTheme.name = "theme_enum";
            foreach (EditorTheme theme in (EditorTheme[])Enum.GetValues(typeof(EditorTheme)))
            {
                toolbarTheme.menu.AppendAction(theme.ToString(), new Action<DropdownMenuAction>(x => ChangeTheme(theme, toolbarTheme)));
            }
            //toolbar.Add(toolbarTheme);



            Toggle autoSaveToggle = new Toggle("   Auto Save")
            {
                value = AutoSave,
                name = "autosave_toogle"
            };
            autoSaveToggle.RegisterValueChangedCallback(evt =>
            {
                AutoSave = evt.newValue;
                Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSave = evt.newValue;
                Save();
            });
            //toolbar.Add(autoSaveToggle);

            ToolbarButton importBtn = new ToolbarButton()
            {
                text = "Import Script",
                name = "import_btn",
                tooltip = "import .csv file"
            };
            importBtn.clicked += () =>
            {
                // Save Actual Work
                Save();
                string path = EditorUtility.OpenFilePanel("Import Dialogue Localization File", Application.dataPath, "csv");
                if (path.Length != 0)
                {
                    currentDialogueContainer.ImportText(path, currentDialogueContainer);
                }
            };
            toolbar.Add(importBtn);

            ToolbarButton exportBtn = new ToolbarButton()
            {
                text = "Export Script",
                name = "export_btn",
                tooltip = "export script .csv file"
            };
            exportBtn.clicked += () =>
            {
                // Save Actual Work
                Save();

                string path = EditorUtility.SaveFilePanel("Export Dialogue Localization File", Application.dataPath, currentDialogueContainer.name, "csv");
                if (path.Length != 0)
                {
                    currentDialogueContainer.GenerateCSV(path, currentDialogueContainer);
                }
            };
            toolbar.Add(exportBtn);

            ToolbarButton saveBtn = new ToolbarButton() {
                text = "Apply",
                name = "save_btn"

            };
            saveBtn.clicked += () => {
                Save();
                if (Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").ManualSaveLogs) Debug.Log("Manual Save");
            };
            toolbar.Add(saveBtn);

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
            //toolbar.Add(loadBtn);

            rootVisualElement.Add(toolbar);

        }

        private void OnGUI()
        {
            if (AutoSave && EditorApplication.timeSinceStartup - lastUpdateTime >= Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").AutoSaveInterval && !Application.isPlaying)
            {
                lastUpdateTime = (float)EditorApplication.timeSinceStartup;
                Save();
            }

            if (!Application.isPlaying)
            {
                _NoEditInfo = false;
                if (_box != null)
                {
                    rootVisualElement.Remove(_box);
                    _box = null;
                }

                if (_infoBox != null)
                {
                    rootVisualElement.Remove(_infoBox);
                    _infoBox = null;
                }
                graphView.ValidateDialogue();
            }

            if (Application.isPlaying && !_NoEditInfo)
            {
                _box = new Box();
                _box.StretchToParentSize();
                _box.style.backgroundColor = new StyleColor(new Color(0, 0, 0, .5f));
                rootVisualElement.Add(_box);

                _infoBox = new HelpBox("Dialogue Editor cannot be used when Play Mode is ON,\n TURN OFF the game to edit Dialogue", HelpBoxMessageType.Warning);
                _infoBox.AddToClassList("PlayWarning");
                _infoBox.StretchToParentSize();

                _infoBox.style.height = rootVisualElement.resolvedStyle.height * .3f;
                _infoBox.style.width = rootVisualElement.resolvedStyle.width * .5f;
                _infoBox.style.left = rootVisualElement.resolvedStyle.width * .25f;
                _infoBox.style.top = rootVisualElement.resolvedStyle.height * .35f;

                rootVisualElement.Add(_infoBox);

                _NoEditInfo = true;
            }
        }

        private void Load()
        {
            if (currentDialogueContainer != null && !Application.isPlaying)
            {
                Language(LocalizationEnum.English, toolbarMenu);
                ChangeTheme(Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").Theme, toolbarTheme);
                nameOfDialogueContainer.text = "" + currentDialogueContainer.name;
                saveAndLoad.Load(currentDialogueContainer);

            }
        }
        private void Save()
        {
            if (currentDialogueContainer != null && !Application.isPlaying)
            {
                saveAndLoad.Save(currentDialogueContainer);
            }
        }
        private void Language(LocalizationEnum _language, ToolbarMenu _toolbarMenu)
        {
            toolbarMenu.text =  _language.ToString() + "";
            languageEnum = _language;
            graphView.LanguageReload();
        }
        private void ChangeTheme(EditorTheme _theme, ToolbarMenu _toolbarMenu)
        {
            toolbarTheme.text = _theme.ToString() + "";
            Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").Theme = _theme;

            rootVisualElement.styleSheets.Remove(rootVisualElement.styleSheets[rootVisualElement.styleSheets.count - 1]);
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{_theme.ToString()}Theme"));

            graphView.UpdateTheme(_theme.ToString());
        }


    }
#endif
}