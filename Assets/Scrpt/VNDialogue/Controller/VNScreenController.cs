using UnityEngine;
using UnityEngine.UI;

public class VNScreenController : MonoBehaviour
{
    void Start() {
        // Canvas에 연결된 RectTransform 가져오기
        RectTransform canvasRect = GetComponent<RectTransform>();

        // 자신의 RectTransform을 부모 크기에 딱 맞게 조정
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // 자신의 RectTransform에 Image 컴포넌트 추가 (검은 화면 역할)
        Image blackOutImage = gameObject.AddComponent<Image>();
        blackOutImage.color = Color.black;
        blackOutImage.raycastTarget = false; // 이벤트 무시

        // 자식 UI 요소 추가 및 크기 설정 (예: 텍스트 UI 요소 추가)
        GameObject childObject = new GameObject("ChildUI");
        RectTransform childRectTransform = childObject.AddComponent<RectTransform>();
        childRectTransform.SetParent(rectTransform); // 부모 설정
        childRectTransform.anchorMin = Vector2.zero;
        childRectTransform.anchorMax = Vector2.one;
        childRectTransform.offsetMin = Vector2.zero;
        childRectTransform.offsetMax = Vector2.zero;

        Text textComponent = childObject.AddComponent<Text>();
        textComponent.text = "Hello, World!";
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // 폰트 설정
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;
    }
}
