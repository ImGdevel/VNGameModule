using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VNDialogueModule : MonoBehaviour
{
    [SerializeField] GameObject dialogueUI;
    [SerializeField] VNDialogController dialogController;
    [SerializeField] SceneController sceneController;
    [SerializeField] ChoiceController choiceController;
    VNDialogManager dialogManager;
    

    private List<DialogData> dialogueList;
    private Dictionary<string, string> chracterName;
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

    void Start() {
        dialogController = dialogController.GetComponent<VNDialogController>();
        sceneController = sceneController.GetComponent<SceneController>();
        dialogManager = VNDialogManager.Instance.GetComponent<VNDialogManager>();

        sceneEvents = dialogManager.GetSceneEvents();
        isPauseGame = true;

        dialogController.OnTypingEnd += NextScene;
        choiceController.ChoiceScene += JumpScene;
        SettingsManager.OnSettingsChanged += ApplaySetting;
        MenuController.OnMenuOpened += PauseGame;
        dialogueUI.SetActive(false);

        Invoke("StartDialogue", 2.0f);
    }

    private void  StartDialogue() {
        ApplaySetting(SettingsManager.GetSettings);
        ToggleDialogueUI();
        LoadDialogue(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        isPauseGame = false;   
    }

    private void OnDestroy() {
        dialogController.OnTypingEnd -= NextScene;
        choiceController.ChoiceScene -= JumpScene;
        SettingsManager.OnSettingsChanged -= ApplaySetting;
        MenuController.OnMenuOpened -= PauseGame;
    }

    private void LoadDialogue(string loadName) {
        if (dialogManager == null) {
            Debug.LogError("DialogManager is not assigned.");
            return;
        }
        dialogueList = dialogManager.GetSceneDialogs(loadName); // 대화 데이터 받아오기

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
            switch (eventData.type) {
                case "ShowCharacter":
                    Debug.Log("캐릭터 전환");

                    break;
                    
                case "ChangeBackground":
                    Debug.Log("배경 전환");
                    break;

                case "PlaySound":
                    if (onSceneSkipMove) break;
                    BackgroundMusicManager.Instance.PlayMusic("JazzCafe");
                    break;

                case "PlayMusic":
                    string musicAudio = eventData.data;
                    if (onSceneSkipMove) break;
                    BackgroundMusicManager.Instance.PlayMusic(musicAudio);
                    break;

                case "SceneChange":
                    SceneChange(eventData.data);
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
                dialogController.ClearDialogue(); // 이전 대화 내용을 지웁니다.
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

    public void ApplaySetting(Settings settings) {
        typingSpeed = settings.dialogueSettings.typingSpeed;
        autoScrollDelay = settings.dialogueSettings.dialogueDelay;

        AutoDialogueKey = settings.controlSettings.AutoDialogKeyCode;
        NextDialogueKey = settings.controlSettings.NextDialogKeyCode;
        SkipDialogueKey = settings.controlSettings.SkipKeyCode;
    }

    private void ToggleDialogueUI() {
        showDialogue = !showDialogue;
        if (showDialogue) {
            dialogueUI.SetActive(true);
        }
        else {
            dialogueUI.SetActive(false);
        }
    }
}
