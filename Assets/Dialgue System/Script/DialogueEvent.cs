using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace DialogueSystem.Event
{
    /// <summary>
    /// Basic Event Class
    /// </summary>
    public abstract class DialogueEvent : ScriptableObject
    {
        #region Variables
        // Here you can add the variables you want to change in the Scriptable Object
        #endregion

        /// <summary>.
        /// The RunDialogue function is called by the Event Node
        /// It can also be called manually
        /// </summary>.
        public abstract void RunEvent();
    }

    public static class EventSOCreator
    {
        private static string scriptFilePath;

        public static string k_scriptFilePath {
            get {
                if (string.IsNullOrEmpty(scriptFilePath)) {
                    scriptFilePath = GetScriptFilePath();
                }
                return Path.GetDirectoryName(scriptFilePath);
            }
        }

        private static string GetScriptFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null)
        {
            return path;
        }

        [MenuItem("Assets/Create/Dialogue/New Event")]
        public static void NewEvent()
        {
            string script = k_scriptFilePath + "/Events/EventSOTemplate.cs.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(script, "Event.cs");
        }
    }
}