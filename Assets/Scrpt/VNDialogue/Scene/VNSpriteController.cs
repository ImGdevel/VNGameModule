using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNSpriteController : MonoBehaviour
{
    private SpriteRenderer currentSprite;
    private SpriteRenderer changeSprite;
    private const float defaultDuration = 0.1f;
    private List<Sprite> spriteList;
    private bool isTransitioning = false;

    private void Awake() {
        InitializeSprites();
    }

    private void InitializeSprites() {
        currentSprite = gameObject.GetComponent<SpriteRenderer>();
        if (currentSprite == null) {
            currentSprite = gameObject.AddComponent<SpriteRenderer>();
        }
        GameObject changeBackgroundObject = new GameObject("ChangeSprite");
        changeBackgroundObject.transform.SetParent(transform);
        changeSprite = changeBackgroundObject.AddComponent<SpriteRenderer>();
        changeSprite.sortingOrder = currentSprite.sortingOrder + 1;
        changeSprite.enabled = false;
    }

    public void SetSpriteList(List<Sprite> sprites) {
        spriteList = sprites;
    }

    public void ShowSpriteInstant(int index) {
        if (IsValidIndex(index)) {
            currentSprite.sprite = spriteList[index];
        }
        else {
            Debug.LogWarning("Invalid sprite index: " + index);
        }
    }

    public IEnumerator ChangeSpriteCrossfade(int index, float transitionDuration = defaultDuration) {
        if (!isTransitioning && IsValidIndex(index)) {
            isTransitioning = true;

            if (currentSprite.sprite == null) {
                StartCoroutine(FadeInSprite(index, transitionDuration));
            }
            else {
                changeSprite.enabled = true;
                changeSprite.sprite = spriteList[index];
                StartCoroutine(FadeInChangeSprite(transitionDuration));
                StartCoroutine(FadeOutSprite(transitionDuration));
            }

            yield return new WaitForSeconds(transitionDuration);

            currentSprite.sprite = spriteList[index];
            currentSprite.color = Color.white;
            changeSprite.enabled = false;
            isTransitioning = false;
        }
        else {
            Debug.LogWarning("Invalid sprite index: " + index);
        }
    }

    public IEnumerator MoveSpritePosition(Vector2 movePosition, float transitionDuration = defaultDuration) {
        if (!isTransitioning) {
            isTransitioning = true;
            Vector3 startPosition = currentSprite.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.transform.position = Vector3.Lerp(startPosition, movePosition, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.position = movePosition;
            isTransitioning = false;
        }
    }

    public IEnumerator ZoomSprite(Vector2 zoomScale, float transitionDuration = defaultDuration) {
        if (!isTransitioning) {
            Vector2 startScale = currentSprite.transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.transform.localScale = Vector2.Lerp(startScale, zoomScale, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.localScale = zoomScale;
        }
    }

    private bool IsValidIndex(int index) {
        return index >= 0 && index < spriteList.Count;
    }

    public IEnumerator FadeInSprite(int index, float transitionDuration = defaultDuration) {
        isTransitioning = true;
        float elapsedTime = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = Color.white;
        currentSprite.sprite = spriteList[index];

        while (elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isTransitioning = false;
    }

    public IEnumerator FadeOutSprite(float transitionDuration = defaultDuration) {
        isTransitioning = true;
        float elapsedTime = 0f;
        Color startColor = Color.white;
        Color endColor = new Color(1f, 1f, 1f, 0f);

        while (elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentSprite.sprite = null;
        isTransitioning = false;
    }

    private IEnumerator FadeInChangeSprite(float transitionDuration = defaultDuration) {
        float elapsedTime = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = Color.white;

        while (elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            changeSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ChangeSpriteWithFadeToBlack(int index, float transitionDuration = defaultDuration) {
        if (!isTransitioning && IsValidIndex(index)) {
            isTransitioning = true;
            float elapsedTime = 0f;
            Color startColor = currentSprite.color;
            Color endColor = new Color(0f, 0f, 0f, 1f); // 검은 화면

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.color = endColor;
            currentSprite.sprite = spriteList[index];

            elapsedTime = 0f;
            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.color = Color.Lerp(endColor, startColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.color = startColor;
            isTransitioning = false;
        }
        else {
            Debug.LogWarning("Invalid sprite index: " + index);
        }
    }
}
