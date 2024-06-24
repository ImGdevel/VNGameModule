using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;

namespace VisualNovelGame
{

    public class ScenarioManager : MonoBehaviour
    {
        private Dictionary<int, Scenario> scenarios = new Dictionary<int, Scenario>();
        
        public void LoadScenarios(string path)
        {
            
        }

        public Scenario GetScenario(int id)
        {
            if (scenarios.ContainsKey(id)) {
                return scenarios[id];
            }
            return null;
        }

        public void SaveScenario(Scenario scenario, string path)
        {
            // Save scenario to a file or resource
        }

        // �����͸� ���� ���� �޼���
        public void OpenScenarioEditor()
        {
            ScenarioEditorWindow.ShowWindow();
        }
    }
}

