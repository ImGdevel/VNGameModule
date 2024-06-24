using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace VisualNovelGame
{
    public class ScenarioGraphView : GraphView
    {
        public ScenarioGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("ScenarioGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            CreateEdgeConnectorListener(); // 연결 리스너 생성

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
            AddElement(CreateDialogueNodeInstance(nodeName));
        }

        public void CreateChoiceNode(string nodeName)
        {
            AddElement(CreateChoiceNodeInstance(nodeName));
        }

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
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
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

        private void CreateEdgeConnectorListener()
        {
            var listener = new EdgeConnectorListener();
            this.graphViewChanged += listener.OnGraphViewChanged;
            this.graphViewChanged += listener.OnEdgeCreationRequested; // 연결 요청 리스너 추가
        }

        private class EdgeConnectorListener : IGraphViewChange
        {
            public void OnGraphViewChanged(GraphViewChange graphViewChange)
            {
                // Implement graph view changes if needed
            }

            public void OnEdgeCreationRequested(GraphViewChange graphViewChange)
            {
                // Iterate through the edges to be created
                foreach (var edge in graphViewChange.edgesToCreate) {
                    // Ensure that only compatible ports can be connected
                    if (edge.input.node != null && edge.output.node != null) {
                        var inputPort = edge.input as Port;
                        var outputPort = edge.output as Port;

                        // Check if the connection is valid (example: Next to Input)
                        if (IsValidConnection(inputPort, outputPort)) {
                            // Create the edge
                            edge.input.Connect(edge.output);
                            edge.input.edgeConnector.edgeControl.edgeDragHelper.OnDrop(new Edge());

                            // Notify graph view change
                            graphViewChange.edgesToCreate.Remove(edge);
                            graphViewChange.edgesToCreate.Add(edge);
                        }
                    }
                }
            }

            private bool IsValidConnection(Port inputPort, Port outputPort)
            {
                // Example validation: allow connection from Next to Input
                if (outputPort.portName == "Next" && inputPort.portName == "Input") {
                    return true;
                }
                return false;
            }
        }
    }

    public class DialogueNode : Node
    {
        public string GUID;
        public bool EntryPoint = false;
        public string DialogueText;
    }

    public class ChoiceNode : Node
    {
        public string GUID;
        public string ChoiceText;
    }
}
