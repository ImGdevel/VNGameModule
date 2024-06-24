using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VisualNovelGame
{

    public class VisualNovelModule : MonoBehaviour
    {
        public ScenarioManager scenarioManager;
        public ScenarioController scenarioController;
        public CharacterController characterController;
        public BackgroundController backgroundController;
        public MusicController musicController;

        private int currentScenarioId;

        void Start()
        {
            LoadScenario(1); // ���÷� ù ��° �ó������� �ε�
        }

        public void LoadScenario(int scenarioId)
        {
            Scenario scenario = scenarioManager.GetScenario(scenarioId);
            if (scenario != null) {
                currentScenarioId = scenarioId;
                scenarioController.SetScenario(scenario);
                characterController.SetScenario(scenario);
                backgroundController.SetScenario(scenario);
                musicController.SetScenario(scenario);
            }
        }

        void Update()
        {
            // Ŭ������ ��� �ѱ�� �� �÷��̾���� ��ȣ�ۿ� ����
            if (Input.GetMouseButtonDown(0)) {
                scenarioController.NextDialogue();
            }
        }
    }


}

