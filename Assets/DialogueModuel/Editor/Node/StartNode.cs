using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using DialogueSystem.Editor;

namespace DialogueSystem.Nodes
{
    public class StartNode : BaseNode
    {


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


            AddOutputPort("Output", Port.Capacity.Single);
            RefreshExpandedState();
            RefreshPorts();
            AddValidationContainer();
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
