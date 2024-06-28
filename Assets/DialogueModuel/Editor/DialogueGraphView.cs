using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using MeetAndTalk.Nodes;

namespace MeetAndTalk.Editor
{
    [ExecuteInEditMode] // �� Ŭ������ ������ ��忡���� ����ǵ��� �մϴ�.
    public class DialogueGraphView : GraphView
    {
        private DialogueEditorWindow editorWindow; // ��ȭ ������ â�� �����մϴ�.
        private NodeSearchWindow searchWindow; // ��� �˻� â�� �����մϴ�.

        // ������: GraphView�� �ʱ� ������ �����մϴ�.
        public DialogueGraphView(DialogueEditorWindow _editorWindow)
        {
            editorWindow = _editorWindow;

            // ��ũ �׸� ��Ÿ�Ͻ�Ʈ�� �ε��ϰ� �߰��մϴ�.
            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>("Themes/DarkTheme");
            styleSheets.Add(tmpStyleSheet);

            // �� ������ �մϴ�.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // �׷����� �پ��� ���۱⸦ �߰��մϴ�.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            // �׸��� ����� �߰��ϰ� �θ� ũ�⿡ ����ϴ�.
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // �˻� â�� �߰��մϴ�.
            AddSearchWindow();
        }

        // ��ȭ �׷����� ��ȿ���� �˻��մϴ�.
        public void ValidateDialogue()
        {
            List<BaseNode> bases = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in bases) { node.Validate(); }
        }

        // �׸��� ������Ʈ�մϴ�.
        public void UpdateTheme(string name)
        {
            styleSheets.Remove(styleSheets[styleSheets.count - 1]);
            styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{name}Theme"));

            List<BaseNode> bases = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in bases) { node.UpdateTheme(name); }
        }

        // ��� �˻� â�� �߰��մϴ�.
        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        // ���� ��Ʈ�� ȣȯ�Ǵ� ��Ʈ�� ��ȯ�մϴ�.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            Port startPortView = startPort;

            ports.ForEach((port) => {
                Port portView = port;

                // ���� ��Ʈ�� ���� ��忡 ���� �ʰ� ������ �ٸ� ��Ʈ�� ã���ϴ�.
                if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction) {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        // �� �ٽ� �ε��մϴ�.
        public void LanguageReload()
        {
            List<DialogueChoiceNode> dialogueChoiceNodes = nodes.ToList().Where(node => node is DialogueChoiceNode).Cast<DialogueChoiceNode>().ToList();
            List<DialogueNode> dialogueNodes = nodes.ToList().Where(node => node is DialogueNode).Cast<DialogueNode>().ToList();

            foreach (DialogueChoiceNode dialogueNode in dialogueChoiceNodes) {
                dialogueNode.ReloadLanguage();
            }
            foreach (DialogueNode dialogueNode in dialogueNodes) {
                dialogueNode.ReloadLanguage();
            }
        }

        // ���� ��带 �����մϴ�.
        public StartNode CreateStartNode(Vector2 _pos)
        {
            StartNode tmp = new StartNode(_pos, editorWindow, this);
            tmp.name = "Start";

            return tmp;
        }

        // ���� ��带 �����մϴ�.
        public EndNode CreateEndNode(Vector2 _pos)
        {
            EndNode tmp = new EndNode(_pos, editorWindow, this);
            tmp.name = "End";

            return tmp;
        }

        // ��ȭ ���� ��带 �����մϴ�.
        public DialogueChoiceNode CreateDialogueChoiceNode(Vector2 _pos)
        {
            DialogueChoiceNode tmp = new DialogueChoiceNode(_pos, editorWindow, this);
            tmp.name = "Choice";
            tmp.ReloadLanguage();

            return tmp;
        }

        // ��ȭ ��带 �����մϴ�.
        public DialogueNode CreateDialogueNode(Vector2 _pos)
        {
            DialogueNode tmp = new DialogueNode(_pos, editorWindow, this);
            tmp.name = "Dialog";
            tmp.ReloadLanguage();

            return tmp;
        }
    }
}
