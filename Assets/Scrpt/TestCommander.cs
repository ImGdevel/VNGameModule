using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;

public class TestCommander : MonoBehaviour
{
    public ScenarioManager scenarioManager;
    string currentScene = null;


    void Start()
    {
        currentScene = scenarioManager.GetStartSceneId();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T)) {
            Debug.Log("id:" + currentScene);
            ScriptDTO script = scenarioManager.GetSceneDataById(currentScene);
            
            if(script is DialogueScriptDTO dto) {
                Debug.Log(dto.DialogueText);
            }

            currentScene = scenarioManager.GetNextSceneIdById(script.scriptId);
        }
    }
}
