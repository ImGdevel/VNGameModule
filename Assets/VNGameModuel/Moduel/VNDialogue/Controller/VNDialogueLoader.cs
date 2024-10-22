using UnityEngine;
using System.Collections.Generic;
using System;

public class VNDialogueLoader : MonoBehaviour
{
    public static Dictionary<string, List<DialogData>> LoadDialogueFile(string fileName) {
        try {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            DialogueJSONData dialogJsonData = JsonUtility.FromJson<DialogueJSONData>(textAsset.text);

            Dictionary<string, List<DialogData>> Dialogs = new Dictionary<string, List<DialogData>>();

            foreach (var sceneData in dialogJsonData.scenes) {
                Dialogs.Add(sceneData.name, sceneData.dialogs);
            }
            return Dialogs;
        }
        catch (Exception e) {
            Debug.LogError("Error loading dialogue file: " + e.Message);
            return null;
        }
    }

    public static Dictionary<string, List<EventData>> LoadEventFile(string fileName) {
        try {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            SceneEventJSONData eventJsonData = JsonUtility.FromJson<SceneEventJSONData>(textAsset.text);

            Dictionary<string, List<EventData>> Events = new Dictionary<string, List<EventData>>();

            foreach (var eventData in eventJsonData.events) {
                Events.Add(eventData.id, eventData.events);
            }
            return Events;
        }
        catch (Exception e) {
            Debug.LogError("Error loading event file: " + e.Message);
            return null;
        }
    }

    public static Dictionary<string, string> LoadCharacterNames(string fileName) {
        try {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            DialogueJSONData dialogJsonData = JsonUtility.FromJson<DialogueJSONData>(textAsset.text);

            Dictionary<string, string> characterNames = dialogJsonData.characters;
            return characterNames;
        }
        catch (Exception e) {
            Debug.LogError("Error loading character names: " + e.Message);
            return null;
        }
    }
}

[System.Serializable]
public class VNDialogDatas
{
    public Dictionary<string, List<DialogData>> dialogueDatas;
    public Dictionary<string, List<EventData>> eventDatas;
    public Dictionary<string, string> characterNames;
}

[System.Serializable]
public class DialogData
{
    public string id;
    public string character;
    public string content;
    public List<ChoiceData> choices;
}

[System.Serializable]
public class ChoiceData
{
    public string text;
    public string nextDialog;
}

[System.Serializable]
public class SceneData
{
    public string name;
    public List<DialogData> dialogs;
}

[System.Serializable]
public class DialogueJSONData
{
    public string language;
    public Dictionary<string, string> characters;
    public List<SceneData> scenes;
}

[System.Serializable]
public class EventData
{
    public string type;
    public Data data;
}

[System.Serializable]
public class Data
{
    public string name;
    public int number;
    public float time;
    public Position position;
    public float scale;
}

[System.Serializable]
public class Position
{
    public float x;
    public float y;
}

[System.Serializable]
public class SceneEventData
{
    public string id;
    public List<EventData> events;
}

[System.Serializable]
public class SceneEventJSONData
{
    public string scenes;
    public List<SceneEventData> events;
}