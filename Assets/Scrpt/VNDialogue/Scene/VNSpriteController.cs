using System.Collections;
using UnityEngine;

public class VNSpriteController : MonoBehaviour
{
    private SpriteRenderer currentSprite;
    private SpriteRenderer changeSprite;

    private const float defaultDuration = 0.1f;
    private bool isTransitioning = false;
    private bool isAnimationEnd = false;

    private void Awake()
    {
        InitializeSprites();
    }

    /// <summary>
    /// // 스프라이트 렌더러 초기화
    /// </summary>
    private void InitializeSprites()
    {
        currentSprite = GetComponent<SpriteRenderer>();
        if (currentSprite == null) {
            currentSprite = gameObject.AddComponent<SpriteRenderer>();
        }

        GameObject changeSpriteObject = new GameObject("ChangeSprite");
        changeSpriteObject.transform.SetParent(transform);
        changeSprite = changeSpriteObject.AddComponent<SpriteRenderer>();
        changeSprite.sortingOrder = currentSprite.sortingOrder + 1;
        changeSprite.enabled = false;

        VNDialogueModule.ForceTerminateScene += EndSpriteEffect;
    }

    /// <summary>
    /// 스크립트가 파괴될 때 정리 작업 수행
    /// </summary>
    private void OnDestroy()
    {
        VNDialogueModule.ForceTerminateScene -= EndSpriteEffect;
        StopAllCoroutines();
    }

    /// <summary>
    /// 스프라이트를 즉시 표시
    /// </summary>
    public void ShowSpriteInstant(Sprite sprite)
    {
        currentSprite.sprite = sprite;
    }

    /// <summary>
    /// 스프라이트 페이드 인 코루틴
    /// </summary>
    public IEnumerator FadeInSprite(Sprite sprite, float transitionDuration = defaultDuration)
    {
        if (!isTransitioning && sprite != null) {
            isTransitioning = true;
            isAnimationEnd = false;
            float elapsedTime = 0f;
            Color startColor = new Color(1f, 1f, 1f, 0f);
            Color endColor = Color.white;

            currentSprite.sprite = sprite;

            while (!isAnimationEnd && elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isTransitioning = false;
        }
    }

    /// <summary>
    /// 스프라이트 페이드 아웃 코루틴
    /// </summary>
    public IEnumerator FadeOutSprite(float transitionDuration = defaultDuration)
    {
        isTransitioning = true;
        isAnimationEnd = false;
        float elapsedTime = 0f;
        Color startColor = Color.white;
        Color endColor = new Color(1f, 1f, 1f, 0f);

        while (!isAnimationEnd && elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSprite.sprite = null;
        isTransitioning = false;
    }

    /// <summary>
    /// 스프라이트 교차 페이드 전환
    /// </summary>
    public IEnumerator ChangeSpriteCrossFade(Sprite sprite, float transitionDuration = defaultDuration)
    {
        if (!isTransitioning && sprite != null) {
            float elapsedTime = 0f;
            isTransitioning = true;
            isAnimationEnd = false;

            changeSprite.enabled = true;
            changeSprite.sprite = sprite;

            StartCoroutine(FadeInChangeSprite(transitionDuration));
            StartCoroutine(FadeOutSprite(transitionDuration));

            while (!isAnimationEnd && elapsedTime < transitionDuration) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.sprite = sprite;
            currentSprite.color = Color.white;
            changeSprite.enabled = false;
            isTransitioning = false;
        }
    }

    /// <summary>
    /// 스프라이트를 검은색으로 페이드 처리한 후 변경
    /// </summary>
    public IEnumerator ChangeSpriteWithFadeToBlack(Sprite sprite, float transitionDuration = defaultDuration)
    {
        if (!isTransitioning && sprite != null) {
            isTransitioning = true;
            isAnimationEnd = false;
            float elapsedTime = 0f;
            Color startColor = currentSprite.color;
            Color endColor = new Color(0f, 0f, 0f, 1f);

            while (!isAnimationEnd && elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.color = endColor;
            currentSprite.sprite = sprite;

            elapsedTime = 0f;
            while (!isAnimationEnd && elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.color = Color.Lerp(endColor, startColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.color = startColor;
            isTransitioning = false;
        }
    }

    /// <summary>
    /// 스프라이트를 새로운 위치로 이동시키고 확대
    /// </summary>
    public IEnumerator MoveAndZoomSprite(Vector3 movePosition, Vector3 zoomScale, float transitionDuration = defaultDuration)
    {
        if (!isTransitioning) {
            isTransitioning = true;
            isAnimationEnd = false;
            Vector3 startPosition = currentSprite.transform.position;
            Vector3 startScale = currentSprite.transform.localScale;

            float elapsedTime = 0f;

            while (!isAnimationEnd && elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;

                // Interpolate position
                Vector3 newPosition = Vector3.Lerp(startPosition, movePosition, normalizedTime);
                currentSprite.transform.position = newPosition;

                // Interpolate scale
                Vector3 newScale = Vector3.Lerp(startScale, zoomScale, normalizedTime);
                currentSprite.transform.localScale = newScale;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.transform.position = movePosition;
            currentSprite.transform.localScale = zoomScale;

            isTransitioning = false;
        }
    }

    /// <summary>
    /// 스프라이트를 변경하면서 페이드 인
    /// </summary>
    private IEnumerator FadeInChangeSprite(float transitionDuration = defaultDuration)
    {
        float elapsedTime = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = Color.white;

        while (!isAnimationEnd && elapsedTime < transitionDuration) {
            float normalizedTime = elapsedTime / transitionDuration;
            changeSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        changeSprite.color = endColor;
    }

    /// <summary>
    /// 트랜스폼 속성(위치와 스케일)을 선형 보간(lerping)
    /// </summary>
    private IEnumerator LerpTransform(Vector3 startValue, Vector3 endValue, float duration, System.Action<Vector3> updateAction)
    {
        float elapsedTime = 0f;

        while (!isAnimationEnd && elapsedTime < duration) {
            float normalizedTime = elapsedTime / duration;
            updateAction(Vector3.Lerp(startValue, endValue, normalizedTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        updateAction(endValue);
    }

    /// <summary>
    /// 스프라이트 효과 종료
    /// </summary>
    private void EndSpriteEffect()
    {
        isAnimationEnd = true;
        isTransitioning = false;
    }
}
