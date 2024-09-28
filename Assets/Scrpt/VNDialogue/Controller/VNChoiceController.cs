using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class VNChoiceController : MonoBehaviour
{
    [SerializeField] private GameObject choiceButtonPrefab; 
    [SerializeField] private Transform choiceButtonContainer; 

    public event Action<string> ChoiceScene;

    /// <summary>
    /// 선택지 디스플레이
    /// </summary>
    /// <param name="choices">선택지</param>
    public void ShowChoices(List<Choice> choices) {
        ClearChoices();

        foreach (Choice choice in choices) {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.ChoiceText;
            Button button = choiceButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    /// <summary>
    /// 시간제한 타이머
    /// </summary>
    /// <param name="choices"></param>
    /// <param name="timer"></param>
    public void ShowChoicesWithTimer(List<Choice> choices, float timer)
    {
        ClearChoices();

        foreach (Choice choice in choices) {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.ChoiceText;
            Button button = choiceButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }



    /// <summary>
    /// 선택지 선택시 이벤트
    /// </summary>
    /// <param name="choice"></param>
    private void OnChoiceSelected(Choice choice) {
        if (!string.IsNullOrEmpty(choice.nextScriptId)) {
            ChoiceScene?.Invoke(choice.nextScriptId);
        }
        ClearChoices();
    }

    /// <summary>
    /// 선택지 비우기
    /// </summary>
    private void ClearChoices()
    {
        foreach (Transform child in choiceButtonContainer) {
            Destroy(child.gameObject);
        }
    }
}
