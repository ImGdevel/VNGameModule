using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNBackgroundController : MonoBehaviour
{
    [SerializeField]
    private SpriteList backgroundList;

    [SerializeField]
    private SpriteRenderer backgroundSprite;
    private SpriteRenderer changeSprite;

    [SerializeField]
    private float transitionDuration = 1.0f;

    private List<Sprite> backgrounds;
    private int currentIndex = 0;
    private bool isTransitioning = false;

    private void Awake() {
        if(backgroundSprite == null) {
            backgroundSprite = transform.gameObject.AddComponent<SpriteRenderer>();
        }
        if(changeSprite == null) {
            GameObject changeBackgroundObject = new GameObject("change backgorund");
            changeBackgroundObject.transform.SetParent(backgroundSprite.transform);
            changeSprite = changeBackgroundObject.AddComponent<SpriteRenderer>();
            changeSprite.sortingOrder = backgroundSprite.sortingOrder + 1;
            changeSprite.enabled = false;
        }
    }

    private void Start() {
        backgrounds = backgroundList.spriteList;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.X) && !isTransitioning) {
            Debug.Log("배경 변경 (즉시)");
            int newIndex = (currentIndex + 1) % backgrounds.Count;
            ChangeBackgroundInstant(newIndex);
        }

        if (Input.GetKeyDown(KeyCode.C) && !isTransitioning) {
            Debug.Log("배경 변경 (크로스페이드)");
            int newIndex = (currentIndex + 1) % backgrounds.Count;
            StartCoroutine(ChangeBackgroundCrossfade(newIndex));
            
        }

        if (Input.GetKeyDown(KeyCode.V) && !isTransitioning) {
            Debug.Log("배경 변경 (페이드 아웃 후 페이드 인)");
            int newIndex = (currentIndex + 1) % backgrounds.Count;
            StartCoroutine(ChangeBackgroundWithFadeToBlack(newIndex));
        }
    }

    private void ChangeBackgroundInstant(int newIndex) {
        if (newIndex >= 0 && newIndex < backgrounds.Count) {
            currentIndex = newIndex;
            backgroundSprite.sprite = backgrounds[currentIndex];
        }
        else {
            Debug.LogWarning("Invalid background index: " + newIndex);
        }
    }

    private IEnumerator ChangeBackgroundCrossfade(int newIndex) {
        if (newIndex >= 0 && newIndex < backgrounds.Count) {
            isTransitioning = true;
            float elapsedTime = 0f;
            Color startColor = new Color(1f, 1f, 1f, 0f);
            Color endColor = changeSprite.color;

            changeSprite.enabled = true;
            changeSprite.color = startColor;

            currentIndex = newIndex;
            changeSprite.sprite = backgrounds[currentIndex];

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                changeSprite.color = Color.Lerp(startColor, endColor, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            backgroundSprite.sprite = backgrounds[currentIndex];
            changeSprite.color = endColor;
            changeSprite.enabled = false;
            isTransitioning = false;
        }
        else {
            Debug.LogWarning("Invalid background index: " + newIndex);
        }
    }

    private IEnumerator ChangeBackgroundWithFadeToBlack(int newIndex) {
        if (newIndex >= 0 && newIndex < backgrounds.Count) {
            isTransitioning = true;
            float elapsedTime = 0f;
            Color startColor = backgroundSprite.color;
            Color endColor = new Color(0f, 0f, 0f, 1f); // 검은 화면

            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                backgroundSprite.color = Color.Lerp(startColor, endColor, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            backgroundSprite.color = endColor;
            currentIndex = newIndex;

            if (currentIndex >= 0 && currentIndex < backgrounds.Count) {
                backgroundSprite.sprite = backgrounds[currentIndex];
            }

            elapsedTime = 0f;
            while (elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                backgroundSprite.color = Color.Lerp(endColor, startColor, normalizedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            backgroundSprite.color = startColor;
            isTransitioning = false;
        }
        else {
            Debug.LogWarning("Invalid background index: " + newIndex);
        }
    }
}
