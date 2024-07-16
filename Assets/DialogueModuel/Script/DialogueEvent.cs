using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem.Event
{
    [System.Serializable]
    public class DialogueEvent : ScriptableObject
    {
        public virtual void RunEvent()
        {
            //Debug.Log("Event called");
        }
    }
}