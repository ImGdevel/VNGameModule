using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DialogueSystem.GlobalValue;
using DialogueSystem.Localization;

namespace DialogueSystem
{
    public class DialogueManager : DialogueGetData
    {
        [HideInInspector] public static DialogueManager Instance;
        public LocalizationManager localizationManager;

        [HideInInspector] public DialogueUIManager dialogueUIManager;
        public AudioSource audioSource;

        public UnityEvent StartDialogueEvent;//대화 시작 이벤트

        public UnityEvent EndDialogueEvent; //대화 종료 이벤트

        
        private BaseNodeData currentDialogueNodeData; //현재 대화 노드 데이터
        private BaseNodeData lastDialogueNodeData; // 마지막 대화 노드 데이터

        
        private TimerChoiceNodeData _nodeTimerInvoke; // 타이머 선택 노드
        private DialogueNodeData _nodeDialogueInvoke; // 대화 노드 데이터 
        private ChoiceNodeData _nodeChoiceInvoke; //대화 선택 노드 데이터

        float Timer;

        private void Awake()
        {
            Instance = this;
            dialogueUIManager= DialogueUIManager.Instance;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            Timer -= Time.deltaTime;
            if (Timer > 0) dialogueUIManager.TimerSlider.value = Timer;
        }

        public void SetupDialogue(DialogueScript dialogue)
        {
            dialogueContainer = dialogue; // 대화 스크립트 가져옴
        }

