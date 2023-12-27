using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;

public class SaveFileManager
{
    private string fileExtension = ".save";
    private string rootPath = Application.persistentDataPath;

    public SaveFileManager() { }

    // Save game data to a file
    public void SaveDataToLocal(string saveFilePath, GameData data, bool isEncryption = false)
    {
        try {
            string savefile = JsonUtility.ToJson(data);
            string savepath = GetFilePath(saveFilePath);

            if (isEncryption) {
                savefile = EncryptionManager.Encrypt(savefile);
            }

            File.WriteAllText(savepath, savefile);
        }
        catch (Exception ex) {
            Debug.LogError("Save failed: " + ex.Message);
        }
    }

    // Load game data from a file
    public GameData LoadDataToLocal(string saveFilePath, bool isEncryption = false)
    {
        try {
            string savepath = GetFilePath(saveFilePath);

            //if (!File.Exists(savepath)) {
            //    Debug.LogWarning("저장된 데이터를 찾을 수 없습니다: " + saveFilePath);
            //   return null;
            //}

            string savefile = File.ReadAllText(savepath);

            if (isEncryption) {
                savefile = EncryptionManager.Decrypt(savefile);
            }

            GameData data = JsonUtility.FromJson<GameData>(savefile);
            return data;
        }
        catch (Exception ex) {
            Debug.LogError("Load failed: " + ex.Message);
            return null;
        }
    }

    // Create an empty save file
    public void CreateEmptySaveFile(string saveFileName)
    {
        try {
            GameData emptyData = new GameData();
            emptyData.uid = "default";

            string json = JsonUtility.ToJson(emptyData);
            string savePath = GetFilePath(saveFileName);

            // Write empty JSON data to a .sav file
            File.WriteAllText(savePath, json);
        }
        catch (Exception ex) {
            Debug.LogError("Save creation failed: " + ex.Message);
        }
    }

    private string GetFilePath(params string[] paths)
    {
        return Path.Combine(rootPath, Path.Combine(paths)) + fileExtension;
    }
}
