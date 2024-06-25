using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

namespace VisualNovelGame
{
    public class ScenarioGraphView : GraphView
    {
        public List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        public List<ChoiceNode> choiceNodes = new List<ChoiceNode>();
        private ScenarioManager scenarioManager;

        public ScenarioGraphView(ScenarioManager manager)
        {
            scenarioManager = manager;
            styleSheets.Add(Resources.Load<StyleSheet>("ScenarioGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.graphViewChanged += OnGraphViewChanged;

            AddElement(GenerateEntryPointNode());
        }

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

        public void CreateDialogueNode(string nodeName)
        {
            var node = CreateDialogueNodeInstance(nodeName);
            dialogueNodes.Add(node);
            AddElement(node);
        }

        public void CreateChoiceNode(string nodeName)
        {
            var node = CreateChoiceNodeInstance(nodeName);
            choiceNodes.Add(node);
            AddElement(node);
        }

        private DialogueNode CreateDialogueNodeInstance(string nodeName)
        {
            var node = new DialogueNode {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(), // GUID ���� �� ����
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            var outputPort = GeneratePort(node, Direction.Output);
            outputPort.portName = "Next";
            node.outputContainer.Add(outputPort);

            var contentContainer = new Foldout();
            contentContainer.text = "Content ����";

            var dialogueField = new TextField("��ȭ ����");
            dialogueField.multiline = true;
            dialogueField.RegisterValueChangedCallback(evt => node.DialogueText = evt.newValue);
            contentContainer.Add(dialogueField);

            node.mainContainer.Add(contentContainer);

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));

            return node;
        }

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

        private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
            port.portName = portDirection == Direction.Input ? "Input" : "Next";
            port.AddManipulator(new EdgeConnector<Edge>(new EdgeConnectorListener()));
            return port;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) => {
                if (startPort != port && startPort.node != port.node) {
                    if (IsValidConnection(startPort, port)) {
                        compatiblePorts.Add(port);
                    }
                }
            });

            return compatiblePorts;
        }

        private void AddChoicePort(ChoiceNode node)
        {
            var generatedPort = GeneratePort(node, Direction.Output);

            var outputPortCount = node.outputContainer.Query<Port>().ToList().Count;
            generatedPort.portName = $"Choice {outputPortCount + 1}";

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }



        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // edgesToCreate�� null�̰ų� ��� �ִ� ��� ó��
            if (graphViewChange.edgesToCreate == null || !graphViewChange.edgesToCreate.Any()) {
                return graphViewChange;
            }

            // ����Ʈ�� �����Ͽ� �����ϰ� ��ȸ�ϵ��� �մϴ�.
            var edgesToCreateCopy = graphViewChange.edgesToCreate.ToList();

            foreach (var edge in edgesToCreateCopy) {
                if (edge.input.node != null && edge.output.node != null) {
                    var inputPort = edge.input as Port;
                    var outputPort = edge.output as Port;

                    if (!IsValidConnection(inputPort, outputPort)) {
                        // ��ȿ���� ���� ������ �����մϴ�.
                        graphViewChange.edgesToCreate.Remove(edge);
                    }
                }
            }

            return graphViewChange;
        }


        private bool IsValidConnection(Port startPort, Port otherPort)
        {
            // ��Ʈ ������ �ݴ����� Ȯ��
            if (startPort.direction == otherPort.direction) {
                return false;
            }

            // ��Ʈ Ÿ���� �������� Ȯ��
            if (startPort.portType != otherPort.portType) {
                return false;
            }

            // ��Ʈ �̸��� ���� ���� ��ȿ�� �˻�
            if (startPort.portName == "Next" && otherPort.portName == "Input") {
                return true;
            }

            return false;
        }

        private class EdgeConnectorListener : IEdgeConnectorListener
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



        public void SaveScenario(string path)
        {
            var scenario = new Scenario();

            foreach (var dialogueNode in dialogueNodes) {
                scenario.dialogues.Add(new Dialogue(
                    dialogueNode.GUID,
                    dialogueNode.DialogueText
                ));
            }

            foreach (var choiceNode in choiceNodes) {
                var choice = new Choice(
                    choiceNode.GUID,
                    choiceNode.ChoiceText
                );

                var ports = choiceNode.outputContainer.Query<Port>().ToList();
                foreach (var port in ports) {
                    if (port.connections.Count() > 0) {
                        var connectedEdge = port.connections.First();
                        var nextNode = connectedEdge.input.node as DialogueNode;
                        if (nextNode != null) {
                            choice.nextDialogueGUIDs.Add(nextNode.GUID);
                        }
                    }
                }

                scenario.choices.Add(choice);
            }

            //scenarioManager.SaveScenario(path, scenario);
        }
    }
}

// ��ȭ ��� Ŭ����
public class DialogueNode : Node
{
    public string GUID;
    public bool EntryPoint = false;
    public string DialogueText;
    public string characterName;
    public string text;
    public string emotion;
}


// ���� ��� Ŭ����
public class ChoiceNode : Node
{
    public string GUID;
    public string ChoiceText;
}