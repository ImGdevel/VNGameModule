using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using DialogueSystem.Editor;

namespace DialogueSystem.Nodes
{
    public class EndNode : BaseNode
    {
        private EndNodeType endNodeType = EndNodeType.End;
        private EnumField enumField;

        public DialogueScript dialogue;
        private Label dialogLabel;
        private ObjectField dialogField;

        public EndNodeType EndNodeType { get => endNodeType; set => endNodeType = value; }

        public EndNode()
        {

        }

        public EndNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "End";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            AddValidationContainer();

            enumField = new EnumField()
            {
                value = endNodeType
            };

            enumField.Init(endNodeType);

            enumField.RegisterValueChangedCallback((value) =>
            {
                endNodeType = (EndNodeType)value.newValue;
            });
            enumField.SetValueWithoutNotify(endNodeType);
            //mainContainer.Add(enumField);

            dialogLabel = new Label("Start Dialogue");
            dialogLabel.AddToClassList("label_name");
            dialogLabel.AddToClassList("Label");
            //mainContainer.Add(dialogLabel);

            dialogField = new ObjectField()
            {
                objectType = typeof(DialogueScript),
                allowSceneObjects = false,
                value = null,
            };
            dialogField.RegisterValueChangedCallback(value =>
            {
                dialogue = value.newValue as DialogueScript;
            });
            dialogField.SetValueWithoutNotify(dialogue);
            dialogField.AddToClassList("ForceTerminateScene");
            //mainContainer.Add(dialogField);

        }

        public override void LoadValueInToField()
        {
            enumField.SetValueWithoutNotify(endNodeType);
            dialogField.SetValueWithoutNotify(dialogue);
        }

        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port input = inputContainer.Query<Port>().First();
            if (!input.connected) warning.Add("Node cannot be called");

            // Update List
            if(EndNodeType == EndNodeType.StartDialogue) 
            {
                dialogLabel.style.display = DisplayStyle.Flex;
                dialogField.style.display = DisplayStyle.Flex;
                if (dialogue == null) error.Add("Dialogue Field is Empty");
            }
            else
            {
                dialogLabel.style.display = DisplayStyle.None;
                dialogField.style.display = DisplayStyle.None;
            }

            ErrorList = error;
            WarningList = warning;
        }
    }
}
