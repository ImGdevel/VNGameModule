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
    [System.Serializable]
    public class StartNode : BaseNode
    {
        public string startID;
        private TextField idField;

        public StartNode()
        {

        }

        public StartNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Start";

            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddValidationContainer();

            idField = new TextField("ID:");
            idField.RegisterValueChangedCallback(value =>
            {
                startID = value.newValue;
            });
            idField.SetValueWithoutNotify(startID);
            //mainContainer.Add(idField);

            AddOutputPort("Output", Port.Capacity.Single);
            RefreshExpandedState();
            RefreshPorts();
        }

        public override void LoadValueInToField()
        {
            idField.SetValueWithoutNotify(startID);
        }

        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port port = outputContainer.Query<Port>().First();
            if (!port.connected) error.Add("Output does not lead to any node");

            ErrorList = error;
            WarningList = warning;
        }
    }
}
