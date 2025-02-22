using DialogueSystem;
using DialogueSystem.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VisualNovelGame;
using VisualNovelGame.Data;

/// <summary>
/// VN게임의 메인 게임 모듈
/// </summary>

namespace VisualNovelGame
{
    public class VNDialogueModule : MonoBehaviour
    {
        public static VNDialogueModule Instance { get; private set; }

        [SerializeField] GameObject dialogueUI;
        [SerializeField] VNDialogController dialogController;
        [SerializeField] VNChoiceController choiceController;
        [SerializeField] VNCharacterController characterController;
        [SerializeField] VNBackgroundController backgroundController;
        VNDialogManager dialogManager;

        public ScenarioManager scenarioManager;
        private string currentSceneId = null;

        private float typingSpeed = 0.03f;
        private float autoScrollDelay = 2.0f;

        private bool isReadToNextScene = false;

        private bool isLockedUserCommand = true;
        private bool waitingForNextScene = false;
        private bool autoScrollEnabled = false;
        private bool onSceneSkipMove = false;
        private bool isDialogueVisible = false;

        private bool isSkipMode = false;

        private int senceCount = 0;

        private string currentSceneName;

        private KeyCode nextDialogueKey = KeyCode.Space;
        private KeyCode skipDialogueKey = KeyCode.LeftControl;
        private KeyCode autoDialogueKey = KeyCode.A;
        private KeyCode hideDialogueKey = KeyCode.Tab;

        public static event Action ForceTerminateScene;

        void Awake()
        {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
                return;
            }

            currentSceneName = SceneManager.GetActiveScene().name;
            InitControllerSetting();
        }

        void Start()
        {
            if (scenarioManager == null) {
                scenarioManager = FindObjectOfType<ScenarioManager>();
            }

            dialogController.OnTypingEnd += EndDialogueScene;
            choiceController.ChoiceScene += PlayScene;
            SettingsManager.OnSettingsChanged += ApplySettings;
            MenuController.OnMenuOpened += SetGamePause;

            isLockedUserCommand = true;

            StartCoroutine(StartSceneAfterDelay(2.0f));
        }

