using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.IO;
using System;
#endif
using UnityEngine;
using UnityEngine.UIElements;

using DialogueSystem.GlobalValue;
using DialogueSystem.Localization;
using DialogueSystem.Event;

namespace DialogueSystem
{
    // 대화 컨테이너를 생성할 수 있는 ScriptableObject
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue Script")]
    [System.Serializable]
    public class DialogueScript : ScriptableObject
    {

        [HideInInspector] public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();
        [HideInInspector] public List<DialogueChoiceNodeData> DialogueChoiceNodeDatas = new List<DialogueChoiceNodeData>();
        [HideInInspector] public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
        [HideInInspector] public List<EndNodeData> EndNodeDatas = new List<EndNodeData>();
        [HideInInspector] public List<StartNodeData> StartNodeDatas = new List<StartNodeData>();
        [HideInInspector] public List<TimerChoiceNodeData> TimerChoiceNodeDatas = new List<TimerChoiceNodeData>();
        [HideInInspector] public List<EventNodeData> EventNodeDatas = new List<EventNodeData>();
        [HideInInspector] public List<RandomNodeData> RandomNodeDatas = new List<RandomNodeData>();
        [HideInInspector] public List<CommandNodeData> CommandNodeDatas = new List<CommandNodeData>();
        [HideInInspector] public List<IfNodeData> IFNodeDatas = new List<IfNodeData>();

        // 모든 노드 데이터를 반환하는 프로퍼티
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

    // 노드 간의 연결 정보를 저장하는 클래스
    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string TargetNodeGuid;
    }

    // 모든 노드의 기본 클래스
    [System.Serializable]
    public class BaseNodeData
    {
        public string NodeGuid;
        public Vector2 Position;
    }

    // 대화 선택 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class DialogueChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // 대화 노드 포트 리스트
        public List<LanguageGeneric<AudioClip>> AudioClips; // 오디오 클립 리스트
        public DialogueCharacter Character; // 대화 캐릭터 정보
        public AvatarPosition AvatarPos; // 아바타 위치
        public AvatarType AvatarType; // 아바타 타입
        public List<LanguageGeneric<string>> TextType; // 대화 텍스트 리스트
        public float Duration; // 노드 지속 시간
    }

    // 타이머 선택 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class TimerChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // 대화 노드 포트 리스트
        public List<LanguageGeneric<AudioClip>> AudioClips; // 오디오 클립 리스트
        public DialogueCharacter Character; // 대화 캐릭터 정보
        public AvatarPosition AvatarPos; // 아바타 위치
        public AvatarType AvatarType; // 아바타 타입
        public List<LanguageGeneric<string>> TextType; // 대화 텍스트 리스트
        public float Duration; // 노드 지속 시간
        public float time; // 타이머 시간
    }

    // 랜덤 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class RandomNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // 대화 노드 포트 리스트
    }

    // 대화 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts; // 대화 노드 포트 리스트
        public List<LanguageGeneric<AudioClip>> AudioClips; // 오디오 클립 리스트
        public DialogueCharacter Character; // 대화 캐릭터 정보
        public AvatarPosition AvatarPos; // 아바타 위치
        public AvatarType AvatarType; // 아바타 타입
        public List<LanguageGeneric<string>> TextType; // 대화 텍스트 리스트
        public float Duration; // 노드 지속 시간
    }

    // 종료 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class EndNodeData : BaseNodeData
    {
        public EndNodeType EndNodeType; // 종료 노드 타입 (End, Repeat, GoBack, ReturnToStart)
    }

    // 시작 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class StartNodeData : BaseNodeData
    {
        public string startID; // 시작 노드 ID
    }

    // 이벤트 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class EventNodeData : BaseNodeData
    {
        public List<EventScriptableObjectData> EventScriptableObjects; // 이벤트 ScriptableObject 리스트
    }

    // 이벤트 ScriptableObject 데이터를 저장하는 클래스
    [System.Serializable]
    public class EventScriptableObjectData
    {
        public DialogueEvent DialogueEventSO; // 대화 이벤트 ScriptableObject
    }

    // 명령 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class CommandNodeData : BaseNodeData
    {
        public string commmand; // 명령 문자열
    }

    // 조건 노드 데이터를 저장하는 클래스
    [System.Serializable]
    public class IfNodeData : BaseNodeData
    {
        public string ValueName; // 조건 값 이름
        public GlobalValueIFOperations Operations; // 조건 연산자
        public string OperationValue; // 조건 값

        public string TrueGUID; // 조건이 참일 때 연결되는 노드 id
        public string FalseGUID; // 조건이 거짓일 때 연결되는 노드 id
    }

    // 언어별 제네릭 타입을 저장하는 클래스
    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LocalizationEnum languageEnum; // 언어 Enum
        public T LanguageGenericType; // 언어별 제네릭 타입
    }

    // 대화 노드 포트를 저장하는 클래스
    [System.Serializable]
    public class DialogueNodePort
    {
        public string InputGuid; // 입력 id
        public string OutputGuid; // 출력 id

#if UNITY_EDITOR
        [HideInInspector] public Port MyPort; // 에디터 전용 포트
#endif
        public TextField TextField; // 텍스트 필드
        public List<LanguageGeneric<string>> TextLanguage = new List<LanguageGeneric<string>>(); // 언어별 텍스트 리스트
    }

    // 종료 노드 타입 Enum
    [System.Serializable]
    public enum EndNodeType
    {
        End, // 종료
        Repeat, // 반복
        GoBack, // 돌아가기
        ReturnToStart // 시작으로 돌아가기
    }
}
