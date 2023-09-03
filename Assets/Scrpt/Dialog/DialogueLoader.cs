using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DialogueLoader : MonoBehaviour
{

    [SerializeField] private TextAsset dialogueJsonAsset;
    [SerializeField] private TextAsset sceneEvnetJsonAsset;

    private Dictionary<string, string> characterNames;
    private Dictionary<string, List<DialogData>> sceneDialogs;
    private Dictionary<string, List<EventData>> sceneEvents;

    public void LoadDialogueData() {
        if (dialogueJsonAsset == null) {
            Debug.LogError("No JSON asset assigned.");
            return;
        }
        string dialogJsonFileName = dialogueJsonAsset.text;
        string eventJsonFileName = sceneEvnetJsonAsset.text;

        DialogueJSONData dialogJsonData = JsonUtility.FromJson<DialogueJSONData>(dialogJsonFileName);
        SceneEventJSONData eventJsonData = JsonUtility.FromJson<SceneEventJSONData>(eventJsonFileName);

        sceneDialogs = new Dictionary<string, List<DialogData>>();
        sceneEvents = new Dictionary<string, List<EventData>>();
        characterNames = dialogJsonData.characters;

        foreach (var sceneData in dialogJsonData.scenes) {
            sceneDialogs.Add(sceneData.name, sceneData.dialogs);
        }

        foreach (var eventData in eventJsonData.events) {
            sceneEvents.Add(eventData.id, eventData.events);
        }
    }

    public Dictionary<string, List<DialogData>> GetSceneDialogs() {
        if (sceneDialogs == null) {
            LoadDialogueData();
        }
        return sceneDialogs;
    }

    public Dictionary<string, List<EventData>> GetSceneEvents() {
        if (sceneDialogs == null) {
            LoadDialogueData();
        }
        return sceneEvents;
    }

    public Dictionary<string, string> GetCharacterNames() {
        if (characterNames == null) {
            LoadDialogueData();
        }
        return characterNames;
    }
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
    public string data;
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
