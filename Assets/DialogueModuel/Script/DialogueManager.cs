using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MeetAndTalk.GlobalValue;
using MeetAndTalk.Localization;

namespace MeetAndTalk
{
    public class DialogueManager : DialogueGetData
    {
        [HideInInspector] public static DialogueManager Instance;
        public LocalizationManager localizationManager;

        [HideInInspector] public DialogueUIManager dialogueUIManager;
        public AudioSource audioSource;

        public UnityEvent StartDialogueEvent;
        public UnityEvent EndDialogueEvent;

        private BaseNodeData currentDialogueNodeData;
        private BaseNodeData lastDialogueNodeData;

        private TimerChoiceNodeData _nodeTimerInvoke;
        private DialogueNodeData _nodeDialogueInvoke;
        private DialogueChoiceNodeData _nodeChoiceInvoke;

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

        public void SetupDialogue(DialogueContainerSO dialogue)
        {
            dialogueContainer = dialogue;
        }

        public void StartDialogue(DialogueContainerSO dialogue)
        {
            dialogueUIManager = DialogueUIManager.Instance;
            dialogueContainer = dialogue;

            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas .SetActive(true);
            StartDialogueEvent.Invoke();
        }

        public void StartDialogue(string ID)
        {
            StartDialogue();
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
                case DialogueChoiceNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EndNodeData nodeData:
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

        private void RunNode(DialogueNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            // Normal Multiline
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) { dialogueUIManager.ResetText(""); dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>"; }
            // No Change Character Multiline
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) { dialogueUIManager.ResetText(""); }
            // Normal Inline
            else if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            // Last Change
            else dialogueUIManager.ResetText("");

            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            dialogueUIManager.SkipButton.SetActive(true);
            MakeButtons(new List<DialogueNodePort>());

            if(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

            _nodeDialogueInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); DialogueNode_NextNode(); }
            if(_nodeData.Duration != 0) StartCoroutine(tmp());
        }
        private void RunNode(DialogueChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            // Normal Multiline
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) { dialogueUIManager.ResetText(""); dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>"; }
            // No Change Character Multiline
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) { dialogueUIManager.ResetText(""); }
            // Normal Inline
            else if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            // Last Change
            else dialogueUIManager.ResetText("");

            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            dialogueUIManager.SkipButton.SetActive(true);
            MakeButtons(new List<DialogueNodePort>());

            _nodeChoiceInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); ChoiceNode_GenerateChoice(); }
            StartCoroutine(tmp());

            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
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
                default:
                    break;
            }
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


        void DialogueNode_NextNode() { CheckNodeType(GetNextNode(_nodeDialogueInvoke)); }
        void ChoiceNode_GenerateChoice() { MakeButtons(_nodeChoiceInvoke.DialogueNodePorts);
            dialogueUIManager.SkipButton.SetActive(false);
        }

        public void SkipDialogue()
        {
            StopAllCoroutines();

            switch (currentDialogueNodeData)
            {
                case DialogueNodeData nodeData:
                    DialogueNode_NextNode();
                    break;
                case DialogueChoiceNodeData nodeData:
                    ChoiceNode_GenerateChoice();
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
