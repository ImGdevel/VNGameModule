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

    private void OnDestroy() {
        StopAllCoroutines();
    }

    private void InitializeSprites() {
        currentSprite = GetComponent<SpriteRenderer>();
        if (currentSprite == null) {
            currentSprite = gameObject.AddComponent<SpriteRenderer>();
        }
        GameObject changeSpriteObject = new GameObject("ChangeSprite");
        changeSpriteObject.transform.SetParent(transform);
        changeSprite = changeSpriteObject.AddComponent<SpriteRenderer>();
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
    }

    public IEnumerator ChangeSpriteCrossFade(int index, float transitionDuration = defaultDuration) {
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
    }

    public IEnumerator MoveSpritePosition(Vector3 movePosition, float transitionDuration = defaultDuration) {
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

    public IEnumerator ZoomSprite(Vector3 zoomScale, float transitionDuration = defaultDuration) {
        if (!isTransitioning) {
            Vector3 startScale = currentSprite.transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.transform.localScale = Vector3.Lerp(startScale, zoomScale, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.localScale = zoomScale;
        }
    }

    private bool IsValidIndex(int index) {
        bool isValid = index >= 0 && index < spriteList.Count;
        if (!isValid) {
            Debug.LogWarning("Invalid sprite index: " + index);
        }
        return isValid;
    }

    public IEnumerator FadeInSprite(int index, float transitionDuration = defaultDuration) {
        if (!isTransitioning && IsValidIndex(index)) {
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
            Color endColor = new Color(0f, 0f, 0f, 1f);

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
    }
}
