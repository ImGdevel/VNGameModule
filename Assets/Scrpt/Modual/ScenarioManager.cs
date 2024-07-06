using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;

namespace VisualNovelGame
{

    public class ScenarioManager : MonoBehaviour
    {
        private Dictionary<int, Scenario> scenarios;
        private DataManager<Dictionary<int, Scenario>> scenariosDataManager;

        void Start()
        {
            scenarios = new Dictionary<int, Scenario>();
            scenariosDataManager = new DataManager<Dictionary<int, Scenario>>("scenarios_index");
        }

        public void LoadScenarios(string path)
        {
            scenarios = scenariosDataManager.LoadData();
        }

        public Scenario GetScenario(int id)
        {
            if (scenarios.ContainsKey(id)) {
                return scenarios[id];
            }
            return null;
        }

        public void SaveScenario()
        {
            scenariosDataManager.SaveData(scenarios);
        }

        // �����͸� ���� ���� �޼���
        public void OpenScenarioEditor()
        {
            ScenarioEditorWindow.ShowWindow();
        }
    }
}

