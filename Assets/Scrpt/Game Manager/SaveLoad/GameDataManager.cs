using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class GameSaveData
{
    public int uid;
    public int saveSlotCount;
    public GameData[] gameDatas;
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private FileManager<GameData> saveFileManager;
    
    private GameData gameData;
    public GameData GameData {  get { return gameData; } }

    private GameSaveData saveData;
    private int courrnetSlot = 0;

    public event Action<GameData> OnGameDataSaved;
    public event Action<GameData> OnGameDataLoaded;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameData = new GameData();
        saveFileManager = new FileManager<GameData>("Save");

        LoadAllData();

    }

    public void SaveData()
    {
        string saveFileName = "save01.save";

        OnGameDataSaved?.Invoke(gameData);

        if (saveFileManager != null) {
            saveFileManager.SaveDataToLocal(saveFileName, saveData.gameDatas[courrnetSlot]);
        }
    }

    public void LoadData()
    {
        string saveFileName = "save01.save";

        if (saveFileManager != null) {
            saveData.gameDatas[courrnetSlot] = saveFileManager.LoadDataToLocal(saveFileName);
        }

        OnGameDataLoaded?.Invoke(gameData);
    }

    public void LoadAllData()
    {
        if (saveFileManager != null) {
            saveData.gameDatas = saveFileManager.LoadAllFilesWithExtension("save");
        }
    }

}
