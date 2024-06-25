using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace VisualNovelGame
{
    // Node 클래스를 추상 클래스로 정의
    public abstract class Node : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;

        protected Node()
        {
            // 노드 스타일 및 기본 설정
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

        // Input 포트 생성
        protected Port CreateInputPort(Port.Capacity capacity = Port.Capacity.Multi)
        {
            var inputPort = GeneratePort(Direction.Input, capacity);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            return inputPort;
        }

        // Node 클래스에서 Output 포트 생성 메서드 수정
        protected Port CreateOutputPort(Port.Capacity capacity = Port.Capacity.Single)
        {
            var outputPort = GeneratePort(Direction.Output, capacity);
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
            return outputPort;
        }

        // EdgeConnectorListener 클래스
        protected class EdgeConnectorListener : IEdgeConnectorListener
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
    }

    // StartNode 클래스
    public class StartNode : Node
    {
        public bool EntryPoint = false;

        public StartNode()
        {
            title = "Start";
            EntryPoint = true;

            // Output 포트 생성
            CreateOutputPort();

            capabilities &= ~Capabilities.Movable; // 노드 이동 불가 설정
            capabilities &= ~Capabilities.Deletable; // 노드 삭제 불가 설정

            RefreshExpandedState();
            RefreshPorts();
        }
    }

    // DialogueNode 클래스에서 Output 포트 생성 부분 수정
    public class DialogueNode : Node
    {
        public string dialogueText;

        public DialogueNode()
        {
            var contentContainer = new Foldout();
            contentContainer.text = "Content 영역";

            var dialogueField = new TextField("대화 내용");
            dialogueField.multiline = true;
            dialogueField.RegisterValueChangedCallback(evt => dialogueText = evt.newValue);
            contentContainer.Add(dialogueField);

            mainContainer.Add(contentContainer);

            // Input 포트는 Multi 용량으로, Output 포트는 Single 용량으로 생성
            CreateInputPort();
            CreateOutputPort();
        }

    }




    // ChoiceNode 클래스
    public class ChoiceNode : Node
    {
        public string ChoiceText;

        public ChoiceNode()
        {
            var contentContainer = new Foldout();
            contentContainer.text = "Content 영역";

            var choiceField = new TextField("선택지");
            choiceField.RegisterValueChangedCallback(evt => ChoiceText = evt.newValue);
            contentContainer.Add(choiceField);

            mainContainer.Add(contentContainer);

            // Input 및 Output 포트 생성
            CreateInputPort();
            CreateOutputPort();
        }
    }

}
