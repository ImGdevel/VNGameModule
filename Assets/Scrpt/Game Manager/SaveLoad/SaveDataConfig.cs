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
}

[System.Serializable]
public class SaveConfig
{
    public int saveNumber;
    public string saveFileName;
}