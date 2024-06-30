using MeetAndTalk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;

namespace VisualNovelGame
{
    public class ScenarioManagers : MonoBehaviour
    {

        public DialogueContainerSO dialogueContainerSO;


        public void ScenarioSave()
        {
            ConvertData();
        }

        private void ConvertData()
        {
            CheckNodeType(GetNextNode(dialogueContainerSO, dialogueContainerSO.StartNodeDatas[0]));



        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {
            // ��� Ÿ�Կ� ���� ������ �Լ� ����
            switch (_baseNodeData) {
                case StartNodeData nodeData:
                    break;
                case DialogueNodeData nodeData:
                    break;
                case DialogueChoiceNodeData nodeData:
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




        // ��ũ��Ʈ�� ����(��ư)

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

