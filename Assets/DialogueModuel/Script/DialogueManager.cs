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
        [HideInInspector] public static DialogueManager Instance; // �̱��� �ν��Ͻ�
        public LocalizationManager localizationManager; // LocalizationManager �ν��Ͻ�

        [HideInInspector] public DialogueUIManager dialogueUIManager; // DialogueUIManager �ν��Ͻ�
        public AudioSource audioSource; // ����� �ҽ� ������Ʈ

        public UnityEvent StartDialogueEvent; // ��ȭ ���� �̺�Ʈ
        public UnityEvent EndDialogueEvent; // ��ȭ ���� �̺�Ʈ

        private BaseNodeData currentDialogueNodeData; // ���� ��ȭ ��� ������
        private BaseNodeData lastDialogueNodeData; // ������ ��ȭ ��� ������

        private TimerChoiceNodeData _nodeTimerInvoke; // Ÿ�̸� ���� ��� ������
        private DialogueNodeData _nodeDialogueInvoke; // ��ȭ ��� ������
        private DialogueChoiceNodeData _nodeChoiceInvoke; // ��ȭ ���� ��� ������

        float Timer; // Ÿ�̸� ����

        private void Awake()
        {
            Instance = this; // �̱��� �ν��Ͻ� ����
            dialogueUIManager = DialogueUIManager.Instance; // DialogueUIManager �ν��Ͻ� ����
            audioSource = GetComponent<AudioSource>(); // ����� �ҽ� ������Ʈ ��������
        }

        private void Update()
        {
            Timer -= Time.deltaTime; // Ÿ�̸� ����
            if (Timer > 0) dialogueUIManager.TimerSlider.value = Timer; // Ÿ�̸� �����̴� ������Ʈ
        }

        public void SetupDialogue(DialogueContainerSO dialogue)
        {
            dialogueContainer = dialogue; // ��ȭ �����̳� ����
        }

        public void StartDialogue(DialogueContainerSO dialogue)
        {
            dialogueUIManager = DialogueUIManager.Instance; // DialogueUIManager �ν��Ͻ� ����
            dialogueContainer = dialogue; // ��ȭ �����̳� ����

            // ���� ��� �����Ͱ� �ϳ��� ���
            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            // ���� ��� �����Ͱ� ���� ���� ��� �������� ����
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas.SetActive(true); // ��ȭ ĵ���� Ȱ��ȭ
            StartDialogueEvent.Invoke(); // ��ȭ ���� �̺�Ʈ ȣ��
        }

        public void StartDialogue(string ID)
        {
            StartDialogue(); // ID�� ��ȭ ����
        }

        public void StartDialogue()
        {
            dialogueUIManager = DialogueUIManager.Instance; // DialogueUIManager �ν��Ͻ� ����

            // ���� ��� �����Ͱ� �ϳ��� ���
            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            // ���� ��� �����Ͱ� ���� ���� ��� �������� ����
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas.SetActive(true); // ��ȭ ĵ���� Ȱ��ȭ
            StartDialogueEvent.Invoke(); // ��ȭ ���� �̺�Ʈ ȣ��
        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {
            // ��� Ÿ�Կ� ���� ������ �Լ� ����
            switch (_baseNodeData) {
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
            CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0])); // ���� ��� ����
        }

        private void RunNode(DialogueNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData; // ������ ��� ������ ������Ʈ
            currentDialogueNodeData = _nodeData; // ���� ��� ������ ������Ʈ

            // ĳ���� �̸��� ��ȭ �ؽ�Ʈ ����
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) {
                dialogueUIManager.ResetText("");
                dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>";
            }
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) {
                dialogueUIManager.ResetText("");
            }
            else if (_nodeData.Character != null) {
                dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            }
            else {
                dialogueUIManager.ResetText("");
            }

            // ��ü ��ȭ �ؽ�Ʈ ����
            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            dialogueUIManager.SkipButton.SetActive(true); // ��ŵ ��ư Ȱ��ȭ
            MakeButtons(new List<DialogueNodePort>()); // ��ư ����

            // ����� Ŭ�� ���
            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null)
                audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

            _nodeDialogueInvoke = _nodeData; // ��ȭ ��� ������ ����

            // ��� ���� �ð� �� ���� ���� �̵�
            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); DialogueNode_NextNode(); }
            if (_nodeData.Duration != 0) StartCoroutine(tmp());
        }

        private void RunNode(DialogueChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData; // ������ ��� ������ ������Ʈ
            currentDialogueNodeData = _nodeData; // ���� ��� ������ ������Ʈ

            // ĳ���� �̸��� ��ȭ �ؽ�Ʈ ����
            if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) {
                dialogueUIManager.ResetText("");
                dialogueUIManager.nameTextBox.text = $"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}</color>";
            }
            else if (dialogueUIManager.showSeparateName && dialogueUIManager.nameTextBox != null && _nodeData.Character != null) {
                dialogueUIManager.ResetText("");
            }
            else if (_nodeData.Character != null) {
                dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            }
            else {
                dialogueUIManager.ResetText("");
            }

            // ��ü ��ȭ �ؽ�Ʈ ����
            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            dialogueUIManager.SkipButton.SetActive(true); // ��ŵ ��ư Ȱ��ȭ
            MakeButtons(new List<DialogueNodePort>()); // ��ư ����

            _nodeChoiceInvoke = _nodeData; // ��ȭ ���� ��� ������ ����

            // ��� ���� �ð� �� ������ ����
            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); ChoiceNode_GenerateChoice(); }
            StartCoroutine(tmp());

            // ����� Ŭ�� ���
            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null)
                audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
        }

        private void RunNode(EndNodeData _nodeData)
        {
            // ���� ��� Ÿ�Կ� ���� �ٸ� ���� ����
            switch (_nodeData.EndNodeType) {
                case EndNodeType.End:
                    dialogueUIManager.dialogueCanvas.SetActive(false); // ��ȭ ĵ���� ��Ȱ��ȭ
                    EndDialogueEvent.Invoke(); // ��ȭ ���� �̺�Ʈ ȣ��
                    break;
                case EndNodeType.Repeat:
                    CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid)); // ���� ��� �ݺ� ����
                    break;
                case EndNodeType.GoBack:
                    CheckNodeType(GetNodeByGuid(lastDialogueNodeData.NodeGuid)); // ������ ���� ���ư���
                    break;
                case EndNodeType.ReturnToStart:
                    CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); // ���� ���� ���ư���
                    break;
                default:
                    break;
            }
        }

        private void MakeButtons(List<DialogueNodePort> _nodePorts)
        {
            List<string> texts = new List<string>(); // ��ư �ؽ�Ʈ ����Ʈ
            List<UnityAction> unityActions = new List<UnityAction>(); // ��ư �׼� ����Ʈ

            // �� ��� ��Ʈ�� ���� ��ư �ؽ�Ʈ�� �׼� ����
            foreach (DialogueNodePort nodePort in _nodePorts) {
                texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                UnityAction tempAction = null;
                tempAction += () => {
                    CheckNodeType(GetNodeByGuid(nodePort.InputGuid)); // ��� ��Ʈ�� GUID�� ����Ͽ� ���� ���� �̵�
                };
                unityActions.Add(tempAction);
            }

            dialogueUIManager.SetButtons(texts, unityActions, false); // ��ư ����
        }

        void DialogueNode_NextNode() { CheckNodeType(GetNextNode(_nodeDialogueInvoke)); } // ���� ��ȭ ��� ����
        void ChoiceNode_GenerateChoice()
        {
            MakeButtons(_nodeChoiceInvoke.DialogueNodePorts); // ������ ����
            dialogueUIManager.SkipButton.SetActive(false); // ��ŵ ��ư ��Ȱ��ȭ
        }

        public void SkipDialogue()
        {
            StopAllCoroutines(); // ��� �ڷ�ƾ ����

            // ���� ��ȭ ��� �����Ϳ� ���� ���� ���� ����
            switch (currentDialogueNodeData) {
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
            dialogueUIManager.dialogueCanvas.SetActive(false); // ��ȭ ĵ���� ��Ȱ��ȭ
            EndDialogueEvent.Invoke(); // ��ȭ ���� �̺�Ʈ ȣ��
        }
    }
}
