using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCameraController : MonoBehaviour
{
    private Transform shakeTransform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1.0f;

    private Vector3 initialPosition;

    void Awake() {
        shakeTransform = transform; // 현재 스크립트가 연결된 GameObject의 Transform을 가져옴
    }

    void OnEnable() {
        initialPosition = shakeTransform.localPosition;
    }

    void Update() {
        if (shakeDuration > 0) {
            shakeTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else {
            shakeDuration = 0f;
            shakeTransform.localPosition = initialPosition;
        }
    }

    // 흔들림을 시작하는 함수
    public void StartShake(float duration, float magnitude) {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

}
