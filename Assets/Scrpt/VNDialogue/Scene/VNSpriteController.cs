using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNSpriteController : MonoBehaviour
{
    private SpriteRenderer currentSprite;
    private SpriteRenderer changeSprite;

    private List<Sprite> spriteList;
    private const float defaultDuration = 0.1f;
    private bool isTransitioning = false;

    // Awake is used for initialization
    private void Awake() {
        InitializeSprites();
    }

    // Initialize sprite renderers
    private void InitializeSprites() {
        currentSprite = GetComponent<SpriteRenderer>();
        if (currentSprite == null) {
            currentSprite = gameObject.AddComponent<SpriteRenderer>();
        }

        // Create a separate GameObject for the changing sprite
        GameObject changeSpriteObject = new GameObject("ChangeSprite");
        changeSpriteObject.transform.SetParent(transform);
        changeSprite = changeSpriteObject.AddComponent<SpriteRenderer>();
        changeSprite.sortingOrder = currentSprite.sortingOrder + 1;
        changeSprite.enabled = false;
    }

    // Set the list of sprites to be used
    public void SetSpriteList(List<Sprite> sprites) {
        spriteList = sprites;
        if (spriteList == null || spriteList.Count == 0) {
            Debug.LogWarning("Sprite list is empty or null.");
        }
    }

    // Handle cleanup when the script is destroyed
    private void OnDestroy() {
        // Stop all running coroutines
        StopAllCoroutines();
    }

    // Show a sprite instantly without any transition
    public void ShowSpriteInstant(int index) {
        if (IsValidIndex(index)) {
            currentSprite.sprite = spriteList[index];
        }
    }

    // Coroutine for fading in a sprite
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

    // Coroutine for fading out a sprite
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

    // Coroutine for crossfading between sprites
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

    // Coroutine for changing the sprite with a fade to black effect
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

    // Coroutine for moving the sprite to a new position
    public IEnumerator MoveSpritePosition(Vector3 movePosition, float transitionDuration = defaultDuration) {
        if (!isTransitioning) {
            isTransitioning = true;
            Vector3 startPosition = currentSprite.transform.position;

            yield return LerpTransform(startPosition, movePosition, transitionDuration, newPosition => {
                currentSprite.transform.position = newPosition;
            });

            isTransitioning = false;
        }
    }

    // Coroutine for zooming the sprite
    public IEnumerator ZoomSprite(Vector3 zoomScale, float transitionDuration = defaultDuration) {
        if (!isTransitioning) {
            Vector3 startScale = currentSprite.transform.localScale;

            yield return LerpTransform(startScale, zoomScale, transitionDuration, newScale => {
                currentSprite.transform.localScale = newScale;
            });
        }
    }

    // Check if the index is valid for the sprite list
    private bool IsValidIndex(int index) {
        if (spriteList == null || index < 0 || index >= spriteList.Count) {
            Debug.LogWarning("Invalid sprite index: " + index);
            return false;
        }
        return true;
    }

    // Coroutine for fading in the change sprite
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

    // Coroutine for lerping transform properties (position and scale)
    private IEnumerator LerpTransform(Vector3 startValue, Vector3 endValue, float duration, System.Action<Vector3> updateAction) {
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float normalizedTime = elapsedTime / duration;
            updateAction(Vector3.Lerp(startValue, endValue, normalizedTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        updateAction(endValue);
    }

}
