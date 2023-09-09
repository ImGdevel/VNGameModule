using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VNSceneManager: MonoBehaviour
{
    public static VNSceneManager Instance { get; private set; }

    private void Awake() {
        if(Instance == null) {
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        // 씬이 로드될 때마다 OnSceneLoaded 메서드를 호출
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // 모든 씬이 시작될 때 호출될 메서드
        Debug.LogWarning("씬 호출됨");
    }

    private void OnDestroy() {
        // 이 스크립트가 파괴될 때 이벤트 핸들러 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
