using DialogueSystem;
using DialogueSystem.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace VisualNovelGame
{
    public class ScenarioManager : DialogueGetData
    {
        public LocalizationManager localizationManager;

        ScriptMapper scriptMapper;

        void Awake()
        {
            if (localizationManager != null) {
                scriptMapper = new ScriptMapper(localizationManager);
            }
            else {
                Debug.LogError("LocalizationManager가 설정되지 않았습니다.");
            }
        }

        public string GetStartSceneId()
        {
            BaseNodeData baseNode = GetNextNode(dialogueScript.StartNodeDatas[0]);
            return baseNode.NodeGuid;
        }

        public ScriptDTO GetSceneDataById(string id)
        {
            BaseNodeData baseNode = GetNodeByGuid(id);
            return CheckNodeType(baseNode);
        }

        public ScriptDTO GetNextSceneData(string id)
        {
            BaseNodeData nextNode = GetNextNode(GetNodeByGuid(id));
            return CheckNodeType(nextNode);
        }

        public string GetNextSceneIdById(string id)
        {
            BaseNodeData nextNode = GetNextNode(GetNodeByGuid(id));
            return nextNode.NodeGuid;
        }

        private ScriptDTO CheckNodeType(BaseNodeData baseNodeData)
        {
            switch (baseNodeData) {
                case StartNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                case DialogueNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                case ChoiceNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                case TimerChoiceNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);
                    
                case EventNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                case EndNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                case RandomNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                case IfNodeData nodeData:
                    return scriptMapper.ToDTO(nodeData);

                default:
                    return null;
            }
        }
    }
}

public class ScriptMapper
{
    LocalizationManager localizationManager;

    public ScriptMapper(LocalizationManager localizationManager)
    {
        this.localizationManager = localizationManager;
    }

    public DialogueScriptDTO ToDTO(DialogueNodeData nodeData)
    {
        LocalizationEnum localization = localizationManager.SelectedLang();
        DialogueCharacter character = nodeData.Character;

        string characterName = $"<color={nodeData.Character.HexColor()}>" +
            $"{character.characterName.Find(text => text.languageEnum == localization).LanguageGenericType}</color>";
        string text = $"{nodeData.TextType.Find(text => text.languageEnum == localization).LanguageGenericType}";
        AudioClip audioClip = nodeData.AudioClips.Find(clip => clip.languageEnum == localization).LanguageGenericType;

        return new DialogueScriptDTO(
                nodeData.NodeGuid,
                text,
                characterName,
                audioClip
            );
    }

    public ChoiceScriptDTO ToDTO(ChoiceNodeData nodeData)
    {
        LocalizationEnum localization = localizationManager.SelectedLang();
        DialogueCharacter character = nodeData.Character;

        string characterName = $"<color={character.HexColor()}>" +
            $"{character.characterName.Find(text => text.languageEnum == localization).LanguageGenericType}</color>";
        string text = nodeData.TextType.Find(t => t.languageEnum == localization).LanguageGenericType;
        AudioClip audioClip = nodeData.AudioClips?.Find(clip => clip.languageEnum == localization)?.LanguageGenericType;

        List<Choice> choices = new List<Choice>();
        foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts) {
            string choiceText = nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType;
            choices.Add(new Choice(nodePort.InputGuid, choiceText));
        }

        return new ChoiceScriptDTO(
            nodeData.NodeGuid,
            text,
            characterName,
            choices,
            audioClip
        );
    }

    public TimerChoiceScriptDTO ToDTO(TimerChoiceNodeData nodeData)
    {
        LocalizationEnum localization = localizationManager.SelectedLang();
        DialogueCharacter character = nodeData.Character;

        string characterName = $"<color={character.HexColor()}>" +
            $"{character.characterName.Find(text => text.languageEnum == localization).LanguageGenericType}</color>";
        string text = nodeData.TextType.Find(t => t.languageEnum == localization).LanguageGenericType;
        AudioClip audioClip = nodeData.AudioClips?.Find(clip => clip.languageEnum == localization)?.LanguageGenericType;

        List<Choice> choices = new List<Choice>();
        foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts) {
            string choiceText = nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType;
            choices.Add(new Choice(nodePort.InputGuid, choiceText));
        }

        return new TimerChoiceScriptDTO(
            nodeData.NodeGuid,
            text,
            characterName,
            choices,
            nodeData.time,
            audioClip
        );
    }

    public RandomScriptDTO ToDTO(RandomNodeData nodeData)
    {
        List<Choice> choices = null;
        foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts) {
            choices.Add(new Choice(nodePort.InputGuid));
        }

        return new RandomScriptDTO(nodeData.NodeGuid, choices);
    }

    // IfNodeData를 IfScriptDTO로 변환
    public IfScriptDTO ToDTO(IfNodeData nodeData)
    {
        return new IfScriptDTO(
            nodeData.NodeGuid,
            nodeData.ValueName,
            nodeData.Operations.ToString(),
            nodeData.OperationValue,
            nodeData.TrueGUID,
            nodeData.FalseGUID
        );
    }

    public EventScriptDTO ToDTO(EventNodeData nodeData)
    {
        List<EventScriptableObjectDTO> events = new List<EventScriptableObjectDTO>();

        foreach (var eventObj in nodeData.EventScriptableObjects) {
            if (eventObj.DialogueEvent != null) {
                events.Add(new EventScriptableObjectDTO(eventObj.DialogueEvent.ToString()));
            }
        }

        return new EventScriptDTO(nodeData.NodeGuid, events);
    }

    public StartScriptDTO ToDTO(StartNodeData nodeData)
    {
        return new StartScriptDTO(nodeData.NodeGuid, nodeData.startID);
    }

    public EndScriptDTO ToDTO(EndNodeData nodeData)
    {
        return new EndScriptDTO(nodeData.NodeGuid, nodeData.EndNodeType.ToString());
    }

}

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
    public string CharacterId;
    public string CharacterName;
    public Sprite CharacterSprite;
    public Vector3 SpritePos;
    public Vector3 SpriteSize;
    public float Duration;

    public CharacterScriptDTO(string scriptId, string characterId, string characterName, Sprite characterSprite, Vector3 spritePos, Vector3 spriteSize, float duration)
        : base(scriptId)
    {
        this.CharacterId = characterId;
        this.CharacterName = characterName;
        this.CharacterSprite = characterSprite;
        this.SpritePos = spritePos;
        this.SpriteSize = spriteSize;
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
