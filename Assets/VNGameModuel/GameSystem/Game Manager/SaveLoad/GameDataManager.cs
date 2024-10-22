using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 데이터 관리자
/// </summary>
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private FileManager<SaveData> saveFileManager;
    private FileManager<SaveDataConfig> saveDataConfigFileManager;
    private List<SaveData> saveDatas;

    private SaveData saveData;
    private GameData gameData;


    [SerializeField]
    private int maxSaveSlots = 1;

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
        saveFileManager = new FileManager<SaveData>("Save", "json");
        saveDataConfigFileManager = new FileManager<SaveDataConfig>("Save", "json");

        LoadAllData();
    }

    public GameData defaultNewGameData = new GameData();

    public void CreateNewGameData(GameData newData = default)
    {
        gameData = (newData == default) ? defaultNewGameData : newData;
    }

    public void SaveGameData(int saveFileNumber = 0)
    {
        OnGameDataSaved?.Invoke(gameData);

        if (this.saveData == null) {
            saveData = new SaveData(saveFileNumber, gameData);
        }
        saveData.saveLastTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        UpdateSaveConfig(saveData);
        saveFileManager.WriteFileToJson(saveData.saveName, saveData);
    }

    public void LoadGameData(int saveFileNumber = 0)
    {
        LoadAllData();

        foreach (var save in saveDatas)
        {
            if(save.saveNumber == saveFileNumber) {
                saveData = save;
                gameData = saveData.gameData;
            }
        }

        OnGameDataLoaded?.Invoke(gameData);
    }

    public void DeleteData(int saveFileNumber = 0)
    {
        foreach (var save in saveDatas) {
            if (save.saveNumber == saveFileNumber) {
                UpdateDeleteSaveConfig(save);
                saveFileManager.DeleteFile(save.saveName);
                break;
            }
        }
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

    public List<SaveData> GetSaveDataList()
    {
        return saveDatas;
    }

    private void UpdateSaveConfig(SaveData saveData)
    {
        SaveDataConfig loadSaveConfig = saveDataConfigFileManager.OpenFileToJson("save_config");
        int matchingIndex = loadSaveConfig.saveConfigs.FindIndex(save => save.saveNumber == saveData.saveNumber);

        if (matchingIndex == -1) {
            loadSaveConfig.AddSaveConfig(saveData.saveName, saveData.saveNumber);
        }
        saveDataConfigFileManager.WriteFileToJson("save_config", loadSaveConfig);
    }

    private void UpdateDeleteSaveConfig(SaveData saveData)
    {
        SaveDataConfig loadSaveConfig = saveDataConfigFileManager.OpenFileToJson("save_config");
        int matchingIndex = loadSaveConfig.saveConfigs.FindIndex(save => save.saveNumber == saveData.saveNumber);

        if (matchingIndex != -1) {
            loadSaveConfig.RemoveSaveConfig(saveData.saveName, saveData.saveNumber);
        }
        saveDataConfigFileManager.WriteFileToJson("save_config", loadSaveConfig);
    }

    private SaveDataConfig LoadSaveDataConfig()
    {
        if (!saveDataConfigFileManager.IsFileExist("save_config")) {
            SaveDataConfig defalutSaveConfig = CreateSaveDataConfig();
            saveDataConfigFileManager.WriteFileToJson("save_config", defalutSaveConfig);
        }
        return saveDataConfigFileManager.OpenFileToJson("save_config");
    }

    private SaveDataConfig CreateSaveDataConfig()
    {
        SaveDataConfig defalutSaveConfig = new SaveDataConfig();
        defalutSaveConfig.maxSaveSlots = maxSaveSlots;
        defalutSaveConfig.uid = EncryptionManager.EncryptHash(SystemInfo.deviceUniqueIdentifier);
        defalutSaveConfig.gameId = Application.productName;
        return defalutSaveConfig;
    }

}
