using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using MeetAndTalk.Editor;

namespace MeetAndTalk.Nodes
{
    public class EndNode : BaseNode
    {
        private EndNodeType endNodeType = EndNodeType.End;
        private EnumField enumField;

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

            mainContainer.Add(enumField);
            AddValidationContainer();
        }

        public override void LoadValueInToField()
        {
            enumField.SetValueWithoutNotify(endNodeType);
        }

        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port input = inputContainer.Query<Port>().First();
            if (!input.connected) warning.Add("Node cannot be called");

            ErrorList = error;
            WarningList = warning;
        }
    }
}
