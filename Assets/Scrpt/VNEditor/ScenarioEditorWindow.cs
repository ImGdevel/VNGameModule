using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualNovelGame
{
    public class ScenarioEditorWindow : EditorWindow
    {
        private ScenarioGraphView graphView;
        private ScenarioManager scenarioManager;

        [MenuItem("Window/Visual Novel Scenario Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<ScenarioEditorWindow>("Scenario Editor");
            window.minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            scenarioManager = FindObjectOfType<ScenarioManager>();
            if (scenarioManager == null) {
                Debug.LogError("ScenarioManager not found in the scene.");
                return;
            }

            ConstructGraphView();
            GenerateToolbar();
        }

        private void ConstructGraphView()
        {
            graphView = new ScenarioGraphView(scenarioManager) {
                name = "Scenario Graph"
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var dialogueNodeButton = new Button(() => { graphView.CreateDialogueNode("Dialogue Node"); });
            dialogueNodeButton.text = "Create Dialogue Node";
            toolbar.Add(dialogueNodeButton);

            var choiceNodeButton = new Button(() => { graphView.CreateChoiceNode("Choice Node"); });
            choiceNodeButton.text = "Create Choice Node";
            toolbar.Add(choiceNodeButton);

            rootVisualElement.Add(toolbar);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }
    }
}
