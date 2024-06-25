using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace VisualNovelGame
{
    // Node Ŭ������ �߻� Ŭ������ ����
    public abstract class Node : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;

        protected Node()
        {
            // ��� ��Ÿ�� �� �⺻ ����
            //styleSheets.Add(Resources.Load<StyleSheet>("NodeStyle"));
            RefreshExpandedState();
            RefreshPorts();
        }

        protected Port GeneratePort(Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
            port.portName = portDirection == Direction.Input ? "Input" : "Next";
            port.AddManipulator(new EdgeConnector<Edge>(new EdgeConnectorListener()));
            return port;
        }

        // Input ��Ʈ ����
        protected Port CreateInputPort(Port.Capacity capacity = Port.Capacity.Multi)
        {
            var inputPort = GeneratePort(Direction.Input, capacity);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            return inputPort;
        }

        // Node Ŭ�������� Output ��Ʈ ���� �޼��� ����
        protected Port CreateOutputPort(Port.Capacity capacity = Port.Capacity.Single)
        {
            var outputPort = GeneratePort(Direction.Output, capacity);
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
            return outputPort;
        }

        // EdgeConnectorListener Ŭ����
        protected class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                // ��Ʈ �ܺο� ������� ���� ����
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                var inputPort = edge.input as Port;
                var outputPort = edge.output as Port;

                if (inputPort != null && outputPort != null && inputPort.node != outputPort.node) {
                    if ((outputPort.portName == "Next" && inputPort.portName == "Input")) {
                        graphView.AddElement(edge);
                    }
                }
            }
        }
    }

    // StartNode Ŭ����
    public class StartNode : Node
    {
        public bool EntryPoint = false;

        public StartNode()
        {
            title = "Start";
            EntryPoint = true;

            // Output ��Ʈ ����
            CreateOutputPort();

            capabilities &= ~Capabilities.Movable; // ��� �̵� �Ұ� ����
            capabilities &= ~Capabilities.Deletable; // ��� ���� �Ұ� ����

            RefreshExpandedState();
            RefreshPorts();
        }
    }

    // DialogueNode Ŭ�������� Output ��Ʈ ���� �κ� ����
    public class DialogueNode : Node
    {
        public string dialogueText;

        public DialogueNode()
        {
            var contentContainer = new Foldout();
            contentContainer.text = "Content ����";

            var dialogueField = new TextField("��ȭ ����");
            dialogueField.multiline = true;
            dialogueField.RegisterValueChangedCallback(evt => dialogueText = evt.newValue);
            contentContainer.Add(dialogueField);

            mainContainer.Add(contentContainer);

            // Input ��Ʈ�� Multi �뷮����, Output ��Ʈ�� Single �뷮���� ����
            CreateInputPort();
            CreateOutputPort();
        }

    }




    // ChoiceNode Ŭ����
    public class ChoiceNode : Node
    {
        public string ChoiceText;

        public ChoiceNode()
        {
            var contentContainer = new Foldout();
            contentContainer.text = "Content ����";

            var choiceField = new TextField("������");
            choiceField.RegisterValueChangedCallback(evt => ChoiceText = evt.newValue);
            contentContainer.Add(choiceField);

            mainContainer.Add(contentContainer);

            // Input �� Output ��Ʈ ����
            CreateInputPort();
            CreateOutputPort();
        }
    }

}
