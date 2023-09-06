using System.Collections;
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
        DontDestroyOnLoad(this);
    }

    private void LoadDialogueData() {
        sceneDialogs = new Dictionary<string, List<DialogData>>();
        Dictionary<string, List<DialogData>> dialogDatas = VNDialogueLoader.LoadDialogueFile();
        if (dialogDatas != null) {
            sceneDialogs = dialogDatas;
        }
        else {
            Debug.LogWarning("No dialogue datas");
        }
    }

    private void LoadEventList() {
        sceneDialogs = new Dictionary<string, List<DialogData>>();
        Dictionary<string, List<EventData>> eventDatas = VNDialogueLoader.LoadEventFile();
        if (eventDatas != null) {
            sceneEvents = eventDatas;
        }
        else {
            Debug.LogWarning("No event datas");
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
        if (sceneEvents == null) {
            LoadEventList();
        }
        return sceneEvents;
    }

    public Dictionary<string, string> GetCharacterName() {
        return characterNames;
    }
}
