using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


internal class FileManager<T>
{
    private string rootFolder; // 폴더 이름

    public FileManager(string rootFolder = "Temp")
    {
        this.rootFolder = rootFolder;

        // Save 폴더가 없으면 생성
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }
    }

    // Save game data to a file
    public void SaveDataToLocal(string saveFileName, T data, bool isEncryption = false)
    {
        try {
            string savefile = JsonUtility.ToJson(data);
            string savepath = GetFilePath(saveFileName);

            if (isEncryption) {
                savefile = EncryptionManager.Encrypt(savefile);
            }

            File.WriteAllText(savepath, savefile);
            Debug.Log($"Data saved to: {savepath}");
        }
        catch (Exception ex) {
            Debug.LogError($"Save failed for {saveFileName}: {ex.Message}");
        }
    }

    // Load game data from a file
    public T LoadDataToLocal(string saveFileName, bool isEncryption = false)
    {
        try {
            string savepath = GetFilePath(saveFileName);

            if (!File.Exists(savepath)) {
                Debug.LogError($"File not found: {savepath}");
                return default;
            }

            Debug.Log($"Loading data from: {savepath}");

            string savefile = File.ReadAllText(savepath);

            if (isEncryption) {
                savefile = EncryptionManager.Decrypt(savefile);
            }

            T data = JsonUtility.FromJson<T>(savefile);
            return data;
        }
        catch (Exception ex) {
            Debug.LogError($"Load failed for {saveFileName}: {ex.Message}");
            return default;
        }
    }


    public T[] LoadAllFilesWithExtension(string extension = null, bool isEncryption = false)
    {
        try {
            string folderPath = GetFolderPath();

            string[] files;
            if (extension == null) {
                files = Directory.GetFiles(folderPath, $"*.*");
            }
            else {
                files = Directory.GetFiles(folderPath, $"*.{extension}");
            }

            List<T> loadedDataList = new List<T>();

            foreach (string filePath in files) {

                string fileContent = File.ReadAllText(filePath);

                Debug.Log(filePath);

                if (isEncryption) {
                    fileContent = EncryptionManager.Decrypt(fileContent);
                }

                T data = JsonUtility.FromJson<T>(fileContent);
                loadedDataList.Add(data);

            }

            return loadedDataList.ToArray();
        }
        catch (Exception ex) {
            Debug.LogError($"Load all files failed: {ex.Message}");
            return null;
        }
    }

    private string GetFolderPath()
    {
        return Path.Combine(Application.persistentDataPath, rootFolder);
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(GetFolderPath(), fileName);
    }
}
