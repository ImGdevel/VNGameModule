using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeetAndTalk.Event
{
    [System.Serializable]
    public class DialogueEventSO : ScriptableObject
    {
        public virtual void RunEvent()
        {
            //Debug.Log("Event called");
        }
    }
}