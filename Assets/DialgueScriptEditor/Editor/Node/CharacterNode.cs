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
    public class CharacterNode : BaseNode
    {
        private DialogueCharacter character = null;
        private float durationShow = 10;

        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        public DialogueCharacter Character { get => character; set => character = value; }
        public float DurationShow { get => durationShow; set => durationShow = value; }

        private TextField texts_Field;
        private ObjectField audioClips_Field;
        private TextField name_Field;
        private ObjectField character_Field;
        private FloatField duration_Field;

        public CharacterEvent characterEvent;
        private EnumField CharacterEventField;

        public CharacterPosition characterPosition;
        public CharacterType characterType;
        private EnumField CharacterPositionField;
        private EnumField CharacterTypeField;


        public CharacterNode()
        {
        }

        public CharacterNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Character Event";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            AddValidationContainer();


            /* Character CLIPS */
            Label label_character = new Label("Character CG");
            label_character.AddToClassList("label_name");
            label_character.AddToClassList("Label");
            mainContainer.Add(label_character);
            character_Field = new ObjectField() {
                objectType = typeof(DialogueCharacter),
                allowSceneObjects = false,
            };
            character_Field.SetValueWithoutNotify(character);
            mainContainer.Add(character_Field);


            /* AVATAR FIELDS IN FOLDOUT */
            Label label_character_evnet = new Label("Character Event");
            label_character_evnet.AddToClassList("label_event");
            label_character_evnet.AddToClassList("Label");
            mainContainer.Add(label_character_evnet);

            //CharacterEventField = new EnumField("Event Type", characterEvent);
            //CharacterEventField.RegisterValueChangedCallback(value => {
            //    characterEvent = (CharacterEvent)value.newValue;
            //});
            //CharacterEventField.SetValueWithoutNotify(characterEvent);
            //mainContainer.Add(CharacterEventField);

            /* Character Position*/
            CharacterPositionField = new EnumField("Position", characterPosition);
            CharacterPositionField.RegisterValueChangedCallback(value => {
                characterPosition = (CharacterPosition)value.newValue;
            });
            CharacterPositionField.SetValueWithoutNotify(characterPosition);
            mainContainer.Add(CharacterPositionField);

            /* Character Emotion*/
            CharacterTypeField = new EnumField("Emotion", characterType);
            CharacterTypeField.RegisterValueChangedCallback(value => {
                characterType = (CharacterType)value.newValue;
            });
            CharacterTypeField.SetValueWithoutNotify(characterType);
            mainContainer.Add(CharacterTypeField);

            /* TEXT NAME */
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
        }


        public override void LoadValueInToField()
        {
            character_Field.SetValueWithoutNotify(character);
            CharacterPositionField.SetValueWithoutNotify(characterPosition);
            CharacterTypeField.SetValueWithoutNotify(characterType);
            duration_Field.SetValueWithoutNotify(durationShow);
        }

        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port input = inputContainer.Query<Port>().First();
            if (!input.connected) warning.Add("Node cannot be called");

            Port output = outputContainer.Query<Port>().First();
            if (!output.connected) error.Add("Output does not lead to any node");


            if (durationShow < 1) warning.Add("Short time for Make Decision");

            ErrorList = error;
            WarningList = warning;

        }

        
    }
}
