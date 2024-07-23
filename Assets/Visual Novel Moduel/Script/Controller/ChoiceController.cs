using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class ChoiceController : MonoBehaviour
{
    [SerializeField] private Sprite buttonSprite; // 버튼에 사용할 스프라이트
    [SerializeField] private float buttonWidth = 160f;
    [SerializeField] private float buttonHeight = 40f;
    [SerializeField] private VerticalLayoutGroup choiceButtonContainer;

    private void Start()
    {
        if (choiceButtonContainer == null) {
            Debug.LogError("ChoiceButtonContainer is not assigned.");
        }
    }

    public void AddChoiceDialogue(List<string> texts, List<UnityAction> actions)
    {
        // 기존 버튼들을 삭제
        ClearChoices();

        // 텍스트와 액션 리스트의 길이가 일치하는지 확인
        if (texts.Count != actions.Count) {
            Debug.LogError("Texts and actions lists must have the same length.");
            return;
        }

        // 텍스트와 액션 리스트를 순회하면서 버튼을 생성
        for (int i = 0; i < texts.Count; i++) {
            // 버튼과 텍스트 UI 요소를 동적으로 생성
            GameObject buttonObject = new GameObject("ChoiceButton" + i);
            buttonObject.transform.SetParent(choiceButtonContainer.transform, false);

            // 버튼 설정
            Button button = buttonObject.AddComponent<Button>();
            Image image = buttonObject.AddComponent<Image>();

            if (image != null && buttonSprite != null) {
                image.sprite = buttonSprite;
                image.type = Image.Type.Sliced; // 버튼에 적합한 타입 설정
            }

            // 텍스트 설정
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            TMP_Text buttonText = textObject.AddComponent<TextMeshProUGUI>(); // TextMeshProUGUI 사용
            if (buttonText != null) {
                buttonText.text = texts[i];
                buttonText.fontSize = 24; // 텍스트 크기 설정
                buttonText.alignment = TextAlignmentOptions.Center; // 텍스트 정렬 설정
                buttonText.color = Color.black; // 텍스트 색상 설정
            }

            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
            if (textRectTransform != null) {
                textRectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
                textRectTransform.anchoredPosition = Vector2.zero; // 텍스트 위치를 중앙으로 설정
            }

            // 버튼 클릭 시 액션 설정
            int index = i; // 지역 변수로 인덱스를 캡처
            button.onClick.AddListener(() => {
                actions[index]?.Invoke(); // 액션이 null이 아닌지 확인
                ClearChoices(); // 선택 후 버튼들을 삭제
            });

            // 버튼의 RectTransform 설정
            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            if (rectTransform != null) {
                rectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            }
        }
    }

    private void ClearChoices()
    {
        // choiceButtonContainer의 모든 자식 버튼들을 제거
        foreach (Transform child in choiceButtonContainer.transform) {
            Destroy(child.gameObject);
        }
    }
}
