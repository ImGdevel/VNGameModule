using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameDataManager gameData;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Update() {

        if (Input.GetKeyDown(KeyCode.S)) {
            GameDataManager.Instance.SaveGameData(2);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            GameDataManager.Instance.LoadGameData(2);
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            GameDataManager.Instance.DeleteData(2);
        }
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
