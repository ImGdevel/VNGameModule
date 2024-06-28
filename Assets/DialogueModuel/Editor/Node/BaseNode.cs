using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using MeetAndTalk.Editor;
using MeetAndTalk.Settings;

namespace MeetAndTalk.Nodes
{
    // 기본 노드 클래스
    public class BaseNode : Node
    {
        public string nodeGuid; // 노드 고유 식별자
        protected DialogueGraphView graphView; // 대화 그래프 뷰 참조
        protected DialogueEditorWindow editorWindow; // 대화 에디터 창 참조
        protected Vector2 defualtNodeSize = new Vector2(200, 250); // 기본 노드 크기

        public List<string> ErrorList = new List<string>(); // 오류 목록
        public List<string> WarningList = new List<string>(); // 경고 목록

        protected string NodeGrid { get => NodeGrid; set => nodeGuid = value; } // 노드 그리드 (현재 코드에서는 잘못된 구현)

        // 기본 생성자
        public BaseNode()
        {
        }

        // 출력 포트를 추가하는 메서드
        public void AddOutputPort(string name, Port.Capacity capality = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capality);
            outputPort.portName = name;
            outputContainer.Add(outputPort);
        }

        // 입력 포트를 추가하는 메서드
        public void AddInputPort(string name, Port.Capacity capality = Port.Capacity.Single)
        {
            Port inputPort = GetPortInstance(Direction.Input, capality);
            inputPort.portName = name;
            inputContainer.Add(inputPort);
        }

        // 포트 인스턴스를 생성하는 메서드
        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        // 필드에 값을 로드하는 가상 메서드 (상속받은 클래스에서 구현)
        public virtual void LoadValueInToField(){}

        // 테마를 업데이트하는 메서드
        public void UpdateTheme(string name)
        {
            if (styleSheets[styleSheets.count - 1].name != "Node")
                styleSheets.Remove(styleSheets[styleSheets.count - 1]);
            styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{name}Theme"));
        }

        // 유효성 검사 컨테이너를 추가하는 메서드
        public void AddValidationContainer()
        {
            // 컨테이너 생성
            VisualElement container = new VisualElement();
            container.name = "ValidationContainer";

            // 오류 컨테이너 생성
            HelpBox ErrorContainer = new HelpBox("Empty Error", HelpBoxMessageType.Error);
            ErrorContainer.name = "ErrorContainer";
            ErrorContainer.style.display = DisplayStyle.None;
            container.Add(ErrorContainer);

            // 경고 컨테이너 생성
            HelpBox WarningContainer = new HelpBox("Empty Warning", HelpBoxMessageType.Warning);
            WarningContainer.name = "WarningContainer";
            WarningContainer.style.display = DisplayStyle.None;
            container.Add(WarningContainer);

            // 컨테이너 추가
            titleContainer.Add(container);
            mainContainer.style.overflow = Overflow.Visible;
        }

        // 노드를 유효성 검사하는 메서드
        public void Validate()
        {
            SetValidation();
            var settings = Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings");
            if (!settings.ShowErrors) ErrorList.Clear();
            if (!settings.ShowWarnings) WarningList.Clear();

            // 오류 박스 업데이트
            HelpBox errorBox = titleContainer.Query<HelpBox>("ErrorContainer").First();
            if (errorBox != null) {
                if (ErrorList.Count < 1) {
                    errorBox.style.display = DisplayStyle.None;
                }
                else {
                    errorBox.style.display = DisplayStyle.Flex;
                    string tmp = $"- ";
                    if (ErrorList.Count == 1) tmp = "";
                    for (int i = 0; i < ErrorList.Count; i++) {
                        tmp += ErrorList[i];
                        if (i != ErrorList.Count - 1) { tmp += "\n- "; }
                    }
                    errorBox.text = tmp;
                }
            }

            // 경고 박스 업데이트
            HelpBox warningBox = titleContainer.Query<HelpBox>("WarningContainer").First();
            if (warningBox != null) {
                if (WarningList.Count < 1) {
                    warningBox.style.display = DisplayStyle.None;
                }
                else {
                    warningBox.style.display = DisplayStyle.Flex;
                    string tmp = $"- ";
                    if (WarningList.Count == 1) tmp = "";
                    for (int i = 0; i < WarningList.Count; i++) {
                        tmp += WarningList[i];
                        if (i != WarningList.Count - 1) { tmp += "\n- "; }
                    }
                    warningBox.text = tmp;
                }
            }
        }

        // 유효성 검사를 설정하는 가상 메서드 (상속받은 클래스에서 구현)
        public virtual void SetValidation() { }
    }
}
