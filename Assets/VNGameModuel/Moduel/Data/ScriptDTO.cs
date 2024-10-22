using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualNovelGame.Data
{
    [System.Serializable]
    public abstract class ScriptDTO
    {
        public string scriptId;

        public ScriptDTO(string scriptId)
        {
            this.scriptId = scriptId;
        }
    }

    [System.Serializable]
    public class DialogueScriptDTO : ScriptDTO
    {
        public AudioClip AudioClips;
        public string DialogueText;
        public string CharacterName;

        public DialogueScriptDTO(string scriptId, string dialogueText, string characterName, AudioClip audioClips = null)
            : base(scriptId)
        {
            this.AudioClips = audioClips;
            this.DialogueText = dialogueText;
            this.CharacterName = characterName;
        }
    }

    [System.Serializable]
    public class ChoiceScriptDTO : ScriptDTO
    {
        public AudioClip AudioClips;
        public string DialogueText;
        public string CharacterName;
        public List<Choice> Choices;

        public ChoiceScriptDTO(string scriptId, string dialogueText, string characterName, List<Choice> choices, AudioClip audioClips = null)
            : base(scriptId)
        {
            this.AudioClips = audioClips;
            this.DialogueText = dialogueText;
            this.CharacterName = characterName;
            this.Choices = choices;
        }
    }

    [System.Serializable]
    public class TimerChoiceScriptDTO : ChoiceScriptDTO
    {
        public float TimeLimit;

        public TimerChoiceScriptDTO(string scriptId, string dialogueText, string characterName, List<Choice> choices, float timeLimits, AudioClip audioClips = null)
            : base(scriptId, dialogueText, characterName, choices, audioClips)
        {
            this.TimeLimit = timeLimits;
        }
    }


    [System.Serializable]
    public class RandomScriptDTO : ScriptDTO
    {
        public List<Choice> randomChoices;

        public RandomScriptDTO(string scriptId, List<Choice> randomChoices)
            : base(scriptId)
        {
            this.randomChoices = randomChoices;
        }
    }

    [System.Serializable]
    public class Choice
    {
        public string nextScriptId;
        public string ChoiceText;

        public Choice(string nextScriptId, string choiceText = "")
        {
            this.nextScriptId = nextScriptId;
            this.ChoiceText = choiceText;
        }
    }

    [System.Serializable]
    public class CharacterScriptDTO : ScriptDTO
    {
        public string CharacterName;
        public Sprite CharacterSprite;
        public CharacterEffectType CharacterEffectType;
        public Vector3 SpritePos;
        public Vector3 SpriteSize;
        public float EffectWeight;
        public float Duration;

        public CharacterScriptDTO(string scriptId, string characterName, Sprite characterSprite, CharacterEffectType characterEffectType, Vector3 spritePos, Vector3 spriteSize, float effectWeight, float duration)
            : base(scriptId)
        {
            this.CharacterName = characterName;
            this.CharacterSprite = characterSprite;
            this.CharacterEffectType = characterEffectType;
            this.SpritePos = spritePos;
            this.SpriteSize = spriteSize;
            this.EffectWeight = effectWeight;
            this.Duration = duration;
        }
    }

    [System.Serializable]
    public class EndScriptDTO : ScriptDTO
    {
        public string EndNodeType;

        public EndScriptDTO(string scriptId, string endNodeType)
            : base(scriptId)
        {
            this.EndNodeType = endNodeType;
        }
    }

    [System.Serializable]
    public class StartScriptDTO : ScriptDTO
    {
        public string StartID;

        public StartScriptDTO(string scriptId, string startId)
            : base(scriptId)
        {
            this.StartID = startId;
        }
    }

    [System.Serializable]
    public class EventScriptDTO : ScriptDTO
    {
        public List<EventScriptableObjectDTO> EventScriptableObjects;

        public EventScriptDTO(string scriptId, List<EventScriptableObjectDTO> eventScriptableObjects)
            : base(scriptId)
        {
            this.EventScriptableObjects = eventScriptableObjects;
        }
    }

    [System.Serializable]
    public class IfScriptDTO : ScriptDTO
    {
        public string ValueName;
        public string Operations;
        public string OperationValue;
        public string TrueGUID;
        public string FalseGUID;

        public IfScriptDTO(string scriptId, string valueName, string operations, string operationValue, string trueGuid, string falseGuid)
            : base(scriptId)
        {
            this.ValueName = valueName;
            this.Operations = operations;
            this.OperationValue = operationValue;
            this.TrueGUID = trueGuid;
            this.FalseGUID = falseGuid;
        }
    }

    [System.Serializable]
    public class EventScriptableObjectDTO
    {
        public string DialogueEvent;

        public EventScriptableObjectDTO(string dialogueEvent)
        {
            this.DialogueEvent = dialogueEvent;
        }
    }

}


