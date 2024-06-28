using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.IO;
using System;
#endif
using UnityEngine;
using UnityEngine.UIElements;

using MeetAndTalk.GlobalValue;
using MeetAndTalk.Localization;
using MeetAndTalk.Event;

namespace MeetAndTalk
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
    [System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();

        public List<DialogueChoiceNodeData> DialogueChoiceNodeDatas = new List<DialogueChoiceNodeData>();
        public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
        public List<EndNodeData> EndNodeDatas = new List<EndNodeData>();
        public List<StartNodeData> StartNodeDatas = new List<StartNodeData>();
        [Header("Pro Version Node")]
        public List<TimerChoiceNodeData> TimerChoiceNodeDatas = new List<TimerChoiceNodeData>();
        public List<EventNodeData> EventNodeDatas = new List<EventNodeData>();
        public List<RandomNodeData> RandomNodeDatas = new List<RandomNodeData>();
        public List<CommandNodeData> CommandNodeDatas = new List<CommandNodeData>();
        public List<IfNodeData> IFNodeDatas = new List<IfNodeData>();

        public List<BaseNodeData> AllNodes
        {
            get
            {
                List<BaseNodeData> tmp = new List<BaseNodeData>();
                tmp.AddRange(DialogueNodeDatas);
                tmp.AddRange(DialogueChoiceNodeDatas);
                tmp.AddRange(TimerChoiceNodeDatas);
                tmp.AddRange(EndNodeDatas);
                tmp.AddRange(EventNodeDatas);
                tmp.AddRange(StartNodeDatas);
                tmp.AddRange(RandomNodeDatas);
                tmp.AddRange(CommandNodeDatas);

                return tmp;
            }
        }
    }
    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string TargetNodeGuid;
    }

    [System.Serializable]
    public class BaseNodeData
    {
        public string NodeGuid;
        public Vector2 Position;
    }

    [System.Serializable]
    public class DialogueChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public DialogueCharacterSO Character;
        public AvatarPosition AvatarPos;
        public AvatarType AvatarType;
        public List<LanguageGeneric<string>> TextType;
        public float Duration;
    }

    [System.Serializable]
    public class TimerChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public DialogueCharacterSO Character;
        public AvatarPosition AvatarPos;
        public AvatarType AvatarType;
        public List<LanguageGeneric<string>> TextType;
        public float Duration;
        public float time;
    }

    [System.Serializable]
    public class RandomNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
    }

    [System.Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public DialogueCharacterSO Character;
        public AvatarPosition AvatarPos;
        public AvatarType AvatarType;
        public List<LanguageGeneric<string>> TextType;
        public float Duration;
    }

    [System.Serializable]
    public class EndNodeData : BaseNodeData
    {
        public EndNodeType EndNodeType;
    }

    [System.Serializable]
    public class StartNodeData : BaseNodeData
    {
        public string startID;
    }


    [System.Serializable]
    public class EventNodeData : BaseNodeData
    {
        public List<EventScriptableObjectData> EventScriptableObjects;
    }
    [System.Serializable]
    public class EventScriptableObjectData
    {
        public DialogueEventSO DialogueEventSO;
    }

    [System.Serializable]
    public class CommandNodeData : BaseNodeData
    {
        public string commmand;
    }
    [System.Serializable]
    public class IfNodeData : BaseNodeData
    {
        public string ValueName;
        public GlobalValueIFOperations Operations;
        public string OperationValue;

        public string TrueGUID;
        public string FalseGUID;
    }

    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LocalizationEnum languageEnum;
        public T LanguageGenericType;
    }

    [System.Serializable]
    public class DialogueNodePort
    {
        public string InputGuid;
        public string OutputGuid;
#if UNITY_EDITOR
        [HideInInspector] public Port MyPort;
#endif
        public TextField TextField;
        public List<LanguageGeneric<string>> TextLanguage = new List<LanguageGeneric<string>>();
    }

    [System.Serializable]
    public enum EndNodeType
    {
        End,
        Repeat,
        GoBack,
        ReturnToStart
    }
}