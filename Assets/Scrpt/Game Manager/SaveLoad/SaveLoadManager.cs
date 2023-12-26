using UnityEngine;
using System.IO;

public class SaveLoadManager
{
    private string fileExtension = ".save";

    // Constructor to set the file extension
    public SaveLoadManager(string extension = ".save") {
        fileExtension = extension;
    }

    // Save game data to a file
    public void SaveUserData(UserData data, string saveFileName) {
        string json = JsonUtility.ToJson(data);
        string savePath = GetSavePath(saveFileName);
        Debug.Log("SaveFile: " + savePath);

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
    protected string GetSavePath(string saveFileName) {
        return GetFilePath(saveFileName + fileExtension);
    }

    private string GetFilePath(params string[] paths) {
        string directoryPath = GetRootPath();

        foreach (string path in paths) {
            directoryPath = Path.Combine(directoryPath ,path);
        }
        return directoryPath;
    }

    private string GetRootPath() {
        return Application.persistentDataPath;
    }
}
