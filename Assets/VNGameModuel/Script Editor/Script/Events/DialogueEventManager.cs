using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace DialogueSystem.Event
{
    public class DialogueEventManager : MonoBehaviour
    {
        public static DialogueEventManager Instance { get; private set; }
        public List<SceneDialogEventInvoke> SceneEventList;

        private void Awake()
        {
            Instance = this;
        }


        public void InvokeSceneEvent(string ID)
        {
            foreach (SceneDialogEventInvoke Event in SceneEventList)
            {
                if (Event.ID == ID) { Event.SceneEvent.Invoke(); }
            }
        }



        public void ConsoleLogEvent(string content, LogType type)
        {
            if (type == LogType.Info) Debug.Log(content);
            if (type == LogType.Warning) Debug.LogWarning(content);
            if (type == LogType.Error) Debug.LogError(content);
        }

        public void CharacterEvent(DialogueCharacter character)
        {
            Debug.LogFormat($"<color=#{ColorUtility.ToHtmlStringRGBA(character.textColor)}>{character.GetName()}");
        }

        public void BackgroundEvent()
        {

        }
    }

    [System.Serializable]
    public class SceneDialogEventInvoke
    {
        public string ID;
        public UnityEvent SceneEvent;
    }
}
