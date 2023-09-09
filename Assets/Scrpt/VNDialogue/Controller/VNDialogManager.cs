using System.Collections.Generic;
using UnityEngine;

public class VNDialogManager : MonoBehaviour
{
    public static VNDialogManager Instance { get; private set; }

    private Dictionary<string, List<DialogData>> sceneDialogs;
    private Dictionary<string, List<EventData>> sceneEvents;
    private Dictionary<string, string> characterNames;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadDialogueData();
        LoadEventList();
        LoadCharacterNames();
    }

    private void LoadDialogueData() {
        sceneDialogs = VNDialogueLoader.LoadDialogueFile("en");
        if (sceneDialogs == null || sceneDialogs.Count == 0) {
            Debug.LogWarning("No dialogue datas");
        }
    }

    private void LoadEventList() {
        sceneEvents = VNDialogueLoader.LoadEventFile("event");
        if (sceneEvents == null || sceneEvents.Count == 0) {
            Debug.LogWarning("No event datas");
        }
    }

    private void LoadCharacterNames() {
        characterNames = VNDialogueLoader.LoadCharacterNames("en");
        if (characterNames == null || characterNames.Count == 0) {
            //Debug.LogWarning("No character names");
        }
    }

    public List<DialogData> GetSceneDialogs(string loadSceneName) {
        if (sceneDialogs == null || sceneDialogs.Count == 0) {
            LoadDialogueData();
        }
        if (sceneDialogs.TryGetValue(loadSceneName, out var dialogs)) {
            return dialogs;
        }
        return null;
    }

    public Dictionary<string, List<EventData>> GetSceneEvents() {
        if (sceneEvents == null || sceneEvents.Count == 0) {
            LoadEventList();
        }
        return sceneEvents;
    }

    public Dictionary<string, string> GetCharacterNames() {
        if (characterNames == null || characterNames.Count == 0) {
            LoadCharacterNames();
        }
        return characterNames;
    }
}
