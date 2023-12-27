using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveLoadManager saveLoadManager;
    public static UserData userData;

    public SaveData quickSaveSlot;

    //public static event Action<SaveData> SaveUserData;
    //public static event Action<SaveData> LoadUserData;

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

        userData = saveLoadManager.LoadUserData("userdata");

        if (userData == null) {
            userData = CreateDefaultUserData();
            saveLoadManager.SaveUserData(userData, "userdata");
        }

        PrintUserData();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            OnSaveUserData();
        }
    }

    private UserData CreateDefaultUserData() {
        UserData defaultData = new UserData();
        defaultData.uid = "";

        return defaultData;
    }

    public void CreateNewSaveSlot() {
        SaveData newSaveData = new SaveData();
        newSaveData.playerName = "";
        newSaveData.chapter = "";
        newSaveData.dialogId = "";
        userData.saveDatas.Add(newSaveData);
    }


    private void PrintUserData() {
        /*
        Debug.Log("Loaded UID: " + userData.uid);
        foreach (SaveData save in userData.saveDatas) {
            Debug.Log("Player Name: " + save.playerName);
            Debug.Log("Chapter: " + save.chapter);
            Debug.Log("Dialog ID: " + save.dialogId);
        }
        */
    }

    public void OnSaveUserData() {
        //SaveUserData?.Invoke(userData.saveDatas[0]);
        PrintUserData();
        saveLoadManager.SaveUserData(userData, "userdata");
    }

    public void OnLoadUserData() {
        userData = saveLoadManager.LoadUserData("userdata");
        //LoadUserData?.Invoke(userData.saveDatas[0]);
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
