using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VNDialogueModule : MonoBehaviour
{
    [SerializeField] GameObject dialogueUI;
    VNDialogController dialogController;
    VNChoiceController choiceController;
    VNCharacterController characterController;
    VNBackgroundController backgroundController;
    VNDialogManager dialogManager;

    private List<DialogData> dialogueList;
    private Dictionary<string, string> characterNames;
    private Dictionary<string, List<EventData>> sceneEvents;
    private int currentDialogueIndex = 0;

    private float typingSpeed = 0.03f;
    private float autoScrollDelay = 2.0f;

    private bool waitingForNextScene = false;
    private bool autoScrollEnabled = false;
    private bool onSceneSkipMove = false;
    private bool showDialogue = false;
    private bool isPauseGame = true;
    private string currentSceneName;

    private KeyCode NextDialogueKey = KeyCode.Space;
    private KeyCode SkipDialogueKey = KeyCode.LeftControl;
    private KeyCode AutoDialogueKey = KeyCode.A;
    private KeyCode HideDialogurKey = KeyCode.Tab;

    void Awake() {
        dialogController = FindObjectOfType<VNDialogController>();
        if (dialogController == null) {
            Debug.LogError("can not find VNDialogController");
        }
        choiceController = FindObjectOfType<VNChoiceController>();
        if (choiceController == null) {
            Debug.LogError("can not find VNChoiceController");
        }
        backgroundController = FindObjectOfType<VNBackgroundController>();
        if (backgroundController == null) {
            Debug.LogError("can not find VNBackgroundController");
        }
        characterController = FindObjectOfType<VNCharacterController>();
        if (characterController == null) {
            Debug.LogError("can not find VNCharacterController");
        }
        dialogManager = FindObjectOfType<VNDialogManager>();
        if (dialogManager == null) {
            Debug.LogError("can not find VNDialogManager");
        }
    }

    void Start() {
        currentSceneName = SceneManager.GetActiveScene().name;
        RegisterEventListeners();

        sceneEvents = dialogManager.GetSceneEvents();
        isPauseGame = true;
        dialogueUI.SetActive(false);

        Invoke("StartDialogue", 2.0f);
    }

    private void StartDialogue() {
        ApplySetting(SettingsManager.GameSetting);
        ToggleDialogueUI();
        LoadDialogue(currentSceneName);
        isPauseGame = false;
    }

    private void OnDestroy() {
        UnregisterEventListeners();
    }

    private void RegisterEventListeners() {
        dialogController.OnTypingEnd += NextScene;
        choiceController.ChoiceScene += JumpScene;
        SettingsManager.OnSettingsChanged += ApplySetting;
        MenuController.OnMenuOpened += PauseGame;
    }

    private void UnregisterEventListeners() {
        dialogController.OnTypingEnd -= NextScene;
        choiceController.ChoiceScene -= JumpScene;
        SettingsManager.OnSettingsChanged -= ApplySetting;
        MenuController.OnMenuOpened -= PauseGame;
    }

    

    private void LoadDialogue(string loadName) {
        if (dialogManager == null) {
            Debug.LogError("DialogManager is not assigned.");
            return;
        }
        dialogueList = dialogManager.GetSceneDialogs(loadName);

        if (dialogueList != null && dialogueList.Count > 0) {
            currentDialogueIndex = 0;
            DialogData dialog = dialogueList[currentDialogueIndex];
            if (Input.GetKey(SkipDialogueKey)) {
                SkipScene(dialog);
            }
            else {
                PlayScene(dialog);
            }
        }
        else {
            Debug.LogWarning("No dialogue data found for the current scene.");
        }
    }

    void Update() {
        if (isPauseGame) {
            return;
        }

        if (Input.GetKeyDown(NextDialogueKey)) {
            PlayScene(dialogueList[currentDialogueIndex]);
        }

        if (Input.GetKey(SkipDialogueKey)) {
            onSceneSkipMove = true;
            SkipScene(dialogueList[currentDialogueIndex]);
        }

        if (Input.GetKeyUp(SkipDialogueKey)) {
            onSceneSkipMove = false;
        }

        if (!autoScrollEnabled && Input.GetKeyDown(AutoDialogueKey)) {
            autoScrollEnabled = true;
            StopAllCoroutines();
            StartCoroutine(AutoPlayScene());
        }
        else if (autoScrollEnabled && Input.anyKeyDown) {
            autoScrollEnabled = false;
            StopCoroutine("AutoPlayScene");
        }

        if (Input.GetKeyDown(HideDialogurKey)) {
            ToggleDialogueUI();
        }
    }

    public void NextScene() {
        currentDialogueIndex = (currentDialogueIndex + 1) % dialogueList.Count;
        waitingForNextScene = false;
    }

    private void PlayScene(DialogData dialog) {
        Debug.Log("current dialogue number: " + currentSceneName + "(" + dialog.id + ")");
        if (dialog.choices.Count == 0) {
            if (sceneEvents.ContainsKey(dialog.id)) {
                PlaySceneEvent(sceneEvents[dialog.id]);
            }

            dialogController.TypeDialogue(dialog.character, dialog.content, typingSpeed);
        }
        else {
            ChoiceScene(dialog);
        }
    }

    private void SkipScene(DialogData dialog) {
        if (dialog.choices.Count == 0) {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                dialogController.CurrentDialogueSkip();
            }
            else {
                if (sceneEvents.ContainsKey(dialog.id)) {
                    PlaySceneEvent(sceneEvents[dialog.id]);
                }
                dialogController.SkipDialogue(dialog.character, dialog.content);
            }
        }
        else {
            ChoiceScene(dialog);
        }
    }

    private void PlaySceneEvent(List<EventData> eventDatas) {
        foreach (EventData eventData in eventDatas) {
            Data data = eventData.data;
            switch (eventData.type) {
                case "ShowCharacter":
                    characterController.ShowCharacter(data.name, data.number, data.time);
                    break;
                case "MoveCharacter":
                    characterController.MoveCharacter(data.name, new Vector2(data.position.x, data.position.y), data.time);
                    break;
                case "DismissCharacter":
                    characterController.DismissCharacter(data.name, data.time);
                    break;
                case "ChangeBackground":
                    // 배경 변경 이벤트 처리 추가
                    break;
                case "PlaySound":
                    if (onSceneSkipMove) break;
                    BackgroundMusicManager.Instance.PlayMusic("JazzCafe");
                    break;
                case "PlayMusic":
                    if (onSceneSkipMove) break;
                    string musicAudio = data.name;
                    BackgroundMusicManager.Instance.PlayMusic(musicAudio);
                    break;
                case "SceneChange":
                    SceneChange(data.name);
                    break;
                default:
                    Debug.LogWarning("Cannot find event type.");
                    break;
            }
        }
    }

    private void SceneChange(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    private void ChoiceScene(DialogData dialog) {
        isPauseGame = true;
        choiceController.transform.gameObject.SetActive(true);
        choiceController.ShowChoices(dialog.choices);
        dialogController.ClearDialogue();
    }

    public void JumpScene(string jump_id) {
        choiceController.transform.gameObject.SetActive(false);
        isPauseGame = false;

        for (int i = 0; i < dialogueList.Count; i++) {
            if (dialogueList[i].id == jump_id) {
                currentDialogueIndex = i;
                dialogController.ClearDialogue();
                PlayScene(dialogueList[currentDialogueIndex]);
                break;
            }
        }
    }

    private IEnumerator AutoPlayScene() {
        while (autoScrollEnabled) {
            PlayScene(dialogueList[currentDialogueIndex]);

            waitingForNextScene = true;
            yield return new WaitUntil(() => !waitingForNextScene);
            yield return new WaitForSeconds(autoScrollDelay);
        }
    }

    public void PauseGame(bool state) {
        isPauseGame = state;
    }

    public void ApplySetting(Settings settings) {
        typingSpeed = settings.dialogueSettings.typingSpeed;
        autoScrollDelay = settings.dialogueSettings.dialogueDelay;

        AutoDialogueKey = settings.controlSettings.AutoDialogKeyCode;
        NextDialogueKey = settings.controlSettings.NextDialogKeyCode;
        SkipDialogueKey = settings.controlSettings.SkipKeyCode;
        HideDialogurKey = settings.controlSettings.HideUIKeyCode;
    }

    private void ToggleDialogueUI() {
        showDialogue = !showDialogue;
        dialogueUI.SetActive(showDialogue);
    }
}