        public void StartDialogue(DialogueScript dialogue)
        {
            dialogueUIManager = DialogueUIManager.Instance;
            dialogueContainer = dialogue;

            if (dialogueContainer.StartNodeDatas.Count == 1)  // 시작노드가 한개인 경우
                CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else { // 시작노드가 여러개인 경우
                CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)]));   
            }

            dialogueUIManager.dialogueCanvas .SetActive(true); // 대화 UI 활성화
            StartDialogueEvent.Invoke(); // 대화 시작 이벤트 호출
        }

        public void StartDialogue(string ID)
        {
            dialogueUIManager = DialogueUIManager.Instance;

            // Try Get Start with ID
            bool withID = false;
            for(int i = 0; i < dialogueContainer.StartNodeDatas.Count; i++)
            {
                if(dialogueContainer.StartNodeDatas[i].startID == ID)
                {
                    CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[i]));
                    withID = true;
                }
            }
            if (!withID)
            {
                if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
                else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }
            }

            dialogueUIManager.dialogueCanvas.SetActive(true);
            StartDialogueEvent.Invoke();
        }

        public void StartDialogue()
        {
            dialogueUIManager= DialogueUIManager.Instance;

            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas.SetActive(true);
            StartDialogueEvent.Invoke();
        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {
            switch (_baseNodeData)
            {
                case StartNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case DialogueNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case ChoiceNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case TimerChoiceNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EventNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EndNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case RandomNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case IfNodeData nodeData:
                    RunNode(nodeData);
                    break;
                default:
                    break;
            }
        }


        private void RunNode(StartNodeData _nodeData)
        {
            CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
        }

        private void RunNode(RandomNodeData _nodeData)
        {
            CheckNodeType(GetNodeByGuid(_nodeData.DialogueNodePorts[Random.Range(0, _nodeData.DialogueNodePorts.Count)].InputGuid));
        }

        private void RunNode(IfNodeData _nodeData)
        {
            string ValueName = _nodeData.ValueName;
            GlobalValueIFOperations Operations = _nodeData.Operations;
            string OperationValue = _nodeData.OperationValue;

            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();

            Debug.Log("XXXX" + _nodeData.TrueGUID + "XXXX");
            CheckNodeType(GetNodeByGuid(manager.IfTrue(ValueName, Operations, OperationValue) ? _nodeData.TrueGUID : _nodeData.FalseGUID));
        }

        private void RunNode(DialogueNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();

            // Gloval Value Multiline
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null && _nodeData.Character.UseGlobalValue) { 
                dialogueUIManager.ResetText(""); 
                dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{manager.Get<string>(GlobalValueType.String, _nodeData.Character.CustomizedName.ValueName)}</color>"; 
            }
            // Normal Multiline
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) { 
                dialogueUIManager.ResetText(""); 
                dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>"; 
            }
            // No Change Character Multiline
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) { 
                dialogueUIManager.ResetText(""); 
            }
            // Global Value Inline
            else if (_nodeData.Character != null && _nodeData.Character.UseGlobalValue) 
                dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{manager.Get<string>(GlobalValueType.String, _nodeData.Character.CustomizedName.ValueName)}: </color>");
            // Normal Inline
            else if (_nodeData.Character != null) 
                dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            // Last Change
            else dialogueUIManager.ResetText("");

            dialogueUIManager.SetFullText($"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}");

            // Character Avatar
            dialogueUIManager.SpriteLeft.SetActive(false); dialogueUIManager.SpriteRight.SetActive(false);
            if (_nodeData.CharacterPos == CharacterPosition.Left && _nodeData.Character != null) { 
                dialogueUIManager.SpriteLeft.SetActive(true); dialogueUIManager.SpriteLeft.GetComponent<Image>().sprite = _nodeData.Character.GetCharacterSprite(_nodeData.CharacterPos, _nodeData.CharacterType); 
            }
            if (_nodeData.CharacterPos == CharacterPosition.Right && _nodeData.Character != null) { 
                dialogueUIManager.SpriteRight.SetActive(true); dialogueUIManager.SpriteRight.GetComponent<Image>().sprite = _nodeData.Character.GetCharacterSprite(_nodeData.CharacterPos, _nodeData.CharacterType); 
            }

            dialogueUIManager.SkipButton.SetActive(true);
            MakeButtons(new List<DialogueNodePort>());

            if(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

            _nodeDialogueInvoke = _nodeData;

            IEnumerator tmp() { 
                yield return new WaitForSeconds(_nodeData.Duration); DialogueNode_NextNode(); 
            }
            if(_nodeData.Duration != 0) 
                StartCoroutine(tmp());
        }

        private void RunNode(ChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();

            // Gloval Value Multiline
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null && _nodeData.Character.UseGlobalValue) { dialogueUIManager.ResetText(""); dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{manager.Get<string>(GlobalValueType.String, _nodeData.Character.CustomizedName.ValueName)}</color>"; }
            // Normal Multiline
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null) { dialogueUIManager.ResetText(""); dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>"; }
            // Global Value Inline
            else if (_nodeData.Character != null && _nodeData.Character.UseGlobalValue) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{manager.Get<string>(GlobalValueType.String, _nodeData.Character.CustomizedName.ValueName)}: </color>");
            // Normal Inline
            else if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            // Last Change
            else dialogueUIManager.ResetText("");

            dialogueUIManager.SetFullText($"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}");

            // Character Avatar
            dialogueUIManager.SpriteLeft.SetActive(false); dialogueUIManager.SpriteRight.SetActive(false);
            if (_nodeData.AvatarPos == CharacterPosition.Left && _nodeData.Character != null) { dialogueUIManager.SpriteLeft.SetActive(true); dialogueUIManager.SpriteLeft.GetComponent<Image>().sprite = _nodeData.Character.GetCharacterSprite(_nodeData.AvatarPos, _nodeData.AvatarType); }
            if(_nodeData.AvatarPos == CharacterPosition.Right && _nodeData.Character != null) { dialogueUIManager.SpriteRight.SetActive(true); dialogueUIManager.SpriteRight.GetComponent<Image>().sprite = _nodeData.Character.GetCharacterSprite(_nodeData.AvatarPos, _nodeData.AvatarType); }

            dialogueUIManager.SkipButton.SetActive(true);
            MakeButtons(new List<DialogueNodePort>());

            _nodeChoiceInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); ChoiceNode_GenerateChoice(); }
            StartCoroutine(tmp());

            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
        }

        private void RunNode(EventNodeData _nodeData)
        {
            foreach (var item in _nodeData.EventScriptableObjects)
            {
                if (item.DialogueEvent != null)
                {
                    item.DialogueEvent.RunEvent();
                }
            }
            CheckNodeType(GetNextNode(_nodeData));
        }

        private void RunNode(EndNodeData _nodeData)
        {
            switch (_nodeData.EndNodeType)
            {
                case EndNodeType.End:
                    dialogueUIManager.dialogueCanvas.SetActive(false);
                    EndDialogueEvent.Invoke();
                    break;
                case EndNodeType.Repeat:
                    CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                    break;
                case EndNodeType.GoBack:
                    CheckNodeType(GetNodeByGuid(lastDialogueNodeData.NodeGuid));
                    break;
                case EndNodeType.ReturnToStart:
                    CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0,dialogueContainer.StartNodeDatas.Count)]));
                    break;
                case EndNodeType.StartDialogue:
                    StartDialogue(_nodeData.Dialogue);
                    break;
                default:
                    break;
            }
        }
        private void RunNode(TimerChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();

            // Gloval Value Multiline
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null && _nodeData.Character.UseGlobalValue) { dialogueUIManager.ResetText(""); dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{manager.Get<string>(GlobalValueType.String, _nodeData.Character.CustomizedName.ValueName)}</color>"; }
            // Normal Multiline
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null) { dialogueUIManager.ResetText(""); dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>"; }
            // Global Value Inline
            else if (_nodeData.Character != null && _nodeData.Character.UseGlobalValue) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{manager.Get<string>(GlobalValueType.String, _nodeData.Character.CustomizedName.ValueName)}: </color>");
            // Normal Inline
            else if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            // Last Change
            else dialogueUIManager.ResetText("");

            dialogueUIManager.SetFullText($"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}");

            // Character Avatar
            dialogueUIManager.SpriteLeft.SetActive(false); dialogueUIManager.SpriteRight.SetActive(false);
            if (_nodeData.AvatarPos == CharacterPosition.Left && _nodeData.Character != null) { dialogueUIManager.SpriteLeft.SetActive(true); dialogueUIManager.SpriteLeft.GetComponent<Image>().sprite = _nodeData.Character.GetCharacterSprite(_nodeData.AvatarPos, _nodeData.AvatarType); }
            if (_nodeData.AvatarPos == CharacterPosition.Right && _nodeData.Character != null) { dialogueUIManager.SpriteRight.SetActive(true); dialogueUIManager.SpriteRight.GetComponent<Image>().sprite = _nodeData.Character.GetCharacterSprite(_nodeData.AvatarPos, _nodeData.AvatarType); }

            dialogueUIManager.SkipButton.SetActive(true);
            MakeButtons(new List<DialogueNodePort>());

            _nodeTimerInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSecondsRealtime(_nodeData.Duration); TimerNode_GenerateChoice(); }
            StartCoroutine(tmp());

            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

        }

        private void MakeButtons(List<DialogueNodePort> _nodePorts)
        {
            List<string> texts = new List<string>();
            List<UnityAction> unityActions = new List<UnityAction>();

            foreach (DialogueNodePort nodePort in _nodePorts)
            {
                texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                UnityAction tempAction = null;
                tempAction += () =>
                {
                    CheckNodeType(GetNodeByGuid(nodePort.InputGuid));
                };
                unityActions.Add(tempAction);
            }

            dialogueUIManager.SetButtons(texts, unityActions, false);
        }

        private void MakeTimerButtons(List<DialogueNodePort> _nodePorts, float ShowDuration, float timer)
        {
            List<string> texts = new List<string>();
            List<UnityAction> unityActions = new List<UnityAction>();

            IEnumerator tmp() { yield return new WaitForSeconds(timer); TimerNode_NextNode(); }
            StartCoroutine(tmp());

            foreach (DialogueNodePort nodePort in _nodePorts)
            {
                if (nodePort != _nodePorts[0])
                {
                    texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                    UnityAction tempAction = null;
                    tempAction += () =>
                    {
                        StopAllCoroutines();
                        CheckNodeType(GetNodeByGuid(nodePort.InputGuid));
                    };
                    unityActions.Add(tempAction);
                }
            }

            dialogueUIManager.SetButtons(texts, unityActions, true);
            dialogueUIManager.TimerSlider.maxValue = timer; Timer = timer;
        }

        void DialogueNode_NextNode() { CheckNodeType(GetNextNode(_nodeDialogueInvoke)); }
        void ChoiceNode_GenerateChoice() { 
            MakeButtons(_nodeChoiceInvoke.DialogueNodePorts);
            dialogueUIManager.SkipButton.SetActive(false);
        }
        void TimerNode_GenerateChoice() { MakeTimerButtons(_nodeTimerInvoke.DialogueNodePorts, _nodeTimerInvoke.Duration, _nodeTimerInvoke.time);
            dialogueUIManager.SkipButton.SetActive(false);
        }
        void TimerNode_NextNode() { CheckNodeType(GetNextNode(_nodeTimerInvoke)); }

        public void SkipDialogue()
        {
            StopAllCoroutines();

            switch (currentDialogueNodeData)
            {
                case DialogueNodeData nodeData:
                    DialogueNode_NextNode();
                    break;
                case ChoiceNodeData nodeData:
                    ChoiceNode_GenerateChoice();
                    break;
                case TimerChoiceNodeData nodeData:
                    TimerNode_GenerateChoice();
                    break;
                default:
                    break;
            }
        }

        public void ForceEndDialog()
        {
            dialogueUIManager.dialogueCanvas.SetActive(false);
            EndDialogueEvent.Invoke();
        }
    }
}
