using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using MeetAndTalk.Editor;
using MeetAndTalk.Settings;

namespace MeetAndTalk.Nodes
{
    // �⺻ ��� Ŭ����
    public class BaseNode : Node
    {
        public string nodeGuid; // ��� ���� �ĺ���
        protected DialogueGraphView graphView; // ��ȭ �׷��� �� ����
        protected DialogueEditorWindow editorWindow; // ��ȭ ������ â ����
        protected Vector2 defualtNodeSize = new Vector2(200, 250); // �⺻ ��� ũ��

        public List<string> ErrorList = new List<string>(); // ���� ���
        public List<string> WarningList = new List<string>(); // ��� ���

        protected string NodeGrid { get => NodeGrid; set => nodeGuid = value; } // ��� �׸��� (���� �ڵ忡���� �߸��� ����)

        // �⺻ ������
        public BaseNode()
        {
        }

        // ��� ��Ʈ�� �߰��ϴ� �޼���
        public void AddOutputPort(string name, Port.Capacity capality = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capality);
            outputPort.portName = name;
            outputContainer.Add(outputPort);
        }

        // �Է� ��Ʈ�� �߰��ϴ� �޼���
        public void AddInputPort(string name, Port.Capacity capality = Port.Capacity.Single)
        {
            Port inputPort = GetPortInstance(Direction.Input, capality);
            inputPort.portName = name;
            inputContainer.Add(inputPort);
        }

        // ��Ʈ �ν��Ͻ��� �����ϴ� �޼���
        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        // �ʵ忡 ���� �ε��ϴ� ���� �޼��� (��ӹ��� Ŭ�������� ����)
        public virtual void LoadValueInToField(){}

        // �׸��� ������Ʈ�ϴ� �޼���
        public void UpdateTheme(string name)
        {
            if (styleSheets[styleSheets.count - 1].name != "Node")
                styleSheets.Remove(styleSheets[styleSheets.count - 1]);
            styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{name}Theme"));
        }

        // ��ȿ�� �˻� �����̳ʸ� �߰��ϴ� �޼���
        public void AddValidationContainer()
        {
            // �����̳� ����
            VisualElement container = new VisualElement();
            container.name = "ValidationContainer";

            // ���� �����̳� ����
            HelpBox ErrorContainer = new HelpBox("Empty Error", HelpBoxMessageType.Error);
            ErrorContainer.name = "ErrorContainer";
            ErrorContainer.style.display = DisplayStyle.None;
            container.Add(ErrorContainer);

            // ��� �����̳� ����
            HelpBox WarningContainer = new HelpBox("Empty Warning", HelpBoxMessageType.Warning);
            WarningContainer.name = "WarningContainer";
            WarningContainer.style.display = DisplayStyle.None;
            container.Add(WarningContainer);

            // �����̳� �߰�
            titleContainer.Add(container);
            mainContainer.style.overflow = Overflow.Visible;
        }

        // ��带 ��ȿ�� �˻��ϴ� �޼���
        public void Validate()
        {
            SetValidation();
            var settings = Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings");
            if (!settings.ShowErrors) ErrorList.Clear();
            if (!settings.ShowWarnings) WarningList.Clear();

            // ���� �ڽ� ������Ʈ
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

            // ��� �ڽ� ������Ʈ
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

        // ��ȿ�� �˻縦 �����ϴ� ���� �޼��� (��ӹ��� Ŭ�������� ����)
        public virtual void SetValidation() { }
    }
}
