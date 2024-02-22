using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct GameObjectEntry
{
    [ReadOnly]
    public int id;

    [Multiline]
    public string text;
}

public class YourScript : MonoBehaviour
{

    public string SecenName;

    public List<GameObjectEntry> gameObjectsList = new List<GameObjectEntry>();

    public void AddGameObjectEntry()
    {
        GameObjectEntry entry = new GameObjectEntry {
            id = gameObjectsList.Count,
        };

        gameObjectsList.Add(entry);

    }
}
