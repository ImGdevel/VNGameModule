using DialogueSystem;
using DialogueSystem.Localization;
using System.Collections.Generic;
using UnityEditor.Localization.Editor;
using UnityEngine;
using VisualNovelGame.Data;

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

                case CharacterNodeData nodeData:
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
                    Debug.LogWarning("Not found data type");
                    return null;
            }
        }
    }
}
