using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;

namespace VisualNovelGame
{

    public class ScenarioController : MonoBehaviour, IController
    {
        private Scenario currentScenario;
        private int currentDialogueIndex;

        public void SetScenario(Scenario scenario)
        {
            currentScenario = scenario;
            currentDialogueIndex = 0;
            DisplayDialogue();
        }

        public void NextDialogue()
        {
            if (currentDialogueIndex < currentScenario.dialogues.Count - 1) {
                currentDialogueIndex++;
                DisplayDialogue();
            }
            else {
                // �ó������� ������ �� ó��
            }
        }

        private void DisplayDialogue()
        {
            // ��� ǥ�� ����
            Debug.Log(currentScenario.dialogues[currentDialogueIndex].text);
        }
    }

}

