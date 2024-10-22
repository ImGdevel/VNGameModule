using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCameraController : MonoBehaviour
{
    private Transform shakeTransform;
    private Vector3 initialPosition;

    private bool isShaking = false;
    private float shakeDuration = 1f;
    private float shakeMagnitude = 0.2f;
    private float shakeDelay = 0.5f;

    private void Awake()
    {
        shakeTransform = transform;
    }

    private void OnEnable()
    {
        initialPosition = shakeTransform.localPosition;
    }

    private void Update()
    {
        if (isShaking) {
            shakeTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
        }
    }

    // 코루틴을 사용하여 흔들림을 시작하는 함수
    public void StartShake(float duration, float magnitude)
    {
        if (!isShaking) {
            //shakeDuration = duration;
            //shakeMagnitude = magnitude;
            StartCoroutine(Shake());
        }
    }

    // 코루틴으로 흔들림 효과 구현
    private IEnumerator Shake()
    {
        isShaking = true;
        Debug.Log("흔들 흔들");
        float elapsed = 0f;
        float startTime = Time.time;

        while (Time.time - startTime < shakeDuration) {
            elapsed += Time.time - startTime;

            // 흔들림 감소
            float percentComplete = elapsed / shakeDuration;
            float dampingFactor = 1 - Mathf.Clamp01(percentComplete);
            shakeTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude * dampingFactor;
            yield return new WaitForSeconds(shakeDelay);
        }

        isShaking = false;
        shakeTransform.localPosition = initialPosition;
    }
}
