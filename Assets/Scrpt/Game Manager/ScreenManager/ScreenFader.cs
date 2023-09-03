using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage; 
    
    private Color originalColor;
    private const float fadeDuration = 1.0f;

    private void Awake() {
        if (fadeImage == null) {
            Debug.Log("스크린 이미지를 찾을 수 없습니다.");
        }
        
        originalColor = fadeImage.color;
        fadeImage.gameObject.SetActive(true);
        FadeIn();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            FadeOut();
        }
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
