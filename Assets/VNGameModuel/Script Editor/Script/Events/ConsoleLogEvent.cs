using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Event
{
    [CreateAssetMenu(menuName = "Dialogue/Event/Console Log")]
    public class ConsoleLogEvent : DialogueEvent
    {
        #region Variables
        public LogType logType;
        public string Content;
        #endregion

        /// <summary>.
        /// The RunDialogue function is called by the Event Node
        /// It can also be called manually
        /// </summary>.
        public override void RunEvent()
        {
            DialogueEventManager.Instance.ConsoleLogEvent(Content, logType);
        }
    }

    public enum LogType
    {
        Info = 0, Warning = 1, Error = 2
    }
}
