using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCameraComponent : MonoBehaviour
{
    private Transform shakeTransform;
    private Vector3 initialPosition;

    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;

    private void Awake() {
        shakeTransform = transform;
    }

    private void OnEnable() {
        initialPosition = shakeTransform.localPosition;
    }

    private void Update() {
        if (isShaking) {
            shakeTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
        }
    }

    // �ڷ�ƾ�� ����Ͽ� ��鸲�� �����ϴ� �Լ�
    public void StartShake(float duration, float magnitude) {
        if (!isShaking) {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
            StartCoroutine(Shake());
        }
    }

    // �ڷ�ƾ���� ��鸲 ȿ�� ����
    private IEnumerator Shake() {
        isShaking = true;

        float elapsed = 0f;

        while (elapsed < shakeDuration) {
            elapsed += Time.deltaTime;

            // ��鸲 ����
            float percentComplete = elapsed / shakeDuration;
            float dampingFactor = 1 - Mathf.Clamp01(percentComplete);
            shakeTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude * dampingFactor;

            yield return null;
        }

        isShaking = false;
        shakeTransform.localPosition = initialPosition;
    }
}
