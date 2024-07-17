using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using DialogueSystem.Nodes;

namespace DialogueSystem.Editor
{
    [ExecuteInEditMode]
    public class DialogueGraphView : GraphView
    {
        private DialogueEditorWindow editorWindow;
        private NodeSearchWindow searchWindow;

        public DialogueGraphView(DialogueEditorWindow _editorWindow)
        {
            editorWindow = _editorWindow;

            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>("Themes/BaseTheme");
            styleSheets.Add(tmpStyleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow();
        }

        public void ValidateDialogue()
        {
            List<BaseNode> bases = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in bases) { node.Validate(); }
        }

        public void UpdateTheme(string name)
        {
            styleSheets.Remove(styleSheets[styleSheets.count - 1]);
            styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{name}Theme"));

            List<BaseNode> bases = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in bases) { node.UpdateTheme(name); }

        }

        private void AddMiniMap()
        {
            MiniMap minimap = new MiniMap()
            {
                anchored = true,
                elementTypeColor = Color.green,
                name = "minimap",
                maxHeight = 100,
                maxWidth = 200
            };
            minimap.SetPosition(new Rect(0, 30, 200, 100));
            Add(minimap);
        }

        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compactiblePorts = new List<Port>();
            Port startPortView = startPort;

            ports.ForEach((port) =>
            {
                Port portView = port;

                if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction)
                {
                    compactiblePorts.Add(port);
                }
            });

            return compactiblePorts;
        }

        public void LanguageReload()
        {
            List<DialogueChoiceNode> dialogueChoiceNodes = nodes.ToList().Where(node => node is DialogueChoiceNode).Cast<DialogueChoiceNode>().ToList();
            List<TimerChoiceNode> timerChoiceNodes = nodes.ToList().Where(node => node is TimerChoiceNode).Cast<TimerChoiceNode>().ToList();
            List<DialogueNode> dialogueNodes = nodes.ToList().Where(node => node is DialogueNode).Cast<DialogueNode>().ToList();

            foreach (DialogueChoiceNode dialogueNode in dialogueChoiceNodes)
            {
                dialogueNode.ReloadLanguage();
            }
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                dialogueNode.ReloadLanguage();
            }
            foreach (TimerChoiceNode dialogueNode in timerChoiceNodes)
            {
                dialogueNode.ReloadLanguage();
            }
        }

        public StartNode CreateStartNode(Vector2 _pos)
        {
            StartNode tmp = new StartNode(_pos, editorWindow, this);
            tmp.name = "Start";

            return tmp;
        }

        public EndNode CreateEndNode(Vector2 _pos)
        {
            EndNode tmp = new EndNode(_pos, editorWindow, this);
            tmp.name = "End";

            return tmp;
        }

        public EventNode CreateEventNode(Vector2 _pos)
        {
            EventNode tmp = new EventNode(_pos, editorWindow, this);
            tmp.name = "Event";

            return tmp;
        }
        public DialogueChoiceNode CreateDialogueChoiceNode(Vector2 _pos)
        {
            DialogueChoiceNode tmp = new DialogueChoiceNode(_pos, editorWindow, this);
            tmp.name = "Choice";
            tmp.ReloadLanguage();

            return tmp;
        }
        public TimerChoiceNode CreateTimerChoiceNode(Vector2 _pos)
        {
            TimerChoiceNode tmp = new TimerChoiceNode(_pos, editorWindow, this);
            tmp.name = "TimerChoice";
            tmp.ReloadLanguage();

            return tmp;
        }

        public DialogueNode CreateDialogueNode(Vector2 _pos)
        {
            DialogueNode tmp = new DialogueNode(_pos, editorWindow, this);
            tmp.name = "Dialog";
            tmp.ReloadLanguage();

            return tmp;
        }

        public RandomNote CreateRandomNode(Vector2 _pos)
        {
            RandomNote tmp = new RandomNote(_pos, editorWindow, this);
            tmp.name = "Random";
            tmp.ReloadLanguage();

            return tmp;
        }

        public IFNode CreateIFNode(Vector2 _pos)
        {
            IFNode tmp = new IFNode(_pos, editorWindow, this);
            tmp.name = "IF";

            return tmp;
        }
    }
}
