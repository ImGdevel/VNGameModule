using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

namespace VNGameModuel.Controller
{

    public class DialogueBoxController : BaseController
    {
        [SerializeField] TMP_Text dialogTextMesh;
        [SerializeField] TMP_Text characterNameTextMesh;

        private bool isTyping = false;
        private float currentTypingSpeed;
        private float originTypingSpeed;
        private const float fastSkipTypingSpeed = 0.00001f;

        public event Action OnEventEnd;


        void Awake()
        {
            dialogTextMesh = dialogTextMesh.GetComponent<TMP_Text>();
            characterNameTextMesh = characterNameTextMesh.GetComponent<TMP_Text>();

            ClearDialogue();
        }

        public void TypeDialogue(string characterName, string content, float typingSpeed = 0.1f)
        {
            if (!isTyping) {
                originTypingSpeed = typingSpeed;
                TypeCharacterName(characterName);
                StartCoroutine(TypeText(content, typingSpeed));
            }
            else {
                StopDialogue();
            }
        }



        public void SkipDialogue(string characterName, string content)
        {
            if (!isTyping) {
                TypeCharacterName(characterName);
                StartCoroutine(TypeText(content, fastSkipTypingSpeed));
            }
        }

        public void ClearDialogue()
        {
            StopDialogue();
            dialogTextMesh.text = "";
            TypeCharacterName("");
        }

        public void StopDialogue()
        {
            isTyping = false;
        }

        public bool IsTyping()
        {
            return isTyping;
        }

        public void CurrentDialogueSkip()
        {
            currentTypingSpeed = fastSkipTypingSpeed;
        }

        private void TypeCharacterName(string name)
        {
            if (!string.IsNullOrEmpty(name)) {
                characterNameTextMesh.text = name;
            }
            else {
                characterNameTextMesh.text = "";
            }
        }

        private IEnumerator TypeText(string text, float speed)
        {
            isTyping = true;
            dialogTextMesh.text = "";
            currentTypingSpeed = speed;

            string currentText = "";
            Stack<string> tagStack = new Stack<string>();

            for (int i = 0; i < text.Length; i++) {
                if (!isTyping)
                    break;

                if (text[i] == '<') {
                    int closingIndex = text.IndexOf('>', i);
                    if (closingIndex > -1) {
                        string tag = text.Substring(i, closingIndex - i + 1);
                        currentText += tag;
                        if (!tag.Contains("/")) {
                            tagStack.Push(tag);
                        }
                        else {
                            tagStack.Pop();
                        }
                        i = closingIndex;
                    }
                }
                else {
                    currentText += text[i];
                    dialogTextMesh.text = currentText + string.Concat(tagStack);
                    yield return new WaitForSeconds(currentTypingSpeed);
                }
            }
            currentTypingSpeed = originTypingSpeed;
            dialogTextMesh.text = text;
            OnEventEnd?.Invoke();
            isTyping = false;
        }
    }
}
