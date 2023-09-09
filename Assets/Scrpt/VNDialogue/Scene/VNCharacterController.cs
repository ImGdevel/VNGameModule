using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCharacterController : MonoBehaviour
{
    [SerializeField]
    private SpriteList spriteDatas;

    [SerializeField]
    private SpriteRenderer currentSprite;
    private SpriteRenderer changeSprite;

    [SerializeField]
    private float transitionDuration = 0.5f;

    private List<Sprite> spriteList;
    private int currentIndex = 0;
    private bool isTransitioning = false;
     
    private void Awake() {
        if (currentSprite == null) {
            currentSprite = transform.gameObject.AddComponent<SpriteRenderer>();
        }
        if (changeSprite == null) {
            GameObject changeBackgroundObject = new GameObject("change character");
            changeBackgroundObject.transform.SetParent(currentSprite.transform);
            changeSprite = changeBackgroundObject.AddComponent<SpriteRenderer>();
            changeSprite.sortingOrder = currentSprite.sortingOrder + 1;
            changeSprite.enabled = false;
        }
    }
     
    private void Start() {
        spriteList = spriteDatas.spriteList;
    }
     
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isTransitioning) {
            Debug.Log("배경 변경 (즉시)");
            int newIndex = (currentIndex) % spriteList.Count;
            ShowCharacterInstant(newIndex);
        }

        if (Input.GetKeyDown(KeyCode.G) && !isTransitioning) {
            Debug.Log("배경 변경 (크로스페이드)");
            int newIndex = (currentIndex) % spriteList.Count;
            StartCoroutine(ShowCharcaterCrossfade(newIndex));
        }

        if (Input.GetKeyDown(KeyCode.J) && !isTransitioning) {
            Debug.Log("페이드 아웃");
            int newIndex = (currentIndex) % spriteList.Count;
            
            StartCoroutine(MoveCharacterPosision(new Vector3(1, 0, 0),0.5f));
        }

        if (Input.GetKeyDown(KeyCode.H) && !isTransitioning) {
            Debug.Log("페이드 아웃");
            int newIndex = (currentIndex) % spriteList.Count;
            StartCoroutine(FadeOutCharacter());
        }
    }

    private void ShowCharacterInstant(int newIndex) {
        if (newIndex >= 0 && newIndex < spriteList.Count) {
            currentIndex = newIndex;
            currentSprite.sprite = spriteList[currentIndex];
        }
        else {
            Debug.LogWarning("Invalid background index: " + newIndex);
        }
    }

    private IEnumerator ShowCharcaterCrossfade(int newIndex) {
        if (newIndex >= 0 && newIndex < spriteList.Count) {
            isTransitioning = true;
            float elapsedTime = 0f;
            Color startColor = new Color(1f, 1f, 1f, 0f);
            Color endColor = changeSprite.color;

            changeSprite.enabled = true;
            changeSprite.color = startColor;

            currentIndex = newIndex;
            changeSprite.sprite = spriteList[currentIndex];

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                changeSprite.color = Color.Lerp(startColor, endColor, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.sprite = spriteList[currentIndex];
            currentSprite.color = endColor;
            changeSprite.color = endColor;
            changeSprite.enabled = false;
            isTransitioning = false;
        }
        else {
            Debug.LogWarning("Invalid background index: " + newIndex);
        }
    }

    private IEnumerator MoveCharacterPosision(Vector3 movePos, float moveTime) {
        if (!isTransitioning) {
            isTransitioning = true;

            Vector3 startPos = currentSprite.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < moveTime) {
                float normalizedTime = elapsedTime / moveTime;
                currentSprite.transform.position = Vector3.Lerp(startPos, movePos, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.position = movePos;
            isTransitioning = false;
        }
    }

    private IEnumerator ZoomCharacter(Vector3 zoomScale, float zoomTime) {
        if (!isTransitioning) {
            isTransitioning = true;

            Vector3 startScale = currentSprite.transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < zoomTime) {
                float normalizedTime = elapsedTime / zoomTime;
                currentSprite.transform.localScale = Vector3.Lerp(startScale, zoomScale, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.localScale = zoomScale;
            isTransitioning = false;
        }
    }

    private IEnumerator FadeOutCharacter() {
        isTransitioning = true;
        float elapsedTime = 0f;
        Color startColor = currentSprite.color;
        Color endColor = new Color(1f, 1f, 1f, 0f);

        while (elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isTransitioning = false;
    }
}
