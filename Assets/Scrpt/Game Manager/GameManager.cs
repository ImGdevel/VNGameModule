using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveLoadManager saveLoadManager;
    public static UserData userData;

    public SaveData quickSaveSlot;

    public static event Action<SaveData> SaveUserData;
    public static event Action<SaveData> LoadUserData;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        saveLoadManager = new SaveLoadManager();

        // 게임 데이터 불러오기 (저장된 데이터가 없을 경우 기본 데이터 생성)
        userData = saveLoadManager.LoadUserData("userdata");

        if (userData == null) {
            userData = CreateDefaultUserData();
            saveLoadManager.SaveUserData(userData, "userdata");
        }

        PrintUserData();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            Debug.Log("저장");
            OnSaveUserData();
        }
    }

    private UserData CreateDefaultUserData() {
        UserData defaultData = new UserData();
        defaultData.uid = "";

        return defaultData;
    }

    private void CreateNewSaveSlot() {
        SaveData newSaveData = new SaveData();
        newSaveData.playerName = "";
        newSaveData.chapter = "";
        newSaveData.dialogId = "";
        userData.saveDatas.Add(newSaveData);
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
        SaveUserData?.Invoke(userData.saveDatas[0]);
        PrintUserData();
        saveLoadManager.SaveUserData(userData, "userdata");
    }

    // 로드 버튼 클릭 시 호출할 메서드 (예시)
    public void OnLoadUserData() {
        userData = saveLoadManager.LoadUserData("userdata");
        LoadUserData?.Invoke(userData.saveDatas[0]);
    }

    //
    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
