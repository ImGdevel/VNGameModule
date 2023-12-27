using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    int gameSaveSlotCount;
    public string uid;
    public List<SaveData> saveDatas = new();

    public GameData(int gameSaveSlotCount = 0)
    {
        this.gameSaveSlotCount = gameSaveSlotCount;
    }
}

