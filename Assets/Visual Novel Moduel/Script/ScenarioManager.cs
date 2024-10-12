using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DialogueSystem;
using DialogueSystem.Localization;
using VNGameModuel.Controller;

namespace VNGameModuel
{
    public class ScenarioManager : DialogueGetData
    {
        public static ScenarioManager Instance;

        [SerializeField] private DialogueBoxController dialogueBoxController;
        [SerializeField] private ChoiceController choiceController;
        [SerializeField] private CharacterCGController characterCGController;
        [SerializeField] private BackgroundController backgroundController;
        [SerializeField] private EventCGController eventCGController;

        public LocalizationManager localizationManager;
        public AudioSource audioSource;

        public UnityEvent StartDialogueEvent;//대화 시작 이벤트
        public UnityEvent EndDialogueEvent; //대화 종료 이벤트

        private BaseNodeData currentDialogueNodeData; //현재 대화 노드 데이터
        private BaseNodeData previousDialogueNodeData; // 이전 대화 노드 데이터

        private bool blockNextDialogue = false;
        private bool isEventRunning = false;

        private void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            dialogueBoxController.OnEventEnd += EndEvent;

        }

        public void SetupDialogue(DialogueScript dialogue)
        {
            dialogueContainer = dialogue;
        }

        public void StartDialogue()
        {

            if (dialogueContainer.StartNodeDatas.Count == 1) 
                RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else {
                RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); 
            }

            StartDialogueEvent.Invoke();
        }

        public void StartDialogue(DialogueScript dialogue)
        {

            dialogueContainer = dialogue;

            if (dialogueContainer.StartNodeDatas.Count == 1)  // 시작노드가 한개인 경우
                RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else { // 시작노드가 여러개인 경우
                RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)]));
            }


            StartDialogueEvent.Invoke(); // 대화 시작 이벤트 호출
        }

        public void StartDialogue(string ID)
        {

            // Try Get Start with ID
            bool withID = false;
            for (int i = 0; i < dialogueContainer.StartNodeDatas.Count; i++) {
                if (dialogueContainer.StartNodeDatas[i].startID == ID) {
                    RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[i]));
                    withID = true;
                }
            }
            if (!withID) {
                if (dialogueContainer.StartNodeDatas.Count == 1) RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
                else { RunNodeByType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }
            }

            StartDialogueEvent.Invoke();
        }


        /// <summary>
        /// 다음 대화 요청
        /// </summary>
        public void RunDialogue()
        {
            if (!blockNextDialogue) {
                RunNodeByType(currentDialogueNodeData);
            }
        }

        /// <summary>
        /// 지정된 id로 점프
        /// </summary>
        /// <param name="id">jump할 대화 id</param>
        public void JumpDialogue(string id)
        {
            if (!blockNextDialogue) {
                RunNodeByType(GetNodeByGuid(id));
            }
        }

        /// <summary>
        /// 이벤트 종료 이벤트 메서드
        /// </summary>
        public void EndEvent()
        {
            isEventRunning = false;
            currentDialogueNodeData = GetNextNode(currentDialogueNodeData);
        }

        /// <summary>
        /// 노드 실행
        /// </summary>
        /// <param name="_baseNodeData"></param>
        public void RunNodeByType(BaseNodeData _baseNodeData)
        {
            switch (_baseNodeData) {
                case StartNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case DialogueNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case ChoiceNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EventNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case RandomNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case IfNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EndNodeData nodeData:
                    RunNode(nodeData);
                    break;
                default:
                    Debug.LogWarning("Not Found Node Type");
                    break;
            }
        }

        private void RunNode(StartNodeData _nodeData)
        {
            StartDialogue();
        }

        /// <summary>
        /// 대화 노드
        /// </summary>
        private void RunNode(DialogueNodeData _nodeData)
        {
            if (dialogueBoxController.IsTyping()) {
                dialogueBoxController.StopDialogue();
                return;
            }

            previousDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            LocalizationEnum localizationEnum = localizationManager.SelectedLang();
           
            //캐릭터 세팅
            if (_nodeData.Character != null) {
                string characterName = _nodeData.Character.characterName.Find(text => text.languageEnum == localizationEnum).LanguageGenericType;
                Sprite sprite = _nodeData.Character.GetCharacterSprite(_nodeData.CharacterPos, _nodeData.CharacterType);
                CharacterPosition position = _nodeData.CharacterPos;

                characterCGController.SetCharacterByPosition(characterName, sprite, position);
            }

            // 텍스트 세팅
            string dialogText = _nodeData.TextType.Find(text => text.languageEnum == localizationEnum).LanguageGenericType;
            dialogueBoxController.TypeDialogue(null, dialogText);

        }

        /// <summary>
        /// 선택노드
        /// </summary>
        private void RunNode(ChoiceNodeData _nodeData)
        {
            previousDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;
            blockNextDialogue = true;

            LocalizationEnum localizationEnum = localizationManager.SelectedLang();

            // 텍스트
            string dialogText = _nodeData.TextType.Find(text => text.languageEnum == localizationEnum).LanguageGenericType;

            // 캐릭터
            if (_nodeData.Character != null) {
                string characterName = _nodeData.Character.characterName.Find(text => text.languageEnum == localizationEnum).LanguageGenericType;
            }

            //오디오
            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationEnum).LanguageGenericType != null) {
                audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationEnum).LanguageGenericType);
            }

            List<DialogueNodePort> _nodePorts = _nodeData.DialogueNodePorts;

            List<string> texts = new List<string>();
            List<UnityAction> unityActions = new List<UnityAction>();

            foreach (DialogueNodePort nodePort in _nodePorts) {
                texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                UnityAction tempAction = null;
                tempAction += () => {
                    RunNodeByType(GetNodeByGuid(nodePort.InputGuid));
                    blockNextDialogue = false;
                };
                unityActions.Add(tempAction);
                
            }
            choiceController.AddChoiceDialogue(texts, unityActions);

        }


        /// <summary>
        /// 랜덤 노드
        /// </summary>
        private void RunNode(RandomNodeData _nodeData)
        {
            RunNodeByType(GetNodeByGuid(_nodeData.DialogueNodePorts[Random.Range(0, _nodeData.DialogueNodePorts.Count)].InputGuid));
        }

        /// <summary>
        /// 조건 노드
        /// </summary>
        private void RunNode(IfNodeData _nodeData)
        {
            
        }

        /// <summary>
        /// 이벤트 노드
        /// </summary>
        private void RunNode(EventNodeData _nodeData)
        {

        }

        public void RunNode(EndNodeData _nodeData)
        {
            Debug.Log("노드 종료!");
            blockNextDialogue = true;
        }
    }
}
