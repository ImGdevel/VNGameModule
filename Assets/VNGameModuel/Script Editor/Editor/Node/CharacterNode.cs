using System;
using DialogueSystem.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace DialogueSystem.Nodes
{
    public class CharacterNode : BaseNode
    {
        private DialogueCharacter character = null;
        private CharacterExpression characterExpression = CharacterExpression.Normal;
        private Vector3 characterSpritePos = Vector3.zero;
        private float characterSpriteSize = 1f;
        private CharacterEffectType characterEffect = CharacterEffectType.None;
        private float effectWeight = 0f;
        private float durationShow = 1f;

        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        public DialogueCharacter Character { get => character; set => character = value; }
        public CharacterExpression CharacterExpression { get => characterExpression; set => characterExpression = value; }
        public Vector3 CharacterSpritePos { get => characterSpritePos; set => characterSpritePos = value; }
        public float CharacterSpriteSize { get => characterSpriteSize; set => characterSpriteSize = value; }
        public CharacterEffectType CharacterEffect { get => characterEffect; set => characterEffect = value; }
        public float EffectWeight { get => effectWeight; set => effectWeight = value; }
        public float DurationShow { get => durationShow; set => durationShow = value; }

        
        private ObjectField character_Field;
        private EnumField expression_Field;
        private Vector2Field spritePos_Field;
        private FloatField spriteSize_Field;
        private EnumField effect_Field;
        private FloatField effectWeight_Field;
        private FloatField duration_Field;

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

            Label label_character = new Label("Character SO");
            label_character.AddToClassList("label_name");
            label_character.AddToClassList("Label");
            mainContainer.Add(label_character);
            character_Field = new ObjectField() {
                objectType = typeof(DialogueCharacter),
                allowSceneObjects = false,
            };
            character_Field.RegisterValueChangedCallback(value => {
                character = value.newValue as DialogueCharacter;
            });
            character_Field.SetValueWithoutNotify(character);
            mainContainer.Add(character_Field);

            expression_Field = new EnumField("Expression", characterExpression);
            expression_Field.RegisterValueChangedCallback(value => {
                characterExpression = (CharacterExpression)value.newValue;
            });
            expression_Field.SetValueWithoutNotify(characterExpression);
            mainContainer.Add(expression_Field);

            effect_Field = new EnumField("Character Effect", characterEffect);
            effect_Field.RegisterValueChangedCallback(value => {
                characterEffect = (CharacterEffectType)value.newValue;
                ToggleFieldsByEffect(characterEffect);
            });
            effect_Field.SetValueWithoutNotify(characterEffect);
            mainContainer.Add(effect_Field);

            
            spritePos_Field = new Vector2Field("Position");
            spritePos_Field.SetValueWithoutNotify(characterSpritePos);
            spritePos_Field.AddToClassList("field-margin");
            mainContainer.Add(spritePos_Field);

            spriteSize_Field = new FloatField("Size");
            spriteSize_Field.SetValueWithoutNotify(characterSpriteSize);
            spriteSize_Field.AddToClassList("field-margin"); 
            mainContainer.Add(spriteSize_Field);

            
            effectWeight_Field = new FloatField("Effect Weight");
            effectWeight_Field.SetValueWithoutNotify(effectWeight);
            mainContainer.Add(effectWeight_Field);

            // Display Time
            Label label_duration = new Label("Display Time");
            mainContainer.Add(label_duration);

            duration_Field = new FloatField("");
            duration_Field.RegisterValueChangedCallback(value => {
                durationShow = value.newValue;
            });
            duration_Field.SetValueWithoutNotify(durationShow);
            mainContainer.Add(duration_Field);

            ToggleFieldsByEffect(characterEffect);
        }

        private void ToggleFieldsByEffect(CharacterEffectType effect)
        {
            bool showTransformFields = effect == CharacterEffectType.FadeIn || effect == CharacterEffectType.FadeOut || effect == CharacterEffectType.Translate;
            bool showWeightField = effect == CharacterEffectType.Shake || effect == CharacterEffectType.Pomping;

            spritePos_Field.style.display = showTransformFields ? DisplayStyle.Flex : DisplayStyle.None;
            spriteSize_Field.style.display = showTransformFields ? DisplayStyle.Flex : DisplayStyle.None;
            effectWeight_Field.style.display = showWeightField ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public override void LoadValueInToField()
        {
            character_Field.SetValueWithoutNotify(character);
            expression_Field.SetValueWithoutNotify(characterExpression);
            spritePos_Field.SetValueWithoutNotify(characterSpritePos);
            spriteSize_Field.SetValueWithoutNotify(characterSpriteSize);
            effect_Field.SetValueWithoutNotify(characterEffect);
            effectWeight_Field.SetValueWithoutNotify(effectWeight);
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