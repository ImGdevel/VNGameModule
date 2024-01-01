using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Pipeline;
using UnityEngine;


internal class FileManager<T>
{
    private string rootFolder; // 폴더 이름
    private string fileExtension;

    public FileManager(string rootFolder = "Temp", string fileExtension = null)
    {
        this.fileExtension = (fileExtension != null) ? "." + fileExtension : "";
        this.rootFolder = rootFolder;
        // Save 폴더가 없으면 생성
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }
    }

    // File Save
    public void WriteFileToJson(string saveFileName, T data, bool isEncryption = false)
    {
        try {
            string savefile = JsonUtility.ToJson(data);
            string savepath = GetFilePath(saveFileName);

            if (isEncryption) {
                savefile = EncryptionManager.EncryptAES(savefile);
            }

            File.WriteAllText(savepath, savefile);
            Debug.Log($"Data saved to: {savepath}");
        }
        catch (Exception ex) {
            Debug.LogError($"Save failed for {saveFileName}: {ex.Message}");
        }
    }

    // File Load
    public T OpenFileToJson(string saveFileName, bool isEncryption = false)
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
                savefile = EncryptionManager.DecryptAES(savefile);
            }

            T data = JsonUtility.FromJson<T>(savefile);
            return data;
        }
        catch (Exception ex) {
            Debug.LogError($"Load failed for {saveFileName}: {ex.Message}");
            return default;
        }
    }

    public void DeleteFile(string saveFileName)
    {
        try {
            string savepath = GetFilePath(saveFileName);
            // 파일이 존재하는지 확인 후 삭제
            if (File.Exists(savepath)) {
                File.Delete(savepath);
                Debug.Log("File deleted successfully: " + savepath);
            }
            else {
                Debug.LogWarning("File does not exist: " + savepath);
            }
        }
        catch (Exception e) {
            Debug.LogError("Error deleting file: " + e.Message);
        }
    }

    public T[] OpenFileInFolder(string extension = null, bool isEncryption = false)
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
                    fileContent = EncryptionManager.DecryptAES(fileContent);
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

    public bool IsFileExist(string saveFileName)
    {
        return File.Exists(GetFilePath(saveFileName));
    }

    private string GetFolderPath()
    {
        return Path.Combine(Application.persistentDataPath, rootFolder);
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(GetFolderPath(), fileName + fileExtension);
    }
}
