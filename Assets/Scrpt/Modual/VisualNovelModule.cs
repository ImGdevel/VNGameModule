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
            LoadScenario(1); // 예시로 첫 번째 시나리오를 로드
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
            // 클릭으로 대사 넘기기 등 플레이어와의 상호작용 관리
            if (Input.GetMouseButtonDown(0)) {
                scenarioController.NextDialogue();
            }
        }
    }


}

