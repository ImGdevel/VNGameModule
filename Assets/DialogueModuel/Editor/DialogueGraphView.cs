using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using MeetAndTalk.Nodes;

namespace MeetAndTalk.Editor
{
    [ExecuteInEditMode] // 이 클래스가 에디터 모드에서도 실행되도록 합니다.
    public class DialogueGraphView : GraphView
    {
        private DialogueEditorWindow editorWindow; // 대화 편집기 창을 참조합니다.
        private NodeSearchWindow searchWindow; // 노드 검색 창을 참조합니다.

        // 생성자: GraphView의 초기 설정을 수행합니다.
        public DialogueGraphView(DialogueEditorWindow _editorWindow)
        {
            editorWindow = _editorWindow;

            // 다크 테마 스타일시트를 로드하고 추가합니다.
            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>("Themes/DarkTheme");
            styleSheets.Add(tmpStyleSheet);

            // 줌 설정을 합니다.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // 그래프에 다양한 조작기를 추가합니다.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            // 그리드 배경을 추가하고 부모 크기에 맞춥니다.
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // 검색 창을 추가합니다.
            AddSearchWindow();
        }

        // 대화 그래프의 유효성을 검사합니다.
        public void ValidateDialogue()
        {
            List<BaseNode> bases = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in bases) { node.Validate(); }
        }

        // 테마를 업데이트합니다.
        public void UpdateTheme(string name)
        {
            styleSheets.Remove(styleSheets[styleSheets.count - 1]);
            styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{name}Theme"));

            List<BaseNode> bases = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in bases) { node.UpdateTheme(name); }
        }

        // 노드 검색 창을 추가합니다.
        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        // 시작 포트와 호환되는 포트를 반환합니다.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            Port startPortView = startPort;

            ports.ForEach((port) => {
                Port portView = port;

                // 시작 포트가 동일 노드에 있지 않고 방향이 다른 포트를 찾습니다.
                if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction) {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        // 언어를 다시 로드합니다.
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

        // 시작 노드를 생성합니다.
        public StartNode CreateStartNode(Vector2 _pos)
        {
            StartNode tmp = new StartNode(_pos, editorWindow, this);
            tmp.name = "Start";

            return tmp;
        }

        // 종료 노드를 생성합니다.
        public EndNode CreateEndNode(Vector2 _pos)
        {
            EndNode tmp = new EndNode(_pos, editorWindow, this);
            tmp.name = "End";

            return tmp;
        }

        // 대화 선택 노드를 생성합니다.
        public DialogueChoiceNode CreateDialogueChoiceNode(Vector2 _pos)
        {
            DialogueChoiceNode tmp = new DialogueChoiceNode(_pos, editorWindow, this);
            tmp.name = "Choice";
            tmp.ReloadLanguage();

            return tmp;
        }

        // 대화 노드를 생성합니다.
        public DialogueNode CreateDialogueNode(Vector2 _pos)
        {
            DialogueNode tmp = new DialogueNode(_pos, editorWindow, this);
            tmp.name = "Dialog";
            tmp.ReloadLanguage();

            return tmp;
        }
    }
}
