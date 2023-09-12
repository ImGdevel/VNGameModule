using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveLoadManager saveLoadManager;
    public static GameData gameData;

    public event Action<GameData> SaveGame;
    public event Action<GameData> LoadGame;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        // SaveLoadManager 인스턴스 생성
        saveLoadManager = new SaveLoadManager();

        // 게임 데이터 불러오기 (저장된 데이터가 없을 경우 기본 데이터 생성)
        gameData = saveLoadManager.LoadGameData("PlayerSave");

        if (gameData == null) {
            gameData = CreateDefaultGameData();
            saveLoadManager.SaveGameData(gameData, "PlayerSave");
        }

        // 게임 데이터 출력 (예시로 출력, 실제 게임에서는 필요에 따라 사용)
        Debug.Log("Loaded UID: " + gameData.uid);
        foreach (SaveData save in gameData.saveDatas) {
            Debug.Log("Player Name: " + save.playerName);
            Debug.Log("Chapter: " + save.chapter);
            Debug.Log("Dialog ID: " + save.dialogId);
        }
    }

    private GameData CreateDefaultGameData() {
        GameData defaultData = new GameData();
        defaultData.uid = "default";

        // 예시로 두 개의 저장 데이터를 생성
        SaveData save1 = new SaveData();
        save1.playerName = "Player1";
        save1.chapter = "Chapter1";
        save1.dialogId = "Dialog1";

        SaveData save2 = new SaveData();
        save2.playerName = "Player2";
        save2.chapter = "Chapter2";
        save2.dialogId = "Dialog2";

        defaultData.saveDatas.Add(save1);
        defaultData.saveDatas.Add(save2);

        return defaultData;
    }

    // 저장 버튼 클릭 시 호출할 메서드 (예시)
    public void OnSaveGameData() {
        SaveGame.Invoke(gameData);
        saveLoadManager.SaveGameData(gameData, "PlayerSave");
    }

    // 로드 버튼 클릭 시 호출할 메서드 (예시)
    public void OnLoadGameData() {
        gameData = saveLoadManager.LoadGameData("PlayerSave");
        LoadGame?.Invoke(gameData);
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
