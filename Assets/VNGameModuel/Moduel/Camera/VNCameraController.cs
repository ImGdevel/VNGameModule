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

    // �ڷ�ƾ�� ����Ͽ� ��鸲�� �����ϴ� �Լ�
    public void StartShake(float duration, float magnitude)
    {
        if (!isShaking) {
            //shakeDuration = duration;
            //shakeMagnitude = magnitude;
            StartCoroutine(Shake());
        }
    }

    // �ڷ�ƾ���� ��鸲 ȿ�� ����
    private IEnumerator Shake()
    {
        isShaking = true;
        Debug.Log("��� ���");
        float elapsed = 0f;
        float startTime = Time.time;

        while (Time.time - startTime < shakeDuration) {
            elapsed += Time.time - startTime;

            // ��鸲 ����
            float percentComplete = elapsed / shakeDuration;
            float dampingFactor = 1 - Mathf.Clamp01(percentComplete);
            shakeTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude * dampingFactor;
            yield return new WaitForSeconds(shakeDelay);
        }

        isShaking = false;
        shakeTransform.localPosition = initialPosition;
    }
}
