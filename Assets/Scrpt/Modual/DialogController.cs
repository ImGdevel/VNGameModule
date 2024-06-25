using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VisualNovelGame;

public class DialogController : MonoBehaviour, IController
{
    public void SetScenario(Scenario scenario)
    {
        // 배경 관련 데이터 설정

        // 배경 id에 따라서 저장된 배경중에서 로드
    }

    [SerializeField] TMP_Text dialogTextMesh;
    [SerializeField] TMP_Text characterNameTextMesh;
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject nameBox;

    private bool isTyping = false;
    private float currentTypingSpeed;
    private float originTypingSpeed;
    private const float fastSkipTypingSpeed = 0.00001f;

    public event Action OnTypingEnd;

    void Awake()
    {
        dialogTextMesh = dialogTextMesh.GetComponent<TMP_Text>();
        characterNameTextMesh = characterNameTextMesh.GetComponent<TMP_Text>();

        nameBox.SetActive(false);
        dialogTextMesh.text = "";
        characterNameTextMesh.text = "";
    }

    public void TypeDialogue(string charcterName, string content, float typingSpeed)
    {
        if (!isTyping) {
            originTypingSpeed = typingSpeed;
            TypeCharacterName(charcterName);
            StartCoroutine(TypeText(content, typingSpeed));
        }
        else {
            isTyping = false;
            StopCoroutine("TypeText");
        }
    }

    public void SkipDialogue(string charcterName, string content)
    {
        if (!isTyping) {
            TypeCharacterName(charcterName);
            StartCoroutine(TypeText(content, fastSkipTypingSpeed));
        }
    }

    public void ClearDialogue()
    {
        isTyping = false;
        dialogTextMesh.text = "";
        TypeCharacterName("");
    }

    public void CurrentDialogueSkip()
    {
        currentTypingSpeed = fastSkipTypingSpeed;
    }

    private void TypeCharacterName(string name)
    {
        if (!string.IsNullOrEmpty(name)) {
            nameBox.SetActive(true);
            characterNameTextMesh.text = name;
        }
        else {
            characterNameTextMesh.text = "";
            nameBox.SetActive(false);
        }
    }

    private IEnumerator TypeText(string text, float speed)
    {
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
        OnTypingEnd?.Invoke(); // 이벤트 발생
        isTyping = false;
    }


}