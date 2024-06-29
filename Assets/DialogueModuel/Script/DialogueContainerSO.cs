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
    // ��ȭ �����̳ʸ� ������ �� �ִ� ScriptableObject
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
    [System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>(); // ��� �� ���� �����͸� �����ϴ� ����Ʈ
        public List<DialogueChoiceNodeData> DialogueChoiceNodeDatas = new List<DialogueChoiceNodeData>(); // ��ȭ ���� ��� �����͸� �����ϴ� ����Ʈ
        public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>(); // ��ȭ ��� �����͸� �����ϴ� ����Ʈ
        public List<EndNodeData> EndNodeDatas = new List<EndNodeData>(); // ���� ��� �����͸� �����ϴ� ����Ʈ
        public List<StartNodeData> StartNodeDatas = new List<StartNodeData>(); // ���� ��� �����͸� �����ϴ� ����Ʈ

        [Header("Pro Version Node")]
        public List<TimerChoiceNodeData> TimerChoiceNodeDatas = new List<TimerChoiceNodeData>(); // Ÿ�̸� ���� ��� �����͸� �����ϴ� ����Ʈ
        public List<EventNodeData> EventNodeDatas = new List<EventNodeData>(); // �̺�Ʈ ��� �����͸� �����ϴ� ����Ʈ
        public List<RandomNodeData> RandomNodeDatas = new List<RandomNodeData>(); // ���� ��� �����͸� �����ϴ� ����Ʈ
        public List<CommandNodeData> CommandNodeDatas = new List<CommandNodeData>(); // ��� ��� �����͸� �����ϴ� ����Ʈ
        public List<IfNodeData> IFNodeDatas = new List<IfNodeData>(); // ���� ��� �����͸� �����ϴ� ����Ʈ

        // ��� ��� �����͸� ��ȯ�ϴ� ������Ƽ
        public List<BaseNodeData> AllNodes {
            get {
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

    // ��� ���� ���� ������ �����ϴ� Ŭ����
    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid; // ���� ����� GUID
        public string TargetNodeGuid; // ��� ����� GUID
    }

    // ��� ����� �⺻ Ŭ����
    [System.Serializable]
    public class BaseNodeData
    {
        public string NodeGuid; // ����� GUID
        public Vector2 Position; // ����� ��ġ
    }

    // ��ȭ ���� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class DialogueChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // ��ȭ ��� ��Ʈ ����Ʈ
        public List<LanguageGeneric<AudioClip>> AudioClips; // ����� Ŭ�� ����Ʈ
        public DialogueCharacterSO Character; // ��ȭ ĳ���� ����
        public AvatarPosition AvatarPos; // �ƹ�Ÿ ��ġ
        public AvatarType AvatarType; // �ƹ�Ÿ Ÿ��
        public List<LanguageGeneric<string>> TextType; // ��ȭ �ؽ�Ʈ ����Ʈ
        public float Duration; // ��� ���� �ð�
    }

    // Ÿ�̸� ���� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class TimerChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // ��ȭ ��� ��Ʈ ����Ʈ
        public List<LanguageGeneric<AudioClip>> AudioClips; // ����� Ŭ�� ����Ʈ
        public DialogueCharacterSO Character; // ��ȭ ĳ���� ����
        public AvatarPosition AvatarPos; // �ƹ�Ÿ ��ġ
        public AvatarType AvatarType; // �ƹ�Ÿ Ÿ��
        public List<LanguageGeneric<string>> TextType; // ��ȭ �ؽ�Ʈ ����Ʈ
        public float Duration; // ��� ���� �ð�
        public float time; // Ÿ�̸� �ð�
    }

    // ���� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class RandomNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // ��ȭ ��� ��Ʈ ����Ʈ
    }

    // ��ȭ ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // ��ȭ ��� ��Ʈ ����Ʈ
        public List<LanguageGeneric<AudioClip>> AudioClips; // ����� Ŭ�� ����Ʈ
        public DialogueCharacterSO Character; // ��ȭ ĳ���� ����
        public AvatarPosition AvatarPos; // �ƹ�Ÿ ��ġ
        public AvatarType AvatarType; // �ƹ�Ÿ Ÿ��
        public List<LanguageGeneric<string>> TextType; // ��ȭ �ؽ�Ʈ ����Ʈ
        public float Duration; // ��� ���� �ð�
    }

    // ���� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class EndNodeData : BaseNodeData
    {
        public EndNodeType EndNodeType; // ���� ��� Ÿ�� (End, Repeat, GoBack, ReturnToStart)
    }

    // ���� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class StartNodeData : BaseNodeData
    {
        public string startID; // ���� ��� ID
    }

    // �̺�Ʈ ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class EventNodeData : BaseNodeData
    {
        public List<EventScriptableObjectData> EventScriptableObjects; // �̺�Ʈ ScriptableObject ����Ʈ
    }

    // �̺�Ʈ ScriptableObject �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class EventScriptableObjectData
    {
        public DialogueEventSO DialogueEventSO; // ��ȭ �̺�Ʈ ScriptableObject
    }

    // ��� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class CommandNodeData : BaseNodeData
    {
        public string commmand; // ��� ���ڿ�
    }

    // ���� ��� �����͸� �����ϴ� Ŭ����
    [System.Serializable]
    public class IfNodeData : BaseNodeData
    {
        public string ValueName; // ���� �� �̸�
        public GlobalValueIFOperations Operations; // ���� ������
        public string OperationValue; // ���� ��

        public string TrueGUID; // ������ ���� �� ����Ǵ� ��� GUID
        public string FalseGUID; // ������ ������ �� ����Ǵ� ��� GUID
    }

    // �� ���׸� Ÿ���� �����ϴ� Ŭ����
    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LocalizationEnum languageEnum; // ��� Enum
        public T LanguageGenericType; // �� ���׸� Ÿ��
    }

    // ��ȭ ��� ��Ʈ�� �����ϴ� Ŭ����
    [System.Serializable]
    public class DialogueNodePort
    {
        public string InputGuid; // �Է� GUID
        public string OutputGuid; // ��� GUID

#if UNITY_EDITOR
        [HideInInspector] public Port MyPort; // ������ ���� ��Ʈ
#endif
        public TextField TextField; // �ؽ�Ʈ �ʵ�
        public List<LanguageGeneric<string>> TextLanguage = new List<LanguageGeneric<string>>(); // �� �ؽ�Ʈ ����Ʈ
    }

    // ���� ��� Ÿ�� Enum
    [System.Serializable]
    public enum EndNodeType
    {
        End, // ����
        Repeat, // �ݺ�
        GoBack, // ���ư���
        ReturnToStart // �������� ���ư���
    }
}
