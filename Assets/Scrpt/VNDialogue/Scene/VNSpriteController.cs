using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNSpriteController : MonoBehaviour
{
    private SpriteRenderer currentSprite;
    private SpriteRenderer changeSprite;

    private List<Sprite> spriteList;
    private const float DefaultDuration = 0.1f;
    private bool isTransitioning = false;

    private void Awake() {
        currentSprite = gameObject.AddComponent<SpriteRenderer>();
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

    public IEnumerator ChangeSpriteCrossfade(int index, float transitionDuration = DefaultDuration) {
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

    public IEnumerator MoveSpritePosition(Vector2 movePosition, float transitionDuration = DefaultDuration) {
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

    public IEnumerator ZoomSprite(Vector2 zoomScale, float transitionDuration = DefaultDuration) {
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

    public IEnumerator FadeInSprite(int index, float transitionDuration = DefaultDuration) {
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

    public IEnumerator FadeOutSprite(float transitionDuration = DefaultDuration) {
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

    private IEnumerator FadeInChangeSprite(float transitionDuration = DefaultDuration) {
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
}
