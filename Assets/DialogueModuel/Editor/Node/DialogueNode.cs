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
        // ��ȭ �ؽ�Ʈ�� ����� Ŭ���� ������ ����Ʈ
        private List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
        private List<LanguageGeneric<AudioClip>> audioClip = new List<LanguageGeneric<AudioClip>>();

        // ��ȭ ĳ���Ϳ� ǥ�� �ð��� ������ ����
        private DialogueCharacterSO character = ScriptableObject.CreateInstance<DialogueCharacterSO>();
        private float durationShow = 10;

        // ��ȭ ��� ��Ʈ�� ������ ����Ʈ
        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        // ������Ƽ ����
        public List<LanguageGeneric<string>> Texts { get => texts; set => texts = value; }
        public List<LanguageGeneric<AudioClip>> AudioClip { get => audioClip; set => audioClip = value; }
        public DialogueCharacterSO Character { get => character; set => character = value; }
        public float DurationShow { get => durationShow; set => durationShow = value; }

        // UI ��Ҹ� ���� �ʵ� ����
        private TextField texts_Field;
        private ObjectField audioClips_Field;
        private TextField name_Field;
        private ObjectField character_Field;
        private FloatField duration_Field;

        // �⺻ ������
        public DialogueNode()
        {
        }

        // Ŀ���� ������: ����� �ʱ� ��ġ, ������ â, �׷��� �並 ����
        public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Dialogue"; // ��� ���� ����
            SetPosition(new Rect(_position, defualtNodeSize)); // ��� ��ġ ����
            nodeGuid = Guid.NewGuid().ToString(); // ���� �ĺ��� ����

            AddInputPort("Input", Port.Capacity.Multi); // �Է� ��Ʈ �߰�
            AddOutputPort("Output", Port.Capacity.Single); // ��� ��Ʈ �߰�

            // �� �� ���� �ؽ�Ʈ�� ����� Ŭ�� �ʱ�ȭ
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

            // ����� Ŭ�� �ʵ� ����
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

            // ĳ���� �ʵ� ����
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

            // �ؽ�Ʈ �ʵ� ����
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

            // ǥ�� �ð� �ʵ� ����
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

            // ��ȿ�� �˻� �����̳� �߰�
            AddValidationContainer();
        }

        // �� �ٽ� �ε��ϴ� �޼���
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

        // �ʵ忡 ���� �ε��ϴ� �޼���
        public override void LoadValueInToField()
        {
            texts_Field.SetValueWithoutNotify(texts.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            character_Field.SetValueWithoutNotify(character);
            duration_Field.SetValueWithoutNotify(durationShow);
        }

        // ��ȿ�� �˻縦 �����ϴ� �޼���
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
