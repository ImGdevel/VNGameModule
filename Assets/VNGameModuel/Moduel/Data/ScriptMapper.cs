using DialogueSystem.Localization;
using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualNovelGame.Data
{
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

            string characterName = character != null
                    ? $"<color={character.HexColor()}>{character.characterName.Find(text => text.languageEnum == localization).LanguageGenericType}</color>"
                    : "";
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

            string characterName = character != null
                    ? $"<color={character.HexColor()}>{character.characterName.Find(text => text.languageEnum == localization).LanguageGenericType}</color>"
                    : "";
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

        public CharacterScriptDTO ToDTO(CharacterNodeData nodeData)
        {
            LocalizationEnum localization = localizationManager.SelectedLang();

            DialogueCharacter character = nodeData.Character;
            Sprite sprite = nodeData.Character.GetCharacterSprite(nodeData.CharacterExpression);
            string characterName = character.characterName.Find(text => text.languageEnum == localization).LanguageGenericType;
            Vector3 pos = nodeData.CharacterSpritePos;
            Vector3 size = Vector3.one * nodeData.CharacterSpriteSize;

            float Duration = nodeData.Duration;

            return new CharacterScriptDTO(
                scriptId: nodeData.NodeGuid,
                characterName: characterName,
                characterSprite: sprite,
                characterEffectType: nodeData.CharacterEffect,
                spritePos: pos,
                spriteSize: size,
                effectWeight: nodeData.EffectWeight,
                duration: Duration
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
}

