using MeetAndTalk;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;
using MeetAndTalk.Localization;

namespace VisualNovelGame
{
    public class ScenarioManagers : MonoBehaviour
    {
        public DialogueContainerSO dialogueContainerSO;
        public LocalizationManager localizationManager;

        private Dictionary<int, Scenario> scenarios;
        private DataManager<Dictionary<int, Scenario>> scenariosDataManager;


        private void Start()
        {
            ConvertData();
        }

        public void ScenarioSave()
        {
            ConvertData();
        }

        private void ConvertData()
        {
            Debug.Log("Content");

            Debug.Log(dialogueContainerSO.StartNodeDatas.Count);

            BaseNodeData startNode = GetNextNode(dialogueContainerSO, dialogueContainerSO.StartNodeDatas[0]);

            

            BaseNodeData node = GetNextNode(dialogueContainerSO, startNode);
            CheckNodeType(node);
            BaseNodeData node2 = GetNextNode(dialogueContainerSO, node);
            CheckNodeType(node2);

            BaseNodeData node3 = GetNextNode(dialogueContainerSO, node2);
            CheckNodeType(node3);

            BaseNodeData node4 = GetNextNode(dialogueContainerSO, node3);
            CheckNodeType(node4);



        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {

            // 노드 타입에 따라 실행할 함수 선택

            Debug.Log("Current Node:" + _baseNodeData.NodeGuid);

            switch (_baseNodeData) {
                case StartNodeData nodeData:

                    break;
                case DialogueNodeData nodeData:
                    Debug.Log("Current Text:" + $"{nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}");
                    Debug.Log("Next Node:" + $"{nodeData.DialogueNodePorts.Count}");

                    break;
                case DialogueChoiceNodeData nodeData:
                    Debug.Log("Current Node:" + $"{nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}");
                    Debug.Log("Next Node:" + $"{nodeData.DialogueNodePorts[0].InputGuid}");
                    Debug.Log("Next Node:" + $"{nodeData.DialogueNodePorts[1].InputGuid}");
                    Debug.Log("Next Node:" + $"{nodeData.DialogueNodePorts[2].InputGuid}");

                    break;
                case EndNodeData nodeData:
                    break;
                default:
                    break;
            }
        }



        public BaseNodeData GetNodeByGuid(DialogueContainerSO dialogueContainer, string _targetNodeGuid)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
        }

        public BaseNodeData GetNodeByNodePort(DialogueContainerSO dialogueContainer, DialogueNodePort _nodePort)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
        }

        public BaseNodeData GetNextNode(DialogueContainerSO dialogueContainer, BaseNodeData _baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueContainer.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == _baseNodeData.NodeGuid);

            return GetNodeByGuid(dialogueContainer, nodeLinkData.TargetNodeGuid);
        }




        // 스크립트로 저장(버튼)

    }


    class Mapper
    {

        public static BaseNodeData GetNodeByGuid(DialogueContainerSO dialogueContainer, string _targetNodeGuid)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
        }

        public static BaseNodeData GetNodeByNodePort(DialogueContainerSO dialogueContainer, DialogueNodePort _nodePort)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
        }

        public static BaseNodeData GetNextNode(DialogueContainerSO dialogueContainer, BaseNodeData _baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueContainer.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == _baseNodeData.NodeGuid);

            return GetNodeByGuid(dialogueContainer, nodeLinkData.TargetNodeGuid);
        }
    }
}

