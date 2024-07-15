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
        [HideInInspector] public static DialogueManager Instance; // 싱글톤 인스턴스
        public LocalizationManager localizationManager; // LocalizationManager 인스턴스

        [HideInInspector] public DialogueUIManager dialogueUIManager; // DialogueUIManager 인스턴스
        public AudioSource audioSource; // 오디오 소스 컴포넌트

        public UnityEvent StartDialogueEvent; // 대화 시작 이벤트
        public UnityEvent EndDialogueEvent; // 대화 종료 이벤트

        private BaseNodeData currentDialogueNodeData; // 현재 대화 노드 데이터
        private BaseNodeData lastDialogueNodeData; // 마지막 대화 노드 데이터

        private TimerChoiceNodeData _nodeTimerInvoke; // 타이머 선택 노드 데이터
        private DialogueNodeData _nodeDialogueInvoke; // 대화 노드 데이터
        private DialogueChoiceNodeData _nodeChoiceInvoke; // 대화 선택 노드 데이터

        float Timer; // 타이머 변수

        private void Awake()
        {
            Instance = this; // 싱글톤 인스턴스 설정
            dialogueUIManager = DialogueUIManager.Instance; // DialogueUIManager 인스턴스 설정
            audioSource = GetComponent<AudioSource>(); // 오디오 소스 컴포넌트 가져오기
        }

        private void Update()
        {
            Timer -= Time.deltaTime; // 타이머 감소
            if (Timer > 0) dialogueUIManager.TimerSlider.value = Timer; // 타이머 슬라이더 업데이트
        }

        public void SetupDialogue(DialogueContainerSO dialogue)
        {
            dialogueContainer = dialogue; // 대화 컨테이너 설정
        }

        public void StartDialogue(DialogueContainerSO dialogue)
        {
            dialogueUIManager = DialogueUIManager.Instance; // DialogueUIManager 인스턴스 설정
            dialogueContainer = dialogue; // 대화 컨테이너 설정

            // 시작 노드 데이터가 하나일 경우
            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            // 시작 노드 데이터가 여러 개일 경우 무작위로 선택
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas.SetActive(true); // 대화 캔버스 활성화
            StartDialogueEvent.Invoke(); // 대화 시작 이벤트 호출
        }

        public void StartDialogue(string ID)
        {
            StartDialogue(); // ID로 대화 시작
        }

        public void StartDialogue()
        {
            dialogueUIManager = DialogueUIManager.Instance; // DialogueUIManager 인스턴스 설정

            // 시작 노드 데이터가 하나일 경우
            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            // 시작 노드 데이터가 여러 개일 경우 무작위로 선택
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas.SetActive(true); // 대화 캔버스 활성화
            StartDialogueEvent.Invoke(); // 대화 시작 이벤트 호출
        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {
            // 노드 타입에 따라 실행할 함수 선택
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
            CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0])); // 다음 노드 실행
        }

        private void RunNode(DialogueNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData; // 마지막 노드 데이터 업데이트
            currentDialogueNodeData = _nodeData; // 현재 노드 데이터 업데이트

            // 캐릭터 이름과 대화 텍스트 설정
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

            // 전체 대화 텍스트 설정
            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            dialogueUIManager.SkipButton.SetActive(true); // 스킵 버튼 활성화
            MakeButtons(new List<DialogueNodePort>()); // 버튼 생성

            // 오디오 클립 재생
            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null)
                audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

            _nodeDialogueInvoke = _nodeData; // 대화 노드 데이터 설정

            // 노드 지속 시간 후 다음 노드로 이동
            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); DialogueNode_NextNode(); }
            if (_nodeData.Duration != 0) StartCoroutine(tmp());
        }

        private void RunNode(DialogueChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData; // 마지막 노드 데이터 업데이트
            currentDialogueNodeData = _nodeData; // 현재 노드 데이터 업데이트

            // 캐릭터 이름과 대화 텍스트 설정
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

            // 전체 대화 텍스트 설정
            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            dialogueUIManager.SkipButton.SetActive(true); // 스킵 버튼 활성화
            MakeButtons(new List<DialogueNodePort>()); // 버튼 생성

            _nodeChoiceInvoke = _nodeData; // 대화 선택 노드 데이터 설정

            // 노드 지속 시간 후 선택지 생성
            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); ChoiceNode_GenerateChoice(); }
            StartCoroutine(tmp());

            // 오디오 클립 재생
            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null)
                audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
        }

        private void RunNode(EndNodeData _nodeData)
        {
            // 엔드 노드 타입에 따라 다른 동작 수행
            switch (_nodeData.EndNodeType) {
                case EndNodeType.End:
                    dialogueUIManager.dialogueCanvas.SetActive(false); // 대화 캔버스 비활성화
                    EndDialogueEvent.Invoke(); // 대화 종료 이벤트 호출
                    break;
                case EndNodeType.Repeat:
                    CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid)); // 현재 노드 반복 실행
                    break;
                case EndNodeType.GoBack:
                    CheckNodeType(GetNodeByGuid(lastDialogueNodeData.NodeGuid)); // 마지막 노드로 돌아가기
                    break;
                case EndNodeType.ReturnToStart:
                    CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); // 시작 노드로 돌아가기
                    break;
                default:
                    break;
            }
        }

        private void MakeButtons(List<DialogueNodePort> _nodePorts)
        {
            List<string> texts = new List<string>(); // 버튼 텍스트 리스트
            List<UnityAction> unityActions = new List<UnityAction>(); // 버튼 액션 리스트

            // 각 노드 포트에 대해 버튼 텍스트와 액션 설정
            foreach (DialogueNodePort nodePort in _nodePorts) {
                texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                UnityAction tempAction = null;
                tempAction += () => {
                    CheckNodeType(GetNodeByGuid(nodePort.InputGuid)); // 노드 포트의 GUID를 사용하여 다음 노드로 이동
                };
                unityActions.Add(tempAction);
            }

            dialogueUIManager.SetButtons(texts, unityActions, false); // 버튼 설정
        }

        void DialogueNode_NextNode() { CheckNodeType(GetNextNode(_nodeDialogueInvoke)); } // 다음 대화 노드 실행
        void ChoiceNode_GenerateChoice()
        {
            MakeButtons(_nodeChoiceInvoke.DialogueNodePorts); // 선택지 생성
            dialogueUIManager.SkipButton.SetActive(false); // 스킵 버튼 비활성화
        }

        public void SkipDialogue()
        {
            StopAllCoroutines(); // 모든 코루틴 중지

            // 현재 대화 노드 데이터에 따라 다음 동작 수행
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
            dialogueUIManager.dialogueCanvas.SetActive(false); // 대화 캔버스 비활성화
            EndDialogueEvent.Invoke(); // 대화 종료 이벤트 호출
        }
    }
}
