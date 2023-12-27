using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private SaveFileManager saveLoadManager;
    private GameData gameData;

    // GameDataManager를 사용하여 데이터 관리 
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        gameData = new GameData();
        saveLoadManager = new SaveFileManager();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveData()
    {
        string saveFileName = "Save/save01";
        // 데이터 저장 

        // 현제 게임 데이터 업데이트

        if (saveLoadManager != null) {
            saveLoadManager.SaveDataToLocal(saveFileName, gameData);
        }

    }

    public void LoadData()
    {
        string saveFileName = "Save/save01";
        // 데이터 불러오기 시도

        // 불러온 데이터 게임에 적용

        if (saveLoadManager != null) {
            gameData = saveLoadManager.LoadDataToLocal(saveFileName);
        }
    }

    public void LoadSaveConfig()
    {

    }

    public void UpdateSaveConfig()
    {

    }
}
