using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VisualNovelGame
{
    public class DialogueUI : MonoBehaviour
    {
        public GameObject dialoguePrefab;
        private GameObject dialogueInstance;

        void Start()
        {
            if (dialoguePrefab != null) {
                dialogueInstance = Instantiate(dialoguePrefab, transform);
                dialogueInstance.SetActive(false); // �⺻������ ��Ȱ��ȭ
            }
        }

        public void ShowDialogue(string text)
        {
            dialogueInstance.SetActive(true);
            dialogueInstance.GetComponentInChildren<Text>().text = text;
        }

        public void HideDialogue()
        {
            dialogueInstance.SetActive(false);
        }
    }
}
