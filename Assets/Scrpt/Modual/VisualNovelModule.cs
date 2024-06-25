using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace VisualNovelGame
{

    public class VisualNovelModule : MonoBehaviour
    {
        // Scenario Manager
        public ScenarioManager scenarioManager;

        
        private VNDialogManager dialogManager;

        // Controller
        public ScenarioController scenarioController;
        public DialogController dialogController;
        public ChoiceController choiceController;
        public CharacterController characterController;
        public BackgroundController backgroundController;
        public AudioController audioController;
        

        private List<DialogData> dialogueList;
        private Dictionary<string, string> characterNames;
        private Dictionary<string, List<EventData>> sceneEvents;
        private int currentDialogueIndex = 0;

        private int currentScenarioId;
        private string currentSceneName;

        private bool isGamePaused = false;
        private bool waitingForNextScene = false;
        private bool autoScrollEnabled = false;
        private bool onSceneSkipMove = false;
        private bool isDialogueVisible = false;

        private float typingSpeed = 0.03f;
        private float autoScrollDelay = 2.0f;

        private KeyCode nextDialogueKey = KeyCode.Space;
        private KeyCode skipDialogueKey = KeyCode.LeftControl;
        private KeyCode autoDialogueKey = KeyCode.A;
        private KeyCode hideDialogueKey = KeyCode.Tab;

        public static event Action OnFinishScenario;

        void Awake()
        {
            if (dialogController == null) {
                dialogController = FindObjectOfType<DialogController>();
                Debug.LogError("DialogController not found.");
            }
            if (choiceController == null) {
                choiceController = FindObjectOfType<ChoiceController>();
                Debug.LogError("DialogController not found.");
            }
            if (backgroundController == null) {
                backgroundController = FindObjectOfType<BackgroundController>();
                Debug.LogError("BackgroundController not found.");
            }
            if (characterController == null) {
                characterController = FindObjectOfType<CharacterController>();
                Debug.LogError("CharacterController not found.");
            }   
            if (audioController == null) {
                audioController = FindObjectOfType<AudioController>();
                Debug.LogError("CharacterController not found.");
            }
        }


        void Start()
        {
            //LoadScenario(1); // 예시로 첫 번째 시나리오를 로드
        }

        private void RegisterEventListeners()
        {
        }

        private void OnDestroy()
        {
            UnregisterEventListeners();
        }

        private void UnregisterEventListeners()
        {

        }

        private IEnumerator StartDialogueAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartDialogue();
        }

        private void StartDialogue()
        {
            ToggleDialogueUI();
            ApplySettings(SettingsManager.GameSetting);
            LoadDialogue(currentSceneName);
            isGamePaused = false;
        }

        private void LoadDialogue(string loadName)
        {
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

        void Update()
        {
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

        public void NextDialogue()
        {
            currentDialogueIndex = (currentDialogueIndex + 1) % dialogueList.Count;
            waitingForNextScene = false;
            OnFinishScenario?.Invoke();
        }

        private void PlayScene(DialogData dialog)
        {
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

        private void SkipScene(DialogData dialog)
        {
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

        private void PlaySceneEvent(List<EventData> eventDatas)
        {

        }

        private void SceneChange(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void ChoiceScene(DialogData dialog)
        {
            isGamePaused = true;
            choiceController.transform.gameObject.SetActive(true);
            choiceController.ShowChoices(dialog.choices);
            dialogController.ClearDialogue();
        }

        public void JumpScene(string jump_id)
        {
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

        private IEnumerator AutoPlayScene()
        {
            while (autoScrollEnabled) {
                PlayScene(dialogueList[currentDialogueIndex]);

                waitingForNextScene = true;
                yield return new WaitUntil(() => !waitingForNextScene);
                yield return new WaitForSeconds(autoScrollDelay);
            }
        }

        public void ToggleGamePause(bool state)
        {
            isGamePaused = state;
        }

        public void ApplySettings(Settings settings)
        {
            typingSpeed = settings.dialogueSettings.typingSpeed;
            autoScrollDelay = settings.dialogueSettings.dialogueDelay;

            autoDialogueKey = settings.controlSettings.AutoDialogKeyCode;
            nextDialogueKey = settings.controlSettings.NextDialogKeyCode;
            skipDialogueKey = settings.controlSettings.SkipKeyCode;
            hideDialogueKey = settings.controlSettings.HideUIKeyCode;
        }

        private void ToggleDialogueUI()
        {
            isDialogueVisible = !isDialogueVisible;
            // 대화창 숨기거나 보이기
        }

        public void SaveDialogueData(GameData saveData)
        {
            Debug.Log("대화 데이터 저장");

        }

        public void LoadDialogueData(GameData saveData)
        {
            Debug.Log("대화 데이터 불러오기 적용");

        }

    }

    /*
    public void LoadScenario(int scenarioId)
        {
            Scenario scenario = scenarioManager.GetScenario(scenarioId);
            if (scenario != null) {
                currentScenarioId = scenarioId;
                scenarioController.SetScenario(scenario);
                characterController.SetScenario(scenario);
                backgroundController.SetScenario(scenario);
                audioController.SetScenario(scenario);
            }
        }

        void Update()
        {
            // 클릭으로 대사 넘기기 등 플레이어와의 상호작용 관리
            if (Input.GetMouseButtonDown(0)) {
                scenarioController.NextDialogue();
            }
        }
    }
    */

}

