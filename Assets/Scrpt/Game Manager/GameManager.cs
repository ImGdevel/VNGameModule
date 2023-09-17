using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveLoadManager saveLoadManager;
    public static UserData userData;

    public event Action<UserData> SaveGame;
    public event Action<UserData> LoadGame;

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
        userData = saveLoadManager.LoadUserData("PlayerSave");

        if (userData == null) {
            userData = CreateDefaultUserData();
            saveLoadManager.SaveUserData(userData, "PlayerSave");
        }

        PrintUserData();
    }

    private UserData CreateDefaultUserData() {
        UserData defaultData = new UserData();
        defaultData.uid = "";

        // 예시로 두 개의 저장 데이터를 생성
        SaveData save1 = new SaveData();
        save1.playerName = "";
        save1.chapter = "";
        save1.dialogId = "";

        defaultData.saveDatas.Add(save1);

        return defaultData;
    }

    private void PrintUserData() {
        // 게임 데이터 출력 (예시로 출력, 실제 게임에서는 필요에 따라 사용)
        Debug.Log("Loaded UID: " + userData.uid);
        foreach (SaveData save in userData.saveDatas) {
            Debug.Log("Player Name: " + save.playerName);
            Debug.Log("Chapter: " + save.chapter);
            Debug.Log("Dialog ID: " + save.dialogId);
        }
    }

    // 저장 버튼 클릭 시 호출할 메서드 (예시)
    public void OnSaveUserData() {
        SaveGame?.Invoke(userData);
        saveLoadManager.SaveUserData(userData, "PlayerSave");
    }

    // 로드 버튼 클릭 시 호출할 메서드 (예시)
    public void OnLoadUserData() {
        userData = saveLoadManager.LoadUserData("PlayerSave");
        LoadGame?.Invoke(userData);
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
