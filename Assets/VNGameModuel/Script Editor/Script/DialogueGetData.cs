using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueGetData : MonoBehaviour
    {
        public DialogueScript dialogueScript;

        protected BaseNodeData GetNodeByGuid(string _targetNodeGuid)
        {
            return dialogueScript.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
        }

        protected BaseNodeData GetNodeByNodePort(DialogueNodePort _nodePort)
        {
            return dialogueScript.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
        }

        protected BaseNodeData GetNextNode(BaseNodeData _baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueScript.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == _baseNodeData.NodeGuid);

            return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
        }
    }
}