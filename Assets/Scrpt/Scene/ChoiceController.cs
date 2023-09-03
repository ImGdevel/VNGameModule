using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChoiceController : MonoBehaviour
{
    [SerializeField] private GameObject choiceButtonPrefab; 
    [SerializeField] private Transform choiceButtonContainer; 

    public event Action<string> ChoiceScene;

    public void ShowChoices(List<ChoiceData> choices) {
        ClearChoices();

        foreach (ChoiceData choice in choices) {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.text;
            Button button = choiceButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private void ClearChoices() {
        foreach (Transform child in choiceButtonContainer) {
            Destroy(child.gameObject);
        }
    }

    private void OnChoiceSelected(ChoiceData choice) {
        if (!string.IsNullOrEmpty(choice.nextDialog)) {
            ChoiceScene?.Invoke(choice.nextDialog);
        }
        else {
            ProcessChoiceEvent(choice);
        }
        ClearChoices();
    }

    private void ProcessChoiceEvent(ChoiceData choice) {
        Debug.Log("?");
    }
}
