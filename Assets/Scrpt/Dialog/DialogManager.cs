using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    public string currentSceneName;

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
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        sceneDialogs = new Dictionary<string, List<DialogData>>();
        DialogueLoader dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader != null) {
            sceneDialogs = dialogueLoader.GetSceneDialogs();
        }
    }

    private void LoadEventList() {
        sceneDialogs = new Dictionary<string, List<DialogData>>();
        DialogueLoader dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader != null) {
            sceneEvents = dialogueLoader.GetSceneEvents();
        }
    }

    public string GetCurrentSceneName() {
        return currentSceneName;
    }

    public List<DialogData> GetSceneDialogs() {
        if (sceneDialogs.Count == 0) {
            LoadDialogueData();
        }
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneDialogs.TryGetValue(currentSceneName, out var dialogs)) {
            return dialogs;
        }
        return null;
    }

    public List<DialogData> GetSceneDialogs(string loadScene) {
        if (sceneDialogs.Count == 0) {
            LoadDialogueData();
        }
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneDialogs.TryGetValue(currentSceneName, out var dialogs)) {
            
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

    public string GetCharacterName(string characterKey) {
        if (characterNames.TryGetValue(characterKey, out var name)) {
            return name;
        }
        return characterKey; // Return the key if character name not found
    }
}
