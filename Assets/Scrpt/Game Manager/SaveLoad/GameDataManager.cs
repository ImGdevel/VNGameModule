using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private FileManager<SaveData> saveFileManager;
    private FileManager<SaveDataConfig> saveDataConfigFileManager;
    private List<SaveData> saveDatas;

    private GameData gameData;
    const int maxSaveSlots = 20;

    public List<SaveData> SaveDatas { get { return saveDatas; } }
    public GameData GameData { get { return gameData; } }

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
        saveDatas = new List<SaveData>();
        saveFileManager = new FileManager<SaveData>("Save", "save");
        saveDataConfigFileManager = new FileManager<SaveDataConfig>("Save", "json");

        LoadAllData();
    }

    public GameData NewGameData = new GameData();

    public void CreateNewGameData()
    {
        GameData GameData = new GameData();
        


    }

    public void SaveData(int saveFileNumber = 0)
    {
        //saveNumber에 대한 Data저장 후 저장        

        OnGameDataSaved?.Invoke(gameData);




    }

    public void LoadData(int saveFileNumber = 0)
    {
        

        OnGameDataLoaded?.Invoke(gameData);
    }

    public void DeleteData(int saveFileNumber = 0)
    {
            
    }


    public void LoadAllData()
    {
        SaveDataConfig saveConfig = LoadSaveDataConfig();
        List<SaveConfig> SaveConfigs = saveConfig.saveConfigs;
        saveDatas.Clear();

        foreach (SaveConfig save in SaveConfigs)
        {
            SaveData saveData = saveFileManager.OpenFileToJson(save.saveFileName);
            saveDatas.Add(saveData);
        }
    }

    private void UpdateSaveConfig()
    {
        // todo: 현재 saveDatas에 있는 Savefile목록을 saveconfig에 저장?
        // 그보다 저장하려는 파일 목록만 업데이트
    }

    private SaveDataConfig LoadSaveDataConfig()
    {
        if (!saveDataConfigFileManager.IsFileExist("save_config")) {
            SaveDataConfig defalutSaveConfig = new SaveDataConfig();
            saveDataConfigFileManager.WriteFileToJson("save_config", defalutSaveConfig);
        }
        return saveDataConfigFileManager.OpenFileToJson("save_config");
    }

}
