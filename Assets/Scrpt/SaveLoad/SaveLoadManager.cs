using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveLoadManager
{
    private string fileExtension = ".sav"; // Default file extension

    // Constructor to set the file extension
    public SaveLoadManager(string extension = ".sav") {
        fileExtension = extension;
    }

    // Save game data to a file
    public void SaveGameData(GameData data, string saveFileName) {
        string json = JsonUtility.ToJson(data);
        string savePath = GetSavePath(saveFileName);

        // Write JSON data to a .sav file
        File.WriteAllText(savePath, json);
    }

    // Load game data from a file
    public GameData LoadGameData(string saveFileName) {
        string savePath = GetSavePath(saveFileName);

        if (File.Exists(savePath)) {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        else {
            Debug.LogWarning("Could not find saved data: " + saveFileName);
            return null;
        }
    }

    // Create an empty save file
    public void CreateEmptySaveFile(string saveFileName) {
        GameData emptyData = new GameData();
        emptyData.uid = "default";

        string json = JsonUtility.ToJson(emptyData);
        string savePath = GetSavePath(saveFileName);

        // Write empty JSON data to a .sav file
        File.WriteAllText(savePath, json);
    }

    // Get the full path to the save file
    private string GetSavePath(string saveFileName) {
        string saveDirectory = Application.persistentDataPath;
        string savePath = Path.Combine(saveDirectory, saveFileName + fileExtension);
        return savePath;
    }
}

[System.Serializable]
public class GameData
{
    public string uid;
    public List<SaveData> saveDatas = new List<SaveData>();
}

[System.Serializable]
public class SaveData
{
    public string playerName;
    public string chapter;
    public string dialogId;
}
