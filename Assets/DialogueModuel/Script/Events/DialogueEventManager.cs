using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueSystem.Event
{
    public class DialogueEventManager : MonoBehaviour
    {
        public static DialogueEventManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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
    }
}
