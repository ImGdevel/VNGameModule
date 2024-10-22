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
        // ���� �ε�� ������ OnSceneLoaded �޼��带 ȣ��
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // ��� ���� ���۵� �� ȣ��� �޼���
        Debug.LogWarning("�� ȣ���");
    }

    private void OnDestroy() {
        // �� ��ũ��Ʈ�� �ı��� �� �̺�Ʈ �ڵ鷯 ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
