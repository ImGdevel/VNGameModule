using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VisualNovelGame
{
    [CustomEditor(typeof(ScenarioManager))]
    public class ScenarioManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ScenarioManager scenarioManager = (ScenarioManager)target;

            if (GUILayout.Button("Open Scenario Editor")) {
                scenarioManager.OpenScenarioEditor();
            }
        }
    }
}
