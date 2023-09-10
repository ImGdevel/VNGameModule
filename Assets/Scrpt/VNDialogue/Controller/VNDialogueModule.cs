using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VNDialogueModule : MonoBehaviour
{
    [SerializeField] GameObject dialogueUI;
    [SerializeField] VNDialogController dialogController;
    [SerializeField] VNChoiceController choiceController;
    [SerializeField] VNCharacterController characterController;
    [SerializeField] VNBackgroundController backgroundController;
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
    private bool isDialogueVisible = false;
    private bool isGamePaused = true;
    private string currentSceneName;

    private KeyCode nextDialogueKey = KeyCode.Space;
    private KeyCode skipDialogueKey = KeyCode.LeftControl;
    private KeyCode autoDialogueKey = KeyCode.A;
    private KeyCode hideDialogueKey = KeyCode.Tab;

    void Awake() {
        currentSceneName = SceneManager.GetActiveScene().name;
        dialogController = FindObjectOfType<VNDialogController>();
        if (dialogController == null) {
            Debug.LogError("VNDialogController not found.");
        }
        choiceController = FindObjectOfType<VNChoiceController>();
        if (choiceController == null) {
            Debug.LogError("VNChoiceController not found.");
        }
        backgroundController = FindObjectOfType<VNBackgroundController>();
        if (backgroundController == null) {
            Debug.LogError("VNBackgroundController not found.");
        }
        characterController = FindObjectOfType<VNCharacterController>();
        if (characterController == null) {
            Debug.LogError("VNCharacterController not found.");
        }
        dialogManager = FindObjectOfType<VNDialogManager>();
        if (dialogManager == null) {
            Debug.LogError("VNDialogManager not found.");
        }
    }

    void Start() {
        isGamePaused = true;
        RegisterEventListeners();
        StartCoroutine(StartDialogueAfterDelay(2.0f));
    }

    private void RegisterEventListeners() {
        dialogController.OnTypingEnd += NextScene;
        choiceController.ChoiceScene += JumpScene;
        SettingsManager.OnSettingsChanged += ApplySettings;
        MenuController.OnMenuOpened += ToggleGamePause;
    }

    private void OnDestroy() {
        UnregisterEventListeners();
    }

    private void UnregisterEventListeners() {
        dialogController.OnTypingEnd -= NextScene;
        choiceController.ChoiceScene -= JumpScene;
        SettingsManager.OnSettingsChanged -= ApplySettings;
        MenuController.OnMenuOpened -= ToggleGamePause;
    }

    private IEnumerator StartDialogueAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        StartDialogue();
    }

    private void StartDialogue() {
        ToggleDialogueUI();
        ApplySettings(SettingsManager.GameSetting);
        LoadDialogue(currentSceneName);
        isGamePaused = false;
    }

    private void LoadDialogue(string loadName) {
        if (dialogManager == null) {
            Debug.LogError("VNDialogManager is not assigned.");
            return;
        }
        sceneEvents = dialogManager.GetSceneEvents();
        dialogueList = dialogManager.GetSceneDialogs(loadName);

        if (dialogueList != null && dialogueList.Count > 0) {
            currentDialogueIndex = 0;
            DialogData dialog = dialogueList[currentDialogueIndex];
            if (Input.GetKey(skipDialogueKey)) {
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
        if (isGamePaused) {
            return;
        }

        if (Input.GetKeyDown(nextDialogueKey)) {
            PlayScene(dialogueList[currentDialogueIndex]);
        }

        if (Input.GetKey(skipDialogueKey)) {
            onSceneSkipMove = true;
            SkipScene(dialogueList[currentDialogueIndex]);
        }

        if (Input.GetKeyUp(skipDialogueKey)) {
            onSceneSkipMove = false;
        }

        if (!autoScrollEnabled && Input.GetKeyDown(autoDialogueKey)) {
            autoScrollEnabled = true;
            StopAllCoroutines();
            StartCoroutine(AutoPlayScene());
        }
        else if (autoScrollEnabled && Input.anyKeyDown) {
            autoScrollEnabled = false;
            StopCoroutine("AutoPlayScene");
        }

        if (Input.GetKeyDown(hideDialogueKey)) {
            ToggleDialogueUI();
        }
    }

    public void NextScene() {
        currentDialogueIndex = (currentDialogueIndex + 1) % dialogueList.Count;
        waitingForNextScene = false;
    }

    private void PlayScene(DialogData dialog) {
        Debug.Log("Current dialogue number: " + currentSceneName + "(" + dialog.id + ")");
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
        isGamePaused = true;
        choiceController.transform.gameObject.SetActive(true);
        choiceController.ShowChoices(dialog.choices);
        dialogController.ClearDialogue();
    }

    public void JumpScene(string jump_id) {
        choiceController.transform.gameObject.SetActive(false);
        isGamePaused = false;

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

    public void ToggleGamePause(bool state) {
        isGamePaused = state;
    }

    public void ApplySettings(Settings settings) {
        typingSpeed = settings.dialogueSettings.typingSpeed;
        autoScrollDelay = settings.dialogueSettings.dialogueDelay;

        autoDialogueKey = settings.controlSettings.AutoDialogKeyCode;
        nextDialogueKey = settings.controlSettings.NextDialogKeyCode;
        skipDialogueKey = settings.controlSettings.SkipKeyCode;
        hideDialogueKey = settings.controlSettings.HideUIKeyCode;
    }

    private void ToggleDialogueUI() {
        isDialogueVisible = !isDialogueVisible;
        dialogueUI.SetActive(isDialogueVisible);
    }
}
