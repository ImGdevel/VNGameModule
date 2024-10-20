using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Assertions.Must;

public class VNDialogController : MonoBehaviour
{
    [SerializeField] TMP_Text dialogTextMesh;
    [SerializeField] TMP_Text characterNameTextMesh;
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject nameBox;

    private bool isTyping = false;
    private float currentTypingSpeed;
    private float originTypingSpeed;
    private const float fastSkipTypingSpeed = 0.00001f;

    public event Action OnTypingEnd;

    void Awake() {
        dialogTextMesh = dialogTextMesh.GetComponent<TMP_Text>();
        characterNameTextMesh = characterNameTextMesh.GetComponent<TMP_Text>();

        nameBox.SetActive(false);
        dialogTextMesh.text = "";
        characterNameTextMesh.text = "";
    }

    private void Start()
    {
        VNDialogueModule.ForceTerminateScene += StopTyping;
    }

    private void OnDestroy()
    {
        VNDialogueModule.ForceTerminateScene -= StopTyping;
    }

    public void TypeDialogue(string charcterName, string content, float typingSpeed) {
        if (!isTyping) {
            originTypingSpeed = typingSpeed;
            TypeCharacterName(charcterName);
            StartCoroutine(TypeText(content, typingSpeed));
        }
        else {
            StopTyping();
        }
    }

    public void StopTyping()
    {
        isTyping = false;
        StopCoroutine("TypeText");
    }

    public void SkipDialogue(string charcterName, string content) {
        if (!isTyping) {
            TypeCharacterName(charcterName);
            StartCoroutine(TypeText(content, fastSkipTypingSpeed));
        }
    }

    public void ClearDialogue() {
        isTyping = false;
        dialogTextMesh.text = "";
        TypeCharacterName("");
    }

    public void CurrentDialogueSkip() {
        currentTypingSpeed = fastSkipTypingSpeed;
    }

    private void TypeCharacterName(string name) {
        if (!string.IsNullOrEmpty(name)) {
            nameBox.SetActive(true);
            characterNameTextMesh.text = name;
        }
        else {
            characterNameTextMesh.text = "";
            nameBox.SetActive(false);
        }
    }

    private IEnumerator TypeText(string text, float speed) {
        string currentText = "";
        isTyping = true;
        dialogTextMesh.text = "";
        currentTypingSpeed = speed;

        foreach (char c in text) {
            if (!isTyping)
                break;

            currentText += c;
            dialogTextMesh.text = currentText;
            yield return new WaitForSeconds(currentTypingSpeed);
        }

        currentTypingSpeed = originTypingSpeed;
        dialogTextMesh.text = text;
        OnTypingEnd?.Invoke();
        isTyping = false;
    }
}
