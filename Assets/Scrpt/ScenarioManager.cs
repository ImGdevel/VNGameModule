using DialogueSystem;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;
using DialogueSystem.Localization;

namespace VisualNovelGame
{
    public class ScenarioManagers : MonoBehaviour
    {
        public DialogueScript dialogueContainerSO;
        //public LocalizationManager localizationManager;

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
            //Debug.Log("Content");

            //Debug.Log(dialogueContainerSO.StartNodeDatas.Count);

            //BaseNodeData startNode = GetNextNode(dialogueContainerSO, dialogueContainerSO.StartNodeDatas[0]);

            

            //BaseNodeData node = GetNextNode(dialogueContainerSO, startNode);
            //CheckNodeType(node);
            //BaseNodeData node2 = GetNextNode(dialogueContainerSO, node);
            //CheckNodeType(node2);

            //BaseNodeData node3 = GetNextNode(dialogueContainerSO, node2);
            //CheckNodeType(node3);

            //BaseNodeData node4 = GetNextNode(dialogueContainerSO, node3);
            //CheckNodeType(node4);



        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {


        }



        public BaseNodeData GetNodeByGuid(DialogueScript dialogueContainer, string _targetNodeGuid)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
        }

        public BaseNodeData GetNodeByNodePort(DialogueScript dialogueContainer, DialogueNodePort _nodePort)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
        }

        public BaseNodeData GetNextNode(DialogueScript dialogueContainer, BaseNodeData _baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueContainer.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == _baseNodeData.NodeGuid);

            return GetNodeByGuid(dialogueContainer, nodeLinkData.TargetNodeGuid);
        }




        // 스크립트로 저장(버튼)

    }


    class Mapper
    {

        public static BaseNodeData GetNodeByGuid(DialogueScript dialogueContainer, string _targetNodeGuid)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
        }

        public static BaseNodeData GetNodeByNodePort(DialogueScript dialogueContainer, DialogueNodePort _nodePort)
        {
            return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
        }

        public static BaseNodeData GetNextNode(DialogueScript dialogueContainer, BaseNodeData _baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueContainer.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == _baseNodeData.NodeGuid);

            return GetNodeByGuid(dialogueContainer, nodeLinkData.TargetNodeGuid);
        }
    }
}

