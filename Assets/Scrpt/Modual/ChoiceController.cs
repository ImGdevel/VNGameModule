using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using VisualNovelGame;



public class ChoiceController : MonoBehaviour, IController
{

    public void SetScenario(Scenario scenario)
    {
        // 배경 관련 데이터 설정

        // 포이스 id에 따라...
    }

    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceButtonContainer;

    public event Action<string> ChoiceScene;

    public void ShowChoices(List<ChoiceData> choices)
    {
        ClearChoices();

        foreach (ChoiceData choice in choices) {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.text;
            Button button = choiceButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceButtonContainer) {
            Destroy(child.gameObject);
        }
    }

    private void OnChoiceSelected(ChoiceData choice)
    {
        if (!string.IsNullOrEmpty(choice.nextDialog)) {
            ChoiceScene?.Invoke(choice.nextDialog);
        }
        else {
            ProcessChoiceEvent(choice);
        }
        ClearChoices();
    }

    private void ProcessChoiceEvent(ChoiceData choice)
    {
        Debug.Log("?");
    }


}