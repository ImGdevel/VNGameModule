using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeetAndTalk.Event
{
    [CreateAssetMenu(menuName = "Dialogue/Event/Console Log")]
    [System.Serializable]
    public class ConsoleLogEvent : DialogueEventSO
    {
        public LogType logType;
        public string Content;

        public override void RunEvent()
        {
            DialogueEventManager.Instance.ConsoleLogEvent(Content, logType);
            base.RunEvent();
        }
    }

    public enum LogType
    {
        Info = 0, Warning = 1, Error = 2
    }
}
