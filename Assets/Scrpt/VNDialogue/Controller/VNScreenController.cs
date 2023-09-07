using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VNScreenController : MonoBehaviour
{
    public static VNScreenController Instance { get; private set; }

    private Image fadeImage;
    private Color originalColor;

    private const float fadeDuration = 1.0f;

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        CreateFadeScreen();

        FadeIn();
    }

    private void CreateFadeScreen() {
        GameObject childObject = new GameObject("FadeScreen");
        RectTransform childRectTransform = childObject.AddComponent<RectTransform>();
        childRectTransform.SetParent(transform, false);
        childRectTransform.anchorMin = Vector2.zero;
        childRectTransform.anchorMax = Vector2.one;
        childRectTransform.offsetMin = Vector2.zero;
        childRectTransform.offsetMax = Vector2.zero;

        Image childImage = gameObject.AddComponent<Image>();
        fadeImage = childImage;
        fadeImage.color = Color.black;
        originalColor = fadeImage.color;
        fadeImage.raycastTarget = false;
    }

    public void FadeIn(float fadeTime = fadeDuration) {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(Fade(fadeImage, originalColor, Color.clear, fadeTime));
    }

    public void FadeOut(float fadeTime = fadeDuration) {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(Fade(fadeImage, Color.clear, originalColor, fadeTime));

    }

    private IEnumerator Fade(Image image, Color startColor, Color targetColor, float duration) {
        float startTime = Time.time;

        while (Time.time - startTime < duration) {
            float normalizedTime = (Time.time - startTime) / duration;
            image.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }
        image.color = targetColor;
        fadeImage.gameObject.SetActive(targetColor != Color.clear);
    }
}
