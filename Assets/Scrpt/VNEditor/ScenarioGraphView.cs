using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace VisualNovelGame
{
    public class ScenarioGraphView : GraphView
    {
        public ScenarioGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("ScenarioGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // �׷��� �信 �⺻ ���۱⸦ �߰��մϴ�.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // �׸��� ����� �߰��մϴ�.
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // ���� Ŀ���� �����ʸ� �����մϴ�.
            this.graphViewChanged += OnGraphViewChanged;

            // ���� ��带 �߰��մϴ�.
            AddElement(GenerateEntryPointNode());
        }

        // ���� ��带 �����մϴ�.
        private DialogueNode GenerateEntryPointNode()
        {
            var node = new DialogueNode {
                title = "Start",
                GUID = Guid.NewGuid().ToString(),
                EntryPoint = true
            };

            var port = GeneratePort(node, Direction.Output);
            port.portName = "Next";
            node.outputContainer.Add(port);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(100, 200, 150, 200));

            return node;
        }

        // ��ȭ ��带 �����ϰ� �׷��� �信 �߰��մϴ�.
        public void CreateDialogueNode(string nodeName)
        {
            AddElement(CreateDialogueNodeInstance(nodeName));
        }

        // ���� ��带 �����ϰ� �׷��� �信 �߰��մϴ�.
        public void CreateChoiceNode(string nodeName)
        {
            AddElement(CreateChoiceNodeInstance(nodeName));
        }

        // ��ȭ ��� �ν��Ͻ��� �����մϴ�.
        private DialogueNode CreateDialogueNodeInstance(string nodeName)
        {
            var node = new DialogueNode {
                title = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            var outputPort = GeneratePort(node, Direction.Output);
            outputPort.portName = "Next";
            node.outputContainer.Add(outputPort);

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));

            return node;
        }

        // ���� ��� �ν��Ͻ��� �����մϴ�.
        private ChoiceNode CreateChoiceNodeInstance(string nodeName)
        {
            var node = new ChoiceNode {
                title = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            var button = new Button(() => { AddChoicePort(node); });
            button.text = "New Choice";
            node.titleButtonContainer.Add(button);

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));

            return node;
        }

        // ��Ʈ�� �����մϴ�.
        private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
            port.portName = portDirection == Direction.Input ? "Input" : "Next";
            port.AddManipulator(new EdgeConnector<Edge>(new EdgeConnectorListener()));
            return port;
        }

        // ���� ��忡 ���� ��Ʈ�� �߰��մϴ�.
        private void AddChoicePort(ChoiceNode node)
        {
            var generatedPort = GeneratePort(node, Direction.Output);

            var outputPortCount = node.outputContainer.Query<Port>().ToList().Count;
            generatedPort.portName = $"Choice {outputPortCount + 1}";

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        // �׷��� �� ���� �� ȣ��Ǵ� �޼���
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // ������ ������ ��ȸ�մϴ�.
            if (graphViewChange.edgesToCreate != null) {
                foreach (var edge in graphViewChange.edgesToCreate) {
                    if (edge.input.node != null && edge.output.node != null) {
                        var inputPort = edge.input as Port;
                        var outputPort = edge.output as Port;

                        // ������ ��ȿ���� Ȯ���մϴ�. (��: Next���� Input����)
                        if (!IsValidConnection(inputPort, outputPort)) {
                            // ��ȿ���� ���� ������ �����մϴ�.
                            graphViewChange.edgesToCreate.Remove(edge);
                        }
                    }
                }
            }
            return graphViewChange;
        }

        // ������ ��ȿ���� Ȯ���ϴ� �޼���
        private bool IsValidConnection(Port inputPort, Port outputPort)
        {
            // ���� ��ȿ�� �˻�: Next���� Input���� ���� ���
            return (outputPort.portName == "Next" && inputPort.portName == "Input");
        }

        // EdgeConnectorListener Ŭ���� �߰�
        private class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                // ��Ʈ �ܺο� ������� ���� ���� (�ʿ信 ���� ����)
                Debug.Log("?");

            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                Debug.Log("!@");
                // ������� ���� ����
                var inputPort = edge.input as Port;
                var outputPort = edge.output as Port;

                if (inputPort != null && outputPort != null && inputPort.node != outputPort.node) {
                    // ��ȿ�� �������� Ȯ���մϴ�.
                    if ((outputPort.portName == "Next" && inputPort.portName == "Input")) {
                        graphView.AddElement(edge);
                    }
                }
            }
        }
    }

    // ��ȭ ��� Ŭ����
    public class DialogueNode : Node
    {
        public string GUID;
        public bool EntryPoint = false;
        public string DialogueText;
    }

    // ���� ��� Ŭ����
    public class ChoiceNode : Node
    {
        public string GUID;
        public string ChoiceText;
    }
}
