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

    private void Awake() {
        currentSprite = transform.gameObject.AddComponent<SpriteRenderer>();
        GameObject changeBackgroundObject = new GameObject("change sprite");
        changeBackgroundObject.transform.SetParent(currentSprite.transform);
        changeSprite = changeBackgroundObject.AddComponent<SpriteRenderer>();
        changeSprite.sortingOrder = currentSprite.sortingOrder + 1;
        changeSprite.enabled = false;
    }

    private void Start() {
        if (spriteList == null) {
            Debug.LogError("not find spriteList");
        }
    }

    public void SetSpriteList(List<Sprite> sprites) {
        spriteList = sprites;
    }

    public void SetPosision(Vector3 posision) {
        transform.position = posision;
    }

    public void ShowSpriteInstant(int index) {
        if (index >= 0 && index < spriteList.Count) {
            currentSprite.sprite = spriteList[index];
        }
        else {
            Debug.LogWarning("Invalid background index: " + index);
        }
    }

    public IEnumerator ChangeSpriteCrossfade(int index, float transitionDuration = defaultDuration) {
        if (!isTransitioning && index >= 0 && index < spriteList.Count) {
            isTransitioning = true;
            
            if (currentSprite.sprite == null) {
                StartCoroutine(FadeInSprite(index, transitionDuration));
            }
            else {
                changeSprite.enabled = true;
                changeSprite.sprite = spriteList[index];
                StartCoroutine(FadeInChangeSprite(transitionDuration));
                StartCoroutine(FadeOutSprite( transitionDuration));
            }
            yield return new WaitForSeconds(transitionDuration);

            currentSprite.sprite = spriteList[index];
            currentSprite.color = new Color(1f, 1f, 1f, 1f);
            changeSprite.enabled = false;
            isTransitioning = false;
        }
        else {
            Debug.LogWarning("Invalid background index: " + index);
        }
    }

    public IEnumerator MoveSpritePosision(Vector2 movePos, float transitionDuration = defaultDuration) {
        if (!isTransitioning) {
            isTransitioning = true;

            Vector2 startPos = currentSprite.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.transform.position = Vector3.Lerp(startPos, movePos, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.position = movePos;
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

    public IEnumerator FadeInSprite(int index, float transitionDuration = defaultDuration) {
        isTransitioning = true;
        float elapsedTime = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = new Color(1f, 1f, 1f, 1f);
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
        Color startColor = new Color(1f, 1f, 1f, 1f);
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
        Color endColor = new Color(1f, 1f, 1f, 1f);

        while (elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            changeSprite.color = Color.Lerp(startColor, endColor, normalizedTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
