using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void SaveData(int saveFileNumber = 0)
    {
        //saveNumber에 대한 Data저장 후 저장
        OnGameDataSaved?.Invoke(gameData);

        // 세이브 파일이 없는 경우 새로운 세이브 파일 생성
        if (this.saveData == null) {
            saveData = new SaveData(saveFileNumber, gameData);
        }
        saveData.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        saveFileManager.WriteFileToJson(saveData.saveName, saveData);
    }

    public void LoadData(int saveFileNumber = 0)
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

    private void UpdateSaveConfig()
    {
        // todo: 현재 saveDatas에 있는 Savefile목록을 saveconfig에 저장?
        // 그보다 저장하려는 파일 목록만 업데이트
    }

    private SaveDataConfig LoadSaveDataConfig()
    {
        if (!saveDataConfigFileManager.IsFileExist("save_config")) {
            SaveDataConfig defalutSaveConfig = new SaveDataConfig();
            defalutSaveConfig.maxSaveSlots = maxSaveSlots;
            defalutSaveConfig.uid = EncryptionManager.EncryptHash(SystemInfo.deviceUniqueIdentifier);
            defalutSaveConfig.gameId = Application.productName;
            saveDataConfigFileManager.WriteFileToJson("save_config", defalutSaveConfig);
        }
        return saveDataConfigFileManager.OpenFileToJson("save_config");
    }

}
