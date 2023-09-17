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
    public void SaveUserData(UserData data, string saveFileName) {
        string json = JsonUtility.ToJson(data);
        string savePath = GetSavePath(saveFileName);

        // Write JSON data to a .sav file
        File.WriteAllText(savePath, json);
    }

    // Load game data from a file
    public UserData LoadUserData(string saveFileName) {
        string savePath = GetSavePath(saveFileName);

        if (File.Exists(savePath)) {
            string json = File.ReadAllText(savePath);
            UserData data = JsonUtility.FromJson<UserData>(json);
            return data;
        }
        else {
            Debug.LogWarning("Could not find saved data: " + saveFileName);
            return null;
        }
    }

    // Create an empty save file
    public void CreateEmptySaveFile(string saveFileName) {
        UserData emptyData = new UserData();
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
