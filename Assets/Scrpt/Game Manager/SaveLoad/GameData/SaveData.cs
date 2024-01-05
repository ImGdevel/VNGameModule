using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int saveNumber;
    public string saveName;
    public string saveCreateTime;
    public string saveLastTime;
    public string saveSnapshotImage;
    public int gamePlayTime;
    public GameData gameData;

    public SaveData(GameData gameData = default) {
        this.saveNumber = 0;
        this.saveName = "save";
        this.saveCreateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.saveLastTime = saveCreateTime;
        this.saveSnapshotImage = "";
        this.gamePlayTime = 0;
        this.gameData = gameData;
    }

    public SaveData(int saveNumber = 1, GameData gameData = default) 
    {
        this.saveNumber = saveNumber;
        this.saveName = "save" + ((saveNumber != 0) ? "_" + saveNumber.ToString("D2") : "");
        this.saveCreateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.saveLastTime = saveCreateTime;
        this.saveSnapshotImage = "";
        this.gamePlayTime = 0;
        this.gameData = gameData;
    }

}