        /// <summary>
        /// 일정시간이 지나고 씬을 시작합니다.
        /// </summary>
        /// <param name="delay"></param>
        private IEnumerator StartSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ToggleDialogueUI();
            ApplySettings(SettingsManager.GameSetting);
            currentSceneId = scenarioManager.GetStartSceneId();
            PlayScene(currentSceneId);
            isLockedUserCommand = false;
        }

        /// <summary>
        /// 유저 커맨드 조작
        /// </summary>
        void Update()
        {

            if (Input.GetKeyDown(nextDialogueKey)) {
                PlayNextScene();
            }

            if (Input.GetKeyUp(skipDialogueKey)) {
                onSceneSkipMove = false;
            }

            if (Input.GetKey(skipDialogueKey)) {
                Time.timeScale = 20.0f;
            }
            else {
                Time.timeScale = 1.0f;
            }
        }




        /// <summary>
        /// 다음 씬을 요청합니다.
        /// </summary>
        public void PlayNextScene()
        {
            if (isLockedUserCommand) {
                return;
            }

            if (isReadToNextScene) {
                ForceTerminateScene?.Invoke();
            }
            PlayScene(currentSceneId);
        }

        /// <summary>
        /// 특정씬으로 이동합니다.
        /// </summary>
        /// <param name="sceneId"></param>
        public void JumpScene(string sceneId)
        {
            if (isReadToNextScene) {
                ForceTerminateScene?.Invoke();
            }
            PlayScene(currentSceneId);
        }


        /// <summary>
        /// 자동 스크롤 모드
        /// </summary>
        public void ToggleAutoScrollSceneMode()
        {
            if (isLockedUserCommand) {
                return;
            }
            autoScrollEnabled = !autoScrollEnabled;
            Debug.Log("자동 스크롤 모드 ON");
        }

        /// <summary>
        /// 씬을 스킵합니다.
        /// </summary>
        public void FastSkipON()
        {
            Time.timeScale = 20.0f;
        }

        /// <summary>
        /// 지정된 ID의 씬을 실행합니다.
        /// </summary>
        /// <param name="sceneId"></param>
        private void PlayScene(string sceneId)
        {
            ScriptDTO scriptDTO = scenarioManager.GetSceneDataById(currentSceneId);
            isLockedUserCommand = false;
            if (scriptDTO != null) {
                isReadToNextScene = false;
                switch (scriptDTO) {
                    case DialogueScriptDTO nodeData:
                        PlayDialogueScene(nodeData);
                        break;
                    case TimerChoiceScriptDTO nodeData:
                        PlayTimeChoiceScene(nodeData);
                        break;
                    case ChoiceScriptDTO nodeData:
                        PlayChoiceScene(nodeData);
                        break;
                    case CharacterScriptDTO nodeData:
                        PlayCharacterScene(nodeData);
                        break;
                    case EndScriptDTO nodeData:
                        PlayEndScene(nodeData);
                        break;
                    case RandomScriptDTO nodeData:
                        PlayRandomScene(nodeData);
                        break;
                    case IfScriptDTO nodeData:
                        PlayIfScene(nodeData);
                        break;
                    default:
                        Debug.Log("Not Find ScriptDTO type");
                        break;
                }
                Debug.Log("Current script number: " + currentSceneName + "(" + sceneId + ")");
            }
            else {
                Debug.Log("Not Find ScriptDTO");
            }
        }

        /// <summary>
        /// 대화 씬
        /// </summary>
        /// <param name="script">스크립트</param>
        private void PlayDialogueScene(DialogueScriptDTO script)
        {
            dialogController.TypeDialogue(script.CharacterName, script.DialogueText, typingSpeed);
        }

        /// <summary>
        /// 대화씬 타이핑이 종료된 경우
        /// </summary>
        public void EndDialogueScene()
        {
            isReadToNextScene = true;
            currentSceneId = scenarioManager.GetNextSceneIdById(currentSceneId);
            if (autoScrollEnabled) {
                StartCoroutine(AutoScrollToNextScene());
            }
        }

        /// <summary>
        /// autoScrollDelay 시간 동안 기다렸다가 다음 씬을 실행하는 Coroutine
        /// </summary>
        private IEnumerator AutoScrollToNextScene()
        {
            yield return new WaitForSeconds(autoScrollDelay);
            currentSceneId = scenarioManager.GetNextSceneIdById(currentSceneId);
            PlayScene(currentSceneId);
        }

        /// <summary>
        /// 선택지 씬
        /// </summary>
        /// <param name="script">스크립트</param>
        private void PlayChoiceScene(ChoiceScriptDTO script)
        {
            isLockedUserCommand = true;
            choiceController.transform.gameObject.SetActive(true);
            choiceController.ShowChoices(script.Choices);
            dialogController.ClearDialogue();
        }

        /// <summary>
        /// 시간 제한 선택지 씬
        /// </summary>
        /// <param name="script">스크립트</param>
        private void PlayTimeChoiceScene(TimerChoiceScriptDTO script)
        {
            isLockedUserCommand = true;
            choiceController.transform.gameObject.SetActive(true);
            choiceController.ShowChoicesWithTimer(script.Choices, script.TimeLimit);
            dialogController.ClearDialogue();
        }

        /// <summary>
        /// 랜덤 스크립트 씬
        /// </summary>
        /// <param name="script">스크립트</param>
        private void PlayRandomScene(RandomScriptDTO script)
        {
            List<Choice> choices = script.randomChoices;
            PlayScene(choices[UnityEngine.Random.Range(0, choices.Count)].nextScriptId);
        }

        /// <summary>
        /// 캐릭터 씬 
        /// </summary>
        /// <param name="script"></param>
        private void PlayCharacterScene(CharacterScriptDTO script)
        {
            switch (script.CharacterEffectType) {
                case CharacterEffectType.None:
                    characterController.ShowCharacter(script.CharacterName, script.CharacterSprite, 0);
                    break;
                case CharacterEffectType.FadeIn:
                    characterController.ShowCharacter(script.CharacterName, script.CharacterSprite);
                    break;
                case CharacterEffectType.FadeOut:
                    break;
                case CharacterEffectType.Translate:
                    break;
                case CharacterEffectType.Shake:
                    break;
                case CharacterEffectType.Pomping:
                    break;
                default:
                    break;
            }

            currentSceneId = scenarioManager.GetNextSceneIdById(currentSceneId);
            PlayScene(currentSceneId);
        }

        /// <summary>
        /// 종료 씬
        /// </summary>
        /// <param name="script">스크립트</param>
        private void PlayEndScene(EndScriptDTO script)
        {
            // 게임 종료 처리 또는 엔딩씬 로직 추가
            Debug.Log("End of the dialogue scene.");
            dialogController.ClearDialogue();
            ForceTerminateScene?.Invoke();
        }

        /// <summary>
        /// If 조건에 따른 씬 실행
        /// </summary>
        /// <param name="script">스크립트</param>
        private void PlayIfScene(IfScriptDTO script)
        {

        }


        /// <summary>
        ///  다음 쳄터로
        /// </summary>
        /// <param name="sceneName"></param>
        private void NextChapter(string ChapterName)
        {
            SceneManager.LoadScene(ChapterName);
        }

        /// <summary>
        /// 게임 상호작용 잠금
        /// </summary>
        /// <param name="state">상태</param>
        public void SetGamePause(bool state)
        {
            isLockedUserCommand = state;
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
            dialogueUI.SetActive(isDialogueVisible);
        }

        public void SaveDialogueData(GameData saveData)
        {
            Debug.Log("대화 데이터 저장");
        }

        public void LoadDialogueData(GameData saveData)
        {
            Debug.Log("대화 데이터 불러오기 적용");
        }

        /// <summary>
        /// 컨트롤러 등록 여부 체크
        /// </summary>
        private void InitControllerSetting()
        {
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

        private void OnDestroy()
        {
            dialogController.OnTypingEnd -= EndDialogueScene;
            choiceController.ChoiceScene -= PlayScene;
            SettingsManager.OnSettingsChanged -= ApplySettings;
            MenuController.OnMenuOpened -= SetGamePause;
        }

    }

}


