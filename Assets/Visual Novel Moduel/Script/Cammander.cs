using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNGameModuel{
    public class Cammander : MonoBehaviour
    {
        public ScenarioManager scenarioManager;

        private bool isGamePaused = false;

        private KeyCode nextDialogueKey = KeyCode.Space;
        private KeyCode skipDialogueKey = KeyCode.LeftControl;
        private KeyCode autoDialogueKey = KeyCode.A;
        private KeyCode hideDialogueKey = KeyCode.Tab;

        void Start()
        {
            // 세팅 값을 가져와서 세팅
            scenarioManager.StartDialogue();
        }

        void Update()
        {
            if (isGamePaused) {
                return;
            }

            if (Input.GetKeyDown(nextDialogueKey) || Input.GetMouseButtonDown(0)) {
                // 다음 대화로
                Debug.Log("다음 대화로");
                scenarioManager.RunDialogue();
            }

            if (Input.GetKey(skipDialogueKey)) {

                Debug.Log("대화 스킵");
            }

            if (Input.GetKeyUp(skipDialogueKey)) {

                Debug.Log("대화 스킵 종료");
            }

        }
    }

}


