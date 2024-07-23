using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNGameModuel
{
    public class SpriteController : MonoBehaviour
    {
        private SpriteRenderer currentSprite;
        private SpriteRenderer changeSprite;

        private const float defaultDuration = 0.1f;

        private bool isTransitioning = false;
        private bool isAnimaionEnd = false;

        private void Awake()
        {
            InitializeSprites();
        }

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
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// 이미지 즉시 변경
        /// </summary>
        /// <param name="sprite">변경할 Sprite</param>
        public void SetSprite(Sprite sprite)
        {
            currentSprite.sprite = sprite;
        }

        /// <summary>
        /// 위치 지정
        /// </summary>
        /// <param name="posision"></param>
        public void SetSpritePosision(Vector2 posision)
        {
            currentSprite.transform.position = posision;
            Debug.Log(currentSprite.transform.position);
        }

        /// <summary>
        /// 스트라이트 효과 종료
        /// </summary>
        public void EndSpriteEffect()
        {
            isAnimaionEnd = true;
            isTransitioning = false;
        }

        /// <summary>
        /// FadeIn방식으로 Sprite 교체
        /// </summary>
        /// <param name="sprite">변경할 Sprite</param>
        /// <param name="transitionDuration">변환시간</param>
        public IEnumerator FadeInSprite(Sprite sprite, float transitionDuration = defaultDuration)
        {
            if (!isTransitioning) {
                isTransitioning = true;
                isAnimaionEnd = false;
                float elapsedTime = 0f;
                Color startColor = new Color(1f, 1f, 1f, 0f);
                Color endColor = Color.white;

                currentSprite.sprite = sprite;

                while (!isAnimaionEnd && elapsedTime < transitionDuration) {
                    float normalizedTime = elapsedTime / transitionDuration;
                    currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                isTransitioning = false;
            }
        }

        /// <summary>
        /// FadeOut 방식으로 Sprite 교체
        /// </summary>
        /// <param name="transitionDuration">변환시간</param>
        public IEnumerator FadeOutSprite(float transitionDuration = defaultDuration)
        {
            isTransitioning = true;
            isAnimaionEnd = false;
            float elapsedTime = 0f;
            Color startColor = Color.white;
            Color endColor = new Color(1f, 1f, 1f, 0f);

            while (!isAnimaionEnd && elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentSprite.sprite = null;
            isTransitioning = false;
        }

        /// <summary>
        /// 이미지를 다른 이미지로 교차 형식으로 전환
        /// </summary>
        /// <param name="sprite">변경할 Sprite</param>
        /// <param name="transitionDuration">변환시간</param>
        public IEnumerator ChangeSpriteCrossFade(Sprite sprite, float transitionDuration = defaultDuration)
        {
            if (!isTransitioning) {
                float elapsedTime = 0f;
                isTransitioning = true;
                isAnimaionEnd = false;

                changeSprite.enabled = true;
                changeSprite.sprite = sprite;

                StartCoroutine(FadeInChangeSprite(transitionDuration));
                StartCoroutine(FadeOutSprite(transitionDuration));

                while (!isAnimaionEnd && elapsedTime < transitionDuration) {
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
        /// 이미지를 검은색으로 전환했다 다른 이미지로 교차 형식으로 전환
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="transitionDuration"></param>
        /// <returns></returns>
        public IEnumerator ChangeSpriteWithFadeToBlack(Sprite sprite, float transitionDuration = defaultDuration)
        {
            if (!isTransitioning) {
                isTransitioning = true;
                isAnimaionEnd = false;
                float elapsedTime = 0f;
                Color startColor = currentSprite.color;
                Color endColor = new Color(0f, 0f, 0f, 1f);

                while (!isAnimaionEnd && elapsedTime < transitionDuration) {
                    float normalizedTime = elapsedTime / transitionDuration;
                    currentSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                currentSprite.color = endColor;
                currentSprite.sprite = sprite;

                elapsedTime = 0f;
                while (!isAnimaionEnd && elapsedTime < transitionDuration) {
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
        /// 스프라이트의 위치를 이동시킵니다.
        /// </summary>
        /// <param name="movePosition">이동 위치</param>
        /// <param name="transitionDuration">이동 시간</param>
        /// <returns></returns>
        public IEnumerator MoveSpritePosition(Vector3 movePosition, float transitionDuration = defaultDuration)
        {
            if (!isTransitioning) {
                isTransitioning = true;
                isAnimaionEnd = false;
                Vector3 startPosition = currentSprite.transform.position;

                yield return LerpTransform(startPosition, movePosition, transitionDuration, newPosition => {
                    currentSprite.transform.position = newPosition;
                });

                isTransitioning = false;

                currentSprite.transform.position = movePosition;

                Debug.Log(currentSprite.transform.position + "/" + movePosition);
            }
        }


        // Coroutine for zooming the sprite
        public IEnumerator ZoomSprite(Vector3 zoomScale, float transitionDuration = defaultDuration)
        {
            if (!isTransitioning) {
                isTransitioning = true;
                isAnimaionEnd = false;
                Vector3 startScale = currentSprite.transform.localScale;

                yield return LerpTransform(startScale, zoomScale, transitionDuration, newScale => {
                    currentSprite.transform.localScale = newScale;
                });

                isTransitioning = false;

            }
        }

        // Coroutine for moving the sprite to a new position and zooming it
        public IEnumerator MoveAndZoomSprite(Vector3 movePosition, Vector3 zoomScale, float transitionDuration = defaultDuration)
        {
            if (!isTransitioning) {
                isTransitioning = true;
                isAnimaionEnd = false;
                Vector3 startPosition = currentSprite.transform.position;
                Vector3 startScale = currentSprite.transform.localScale;

                float elapsedTime = 0f;

                while (!isAnimaionEnd && elapsedTime < transitionDuration) {
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

                // Ensure final position and scale are set
                currentSprite.transform.position = movePosition;
                currentSprite.transform.localScale = zoomScale;

                isTransitioning = false;

            }
        }

        // Coroutine for fading in the change sprite
        private IEnumerator FadeInChangeSprite(float transitionDuration = defaultDuration)
        {
            float elapsedTime = 0f;
            Color startColor = new Color(1f, 1f, 1f, 0f);
            Color endColor = Color.white;

            while (!isAnimaionEnd && elapsedTime < transitionDuration) {
                float normalizedTime = elapsedTime / transitionDuration;
                changeSprite.color = Color.Lerp(startColor, endColor, normalizedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            changeSprite.color = endColor;
        }

        // Coroutine for lerping transform properties (position and scale)
        private IEnumerator LerpTransform(Vector3 startValue, Vector3 endValue, float duration, System.Action<Vector3> updateAction)
        {
            float elapsedTime = 0f;

            while (!isAnimaionEnd && elapsedTime < duration) {
                float normalizedTime = elapsedTime / duration;
                updateAction(Vector3.Lerp(startValue, endValue, normalizedTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            updateAction(endValue);
        }


    }

}

