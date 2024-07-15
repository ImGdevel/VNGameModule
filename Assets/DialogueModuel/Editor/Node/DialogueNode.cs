using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using DialogueSystem.Editor;
using DialogueSystem.Localization;
using DialogueSystem.Event;

namespace DialogueSystem.Nodes
{
    public class DialogueNode : BaseNode
    {
        // 대화 텍스트와 오디오 클립을 저장할 리스트
        private List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
        private List<LanguageGeneric<AudioClip>> audioClip = new List<LanguageGeneric<AudioClip>>();

        // 대화 캐릭터와 표시 시간을 저장할 변수
        private DialogueCharacterSO character = ScriptableObject.CreateInstance<DialogueCharacterSO>();
        private float durationShow = 10;

        // 대화 노드 포트를 저장할 리스트
        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        // 프로퍼티 정의
        public List<LanguageGeneric<string>> Texts { get => texts; set => texts = value; }
        public List<LanguageGeneric<AudioClip>> AudioClip { get => audioClip; set => audioClip = value; }
        public DialogueCharacterSO Character { get => character; set => character = value; }
        public float DurationShow { get => durationShow; set => durationShow = value; }

        // UI 요소를 위한 필드 정의
        private TextField texts_Field;
        private ObjectField audioClips_Field;
        private TextField name_Field;
        private ObjectField character_Field;
        private FloatField duration_Field;

        // 기본 생성자
        public DialogueNode()
        {
        }

        // 커스텀 생성자: 노드의 초기 위치, 에디터 창, 그래프 뷰를 설정
        public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Dialogue"; // 노드 제목 설정
            SetPosition(new Rect(_position, defualtNodeSize)); // 노드 위치 설정
            nodeGuid = Guid.NewGuid().ToString(); // 고유 식별자 생성

            AddInputPort("Input", Port.Capacity.Multi); // 입력 포트 추가
            AddOutputPort("Output", Port.Capacity.Single); // 출력 포트 추가

            // 각 언어에 대해 텍스트와 오디오 클립 초기화
            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum))) {
                texts.Add(new LanguageGeneric<string> {
                    languageEnum = language,
                    LanguageGenericType = ""
                });
                AudioClip.Add(new LanguageGeneric<AudioClip> {
                    languageEnum = language,
                    LanguageGenericType = null
                });
            }

            // 오디오 클립 필드 설정
            Label label_audio = new Label("Voice Audio Clip");
            label_audio.AddToClassList("label_audio");
            label_audio.AddToClassList("Label");
            mainContainer.Add(label_audio);
            audioClips_Field = new ObjectField() {
                objectType = typeof(AudioClip),
                allowSceneObjects = false,
                value = audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType,
            };
            audioClips_Field.RegisterValueChangedCallback(value => {
                audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue as AudioClip;
            });
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            mainContainer.Add(audioClips_Field);

            // 캐릭터 필드 설정
            Label label_character = new Label("Character SO");
            label_character.AddToClassList("label_name");
            label_character.AddToClassList("Label");
            mainContainer.Add(label_character);
            character_Field = new ObjectField() {
                objectType = typeof(DialogueCharacterSO),
                allowSceneObjects = false,
            };
            character_Field.RegisterValueChangedCallback(value => {
                character = value.newValue as DialogueCharacterSO;
            });
            character_Field.SetValueWithoutNotify(character);
            mainContainer.Add(character_Field);

            // 텍스트 필드 설정
            Label label_texts = new Label("Displayed Text");
            label_texts.AddToClassList("label_texts");
            label_texts.AddToClassList("Label");
            mainContainer.Add(label_texts);

            texts_Field = new TextField("");
            texts_Field.RegisterValueChangedCallback(value => {
                texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue;
            });
            texts_Field.SetValueWithoutNotify(texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            texts_Field.multiline = true;
            texts_Field.AddToClassList("TextBox");
            mainContainer.Add(texts_Field);

            // 표시 시간 필드 설정
            Label label_duration = new Label("Display Time");
            label_duration.AddToClassList("label_duration");
            label_duration.AddToClassList("Label");
            mainContainer.Add(label_duration);

            duration_Field = new FloatField("");
            duration_Field.RegisterValueChangedCallback(value => {
                durationShow = value.newValue;
            });
            duration_Field.SetValueWithoutNotify(durationShow);
            duration_Field.AddToClassList("TextDuration");
            mainContainer.Add(duration_Field);

            // 유효성 검사 컨테이너 추가
            AddValidationContainer();
        }

        // 언어를 다시 로드하는 메서드
        public void ReloadLanguage()
        {
            texts_Field.RegisterValueChangedCallback(value => {
                texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue;
            });
            texts_Field.SetValueWithoutNotify(texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);

            audioClips_Field.RegisterValueChangedCallback(value => {
                audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue as AudioClip;
            });
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
        }

        // 필드에 값을 로드하는 메서드
        public override void LoadValueInToField()
        {
            texts_Field.SetValueWithoutNotify(texts.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            character_Field.SetValueWithoutNotify(character);
            duration_Field.SetValueWithoutNotify(durationShow);
        }

        // 유효성 검사를 설정하는 메서드
        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port input = inputContainer.Query<Port>().First();
            if (!input.connected) warning.Add("Node cannot be called");

            Port output = outputContainer.Query<Port>().First();
            if (!output.connected) error.Add("Output does not lead to any node");

            if (durationShow < 1 && durationShow != 0) warning.Add("Short time for Make Decision");
            for (int i = 0; i < Texts.Count; i++) {
                if (Texts[i].LanguageGenericType == "")
                    warning.Add($"No Text for {Texts[i].languageEnum} Language");
            }

            ErrorList = error;
            WarningList = warning;
        }
    }
}
