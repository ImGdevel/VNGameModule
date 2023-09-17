using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UserData
{
    public string uid;
    public List<SaveData> saveDatas = new();
} 

[System.Serializable]
public class SaveData
{
    public string playerName;
    public string chapter;
    public string dialogId;
}


