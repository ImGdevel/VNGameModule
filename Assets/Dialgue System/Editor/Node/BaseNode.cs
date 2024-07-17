using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using DialogueSystem.Editor;
using DialogueSystem.Settings;

namespace DialogueSystem.Nodes
{
    public class BaseNode : Node
    {
        public string nodeGuid;
        protected DialogueGraphView graphView;
        protected DialogueEditorWindow editorWindow;
        protected Vector2 defualtNodeSize = new Vector2(200, 250);

        public List<string> ErrorList = new List<string>();
        public List<string> WarningList = new List<string>();

        protected string NodeGrid { get => NodeGrid; set => nodeGuid = value; }

        public BaseNode()
        {

        }

        public void AddOutputPort(string name, Port.Capacity capality = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capality);
            outputPort.portName = name;
            outputContainer.Add(outputPort);
        }

        public void AddOutputPort(string name, string className, Port.Capacity capality = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capality);
            outputPort.portName = name;
            outputPort.AddToClassList(className);

            outputContainer.Add(outputPort);
        }

        public void AddInputPort(string name, Port.Capacity capality = Port.Capacity.Single)
        {
            Port inputPort = GetPortInstance(Direction.Input, capality);
            inputPort.portName = name;
            inputContainer.Add(inputPort);
        }

        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        public virtual void LoadValueInToField()
        {

        }

        public void UpdateTheme(string name)
        {
            if(styleSheets[styleSheets.count - 1].name != "Node") styleSheets.Remove(styleSheets[styleSheets.count - 1]);
            styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{name}Theme"));
        }

        public void AddValidationContainer()
        {
            // Container
            VisualElement container = new VisualElement();
            container.name = "ValidationContainer";
            
            HelpBox ErrorContainer = (new HelpBox("Empty Error", HelpBoxMessageType.Error));
            ErrorContainer.name = "ErrorContainer";
            ErrorContainer.style.display = DisplayStyle.None;
            container.Add(ErrorContainer);

            HelpBox WarningContainer = (new HelpBox("Empty Warning", HelpBoxMessageType.Warning));
            WarningContainer.name = "WarningContainer";
            WarningContainer.style.display = DisplayStyle.None;
            container.Add(WarningContainer);

            titleContainer.Add(container);
            mainContainer.style.overflow = Overflow.Visible;
        }

        public void Validate()
        {
            SetValidation();
            if (!Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").ShowErrors) ErrorList.Clear();
            if (!Resources.Load<DialogueEditorSettings>("DialogueEditorSettings").ShowWarnings) WarningList.Clear();

            HelpBox errorBox = titleContainer.Query<HelpBox>("ErrorContainer").First();
            if (errorBox != null)
            {
                if(ErrorList.Count < 1) { errorBox.style.display = DisplayStyle.None; } 
                else
                {
                    errorBox.style.display = DisplayStyle.Flex;
                    string tmp = $"- ";
                    if (ErrorList.Count == 1) tmp = "";
                    for (int i = 0; i < ErrorList.Count; i++)
                    {
                        tmp += ErrorList[i];
                        if(i != ErrorList.Count - 1) { tmp += "\n- "; }
                    }
                    errorBox.text = tmp;
                }
            }
            HelpBox warningBox = titleContainer.Query<HelpBox>("WarningContainer").First();
            if (warningBox != null)
            {
                if (WarningList.Count < 1) { warningBox.style.display = DisplayStyle.None; }
                else
                {
                    warningBox.style.display = DisplayStyle.Flex;
                    string tmp = $"- ";
                    if (WarningList.Count == 1) tmp = "";
                    for (int i = 0; i < WarningList.Count; i++)
                    {
                        tmp += WarningList[i];
                        if (i != WarningList.Count - 1) { tmp += "\n- "; }
                    }
                    warningBox.text = tmp;
                }
            }
        }

        public virtual void SetValidation() { }


    }
}