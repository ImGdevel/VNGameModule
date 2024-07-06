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

        private StartNode GenerateEntryPointNode()
        {
            var node = new StartNode {
                title = "Start",
                GUID = Guid.NewGuid().ToString()
            };

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

        private DialogueNode CreateDialogueNodeInstance(string nodeName, string dialogueText = "")
        {
            var node = new DialogueNode();
            node.title = nodeName; // 노드의 제목 설정

            // 대화 내용을 입력하는 TextField에 초기값 설정
            node.dialogueText = dialogueText;

            // 대화 내용을 입력하는 TextField의 초기값 설정
            var textField = node.mainContainer.Q<TextField>();
            if (textField != null)
            {
                textField.SetValueWithoutNotify(dialogueText);
            }

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, new Vector2(300, 400)));

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
            node.SetPosition(new Rect(Vector2.zero, new Vector2(300, 400)));

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
            // edgesToCreate가 null이거나 비어 있는 경우 처리
            if (graphViewChange.edgesToCreate == null || !graphViewChange.edgesToCreate.Any()) {
                return graphViewChange;
            }

            // 리스트를 복사하여 안전하게 순회하도록 합니다.
            var edgesToCreateCopy = graphViewChange.edgesToCreate.ToList();

            foreach (var edge in edgesToCreateCopy) {
                if (edge.input.node != null && edge.output.node != null) {
                    var inputPort = edge.input as Port;
                    var outputPort = edge.output as Port;

                    if (!IsValidConnection(inputPort, outputPort)) {
                        // 유효하지 않은 연결을 제거합니다.
                        graphViewChange.edgesToCreate.Remove(edge);
                    }
                }
            }

            return graphViewChange;
        }


        private bool IsValidConnection(Port startPort, Port otherPort)
        {
            // 포트 방향이 반대인지 확인
            if (startPort.direction == otherPort.direction) {
                return false;
            }

            // 포트 타입이 동일한지 확인
            if (startPort.portType != otherPort.portType) {
                return false;
            }

            // 포트 이름에 따라 연결 유효성 검사
            if (startPort.portName == "Next" && otherPort.portName == "Input") {
                return true;
            }

            return false;
        }

        private class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                // 포트 외부에 드롭했을 때의 동작
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

        }
    }
}