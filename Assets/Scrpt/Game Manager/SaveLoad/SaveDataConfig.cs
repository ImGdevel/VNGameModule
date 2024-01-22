using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveDataConfig
{
    public string uid;
    public string gameId;
    public int maxSaveSlots;
    public List<SaveConfig> saveConfigs;

    public SaveDataConfig()
    {
        this.uid = string.Empty;
        this.gameId = string.Empty;
        this.maxSaveSlots = 0;
        this.saveConfigs = new List<SaveConfig>();
    }

    public void AddSaveConfig(string saveFileName, int saveNumber = 0)
    {
        saveConfigs.Add(new SaveConfig(saveFileName, saveNumber));
    }

    public void RemoveSaveConfig(string saveFileName, int saveNumber = 0)
    {
        SaveConfig saveConfigToRemove = saveConfigs.Find(s => s.saveFileName == saveFileName && s.saveNumber == saveNumber);

        if (saveConfigToRemove != null) {
            saveConfigs.Remove(saveConfigToRemove);
        }
    }
}

[System.Serializable]
public class SaveConfig
{
    public int saveNumber;
    public string saveFileName;

    public SaveConfig(string saveFileName, int saveNumber)
    {
        this.saveFileName = saveFileName;
        this.saveNumber = saveNumber;
    }
}