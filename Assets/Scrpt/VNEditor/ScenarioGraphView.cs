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

            // 그래프 뷰에 기본 조작기를 추가합니다.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // 그리드 배경을 추가합니다.
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // 엣지 커넥터 리스너를 생성합니다.
            this.graphViewChanged += OnGraphViewChanged;

            // 시작 노드를 추가합니다.
            AddElement(GenerateEntryPointNode());
        }

        // 시작 노드를 생성합니다.
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

        // 대화 노드를 생성하고 그래프 뷰에 추가합니다.
        public void CreateDialogueNode(string nodeName)
        {
            AddElement(CreateDialogueNodeInstance(nodeName));
        }

        // 선택 노드를 생성하고 그래프 뷰에 추가합니다.
        public void CreateChoiceNode(string nodeName)
        {
            AddElement(CreateChoiceNodeInstance(nodeName));
        }

        // 대화 노드 인스턴스를 생성합니다.
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

        // 선택 노드 인스턴스를 생성합니다.
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

        // 포트를 생성합니다.
        private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
            port.portName = portDirection == Direction.Input ? "Input" : "Next";
            port.AddManipulator(new EdgeConnector<Edge>(new EdgeConnectorListener()));
            return port;
        }

        // 선택 노드에 선택 포트를 추가합니다.
        private void AddChoicePort(ChoiceNode node)
        {
            var generatedPort = GeneratePort(node, Direction.Output);

            var outputPortCount = node.outputContainer.Query<Port>().ToList().Count;
            generatedPort.portName = $"Choice {outputPortCount + 1}";

            node.outputContainer.Add(generatedPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        // 그래프 뷰 변경 시 호출되는 메서드
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 생성된 엣지를 순회합니다.
            if (graphViewChange.edgesToCreate != null) {
                foreach (var edge in graphViewChange.edgesToCreate) {
                    if (edge.input.node != null && edge.output.node != null) {
                        var inputPort = edge.input as Port;
                        var outputPort = edge.output as Port;

                        // 연결이 유효한지 확인합니다. (예: Next에서 Input으로)
                        if (!IsValidConnection(inputPort, outputPort)) {
                            // 유효하지 않은 엣지를 제거합니다.
                            graphViewChange.edgesToCreate.Remove(edge);
                        }
                    }
                }
            }
            return graphViewChange;
        }

        // 연결이 유효한지 확인하는 메서드
        private bool IsValidConnection(Port inputPort, Port outputPort)
        {
            // 예시 유효성 검사: Next에서 Input으로 연결 허용
            return (outputPort.portName == "Next" && inputPort.portName == "Input");
        }

        // EdgeConnectorListener 클래스 추가
        private class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                // 포트 외부에 드롭했을 때의 동작 (필요에 따라 구현)
                Debug.Log("?");

            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                Debug.Log("!@");
                // 드롭했을 때의 동작
                var inputPort = edge.input as Port;
                var outputPort = edge.output as Port;

                if (inputPort != null && outputPort != null && inputPort.node != outputPort.node) {
                    // 유효한 연결인지 확인합니다.
                    if ((outputPort.portName == "Next" && inputPort.portName == "Input")) {
                        graphView.AddElement(edge);
                    }
                }
            }
        }
    }

    // 대화 노드 클래스
    public class DialogueNode : Node
    {
        public string GUID;
        public bool EntryPoint = false;
        public string DialogueText;
    }

    // 선택 노드 클래스
    public class ChoiceNode : Node
    {
        public string GUID;
        public string ChoiceText;
    }
}
