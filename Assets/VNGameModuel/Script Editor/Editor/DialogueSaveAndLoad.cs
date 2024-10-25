using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using DialogueSystem.Nodes;
using VisualNovelGame;

namespace DialogueSystem.Editor
{
#if UNITY_EDITOR

    public class DialogueSaveAndLoad
    {
        private List<Edge> edges => graphView.edges.ToList();
        private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

        public DialogueGraphView graphView;

        public DialogueSaveAndLoad(DialogueGraphView _graphView)
        {
            graphView = _graphView;
        }

        /// <summary>
        /// 저장
        /// </summary>
        /// <param name="_dialogueScript">스크립트</param>
        public void Save(DialogueScript _dialogueScript)
        {
            SaveEdges(_dialogueScript);
            SaveNodes(_dialogueScript);

            EditorUtility.SetDirty(_dialogueScript);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 불러오기
        /// </summary>
        /// <param name="_dialogueScript">스크립트</param>
        public void Load(DialogueScript _dialogueScript)
        {
            ClearGraph();
            GenerateNodes(_dialogueScript);
            ConnectNodes(_dialogueScript);
        }

        #region Save

        public void SaveEdges(DialogueScript _dialogueScript)
        {
            _dialogueScript.NodeLinkDatas.Clear();

            Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
                BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

                _dialogueScript.NodeLinkDatas.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.nodeGuid,
                    TargetNodeGuid = inputNode.nodeGuid
                });
            }
        }

        /// <summary>
        /// 노드 저장
        /// </summary>
        /// <param name="_dialogueScript"></param>
        private void SaveNodes(DialogueScript _dialogueScript)
        {
            _dialogueScript.DialogueChoiceNodeDatas.Clear();
            _dialogueScript.DialogueNodeDatas.Clear();
            _dialogueScript.TimerChoiceNodeDatas.Clear();
            _dialogueScript.EventNodeDatas.Clear();
            _dialogueScript.EndNodeDatas.Clear();
            _dialogueScript.StartNodeDatas.Clear();
            _dialogueScript.RandomNodeDatas.Clear();
            _dialogueScript.CommandNodeDatas.Clear();
            _dialogueScript.CharacterNodeDatas.Clear();
            _dialogueScript.IfNodeDatas.Clear();
            _dialogueScript.backgroundDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueChoiceNode dialogueChoiceNode:
                        _dialogueScript.DialogueChoiceNodeDatas.Add(SaveNodeData(dialogueChoiceNode));
                        break;
                    case DialogueNode dialogueNode:
                        _dialogueScript.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case TimerChoiceNode timerChoiceNode:
                        _dialogueScript.TimerChoiceNodeDatas.Add(SaveNodeData(timerChoiceNode));
                        break;
                    case CharacterNode characterNode:
                        _dialogueScript.CharacterNodeDatas.Add(SaveNodeData(characterNode));
                        break;
                    case StartNode startNode:
                        _dialogueScript.StartNodeDatas.Add(SaveNodeData(startNode));
                        break;
                    case EndNode endNode:
                        _dialogueScript.EndNodeDatas.Add(SaveNodeData(endNode));
                        break;
                    case EventNode eventNode:
                        _dialogueScript.EventNodeDatas.Add(SaveNodeData(eventNode));
                        break;
                    case RandomNode randomNode:
                        _dialogueScript.RandomNodeDatas.Add(SaveNodeData(randomNode));
                        break;
                    case IFNode ifNode:
                        _dialogueScript.IfNodeDatas.Add(SaveNodeData(ifNode));
                        break;
                    case BackgroundNode backgroundNode:
                        _dialogueScript.backgroundDatas.Add(SaveNodeData(backgroundNode));
                        break;
                    default:
                        break;
                }
            });
        }

        private ChoiceNodeData SaveNodeData(DialogueChoiceNode _node)
        {
            ChoiceNodeData nodeData = new ChoiceNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                TextType = _node.Texts,
                Character = _node.Character,
                AvatarPos = _node.characterPosition,
                AvatarType = _node.characterType,
                AudioClips = _node.AudioClip,
                DialogueNodePorts = _node.dialogueNodePorts,
                Duration = _node.DurationShow
            };
            foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output == nodePort.MyPort)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).nodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).nodeGuid;
                    }
                }
            }


            return nodeData;
        }

        private RandomNodeData SaveNodeData(RandomNode _node)
        {
            RandomNodeData nodeData = new RandomNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                DialogueNodePorts = _node.dialogueNodePorts,
            };
            foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output == nodePort.MyPort)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).nodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).nodeGuid;
                    }
                }
            }


            return nodeData;
        }

        private TimerChoiceNodeData SaveNodeData(TimerChoiceNode _node)
        {
            TimerChoiceNodeData nodeData = new TimerChoiceNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                TextType = _node.Texts,
                Character = _node.Character,
                AvatarPos = _node.characterPosition,
                AvatarType = _node.characterType,
                AudioClips = _node.AudioClip,
                time = _node.ChoiceTime,
                DialogueNodePorts = _node.dialogueNodePorts,
                Duration = _node.DurationShow
            };
            foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output == nodePort.MyPort)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).nodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).nodeGuid;
                    }
                }
            }

            return nodeData;
        }

        private StartNodeData SaveNodeData(StartNode _node)
        {
            StartNodeData nodeData = new StartNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                startID = _node.startID
            };
            return nodeData;
        }

        private EndNodeData SaveNodeData(EndNode _node)
        {
            EndNodeData nodeData = new EndNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                EndNodeType = _node.EndNodeType,
                Dialogue = _node.dialogue
            };
            return nodeData;
        }

        private EventNodeData SaveNodeData(EventNode _node)
        {
            EventNodeData nodeData = new EventNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                EventScriptableObjects = _node.EventScriptableObjectDatas
            };
            return nodeData;
        }

        private DialogueNodeData SaveNodeData(DialogueNode _node)
        {
            DialogueNodeData nodeData = new DialogueNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                TextType = _node.Texts,
                Character = _node.Character,
                AudioClips = _node.AudioClip,
                DialogueNodePorts = _node.dialogueNodePorts,
            };

            return nodeData;
        }

        private CharacterNodeData SaveNodeData(CharacterNode _node)
        {
            

            CharacterNodeData nodeData = new CharacterNodeData {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                Character = _node.Character,
                CharacterExpression = _node.CharacterExpression,
                CharacterSpritePos = _node.CharacterSpritePos,
                CharacterSpriteSize = _node.CharacterSpriteSize,
                CharacterEffect = _node.CharacterEffect,
                Duration = _node.DurationShow,
                DialogueNodePorts = _node.dialogueNodePorts,
            };
            
            return nodeData;
        }

        private BackgroundNodeData SaveNodeData(BackgroundNode _node)
        {
            BackgroundNodeData nodeData = new BackgroundNodeData {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                backgroundSprite = _node.BackgroundSprite
            };

            return nodeData;
        }


        private IfNodeData SaveNodeData(IFNode _node)
        {
            IfNodeData nodeData = new IfNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,

                ValueName = _node.ValueName,
                Operations = _node.Operations,
                OperationValue = _node.OperationValue

            };

            List<string> tmp = new List<string>();

            foreach (Edge edge in edges)
            {
                if ((edge.output.node as BaseNode).nodeGuid == _node.nodeGuid)
                {
                    tmp.Add((edge.input.node as BaseNode).nodeGuid);
                }
            }

            if(tmp.Count > 0) { nodeData.TrueGUID = tmp[0]; }
            if(tmp.Count > 1) { nodeData.FalseGUID = tmp[1]; }

            return nodeData;
        }
        #endregion

        #region Load

        /// <summary>
        /// 에디터 그래프 클리어
        /// </summary>
        private void ClearGraph()
        {
            edges.ForEach(edge => graphView.RemoveElement(edge));

            foreach (BaseNode node in nodes)
            {
                graphView.RemoveElement(node);
            }
        }

        /// <summary>
        /// 노드 생성
        /// </summary>
        /// <param name="_dialogueContainer"></param>
        private void GenerateNodes(DialogueScript _dialogueContainer)
        {
            /* Start Node */
            foreach (StartNodeData node in _dialogueContainer.StartNodeDatas)
            {
                StartNode tempNode = graphView.CreateStartNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;
                tempNode.startID = node.startID;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* End Node */
            foreach (EndNodeData node in _dialogueContainer.EndNodeDatas)
            {
                EndNode tempNode = graphView.CreateEndNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;
                tempNode.EndNodeType = node.EndNodeType;
                tempNode.dialogue = node.Dialogue;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Event Node */
            foreach (EventNodeData node in _dialogueContainer.EventNodeDatas)
            {
                EventNode tempNode = graphView.CreateEventNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (EventScriptableObjectData item in node.EventScriptableObjects)
                {
                    tempNode.AddScriptableEvent(item);
                    //tempNode.GenerateFields(item);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* IF Node */
            foreach (IfNodeData node in _dialogueContainer.IfNodeDatas)
            {
                IFNode tempNode = graphView.CreateIFNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                tempNode.ValueName = node.ValueName;
                tempNode.Operations = node.Operations;
                tempNode.OperationValue = node.OperationValue;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Dialogue Choice Node */
            foreach (ChoiceNodeData node in _dialogueContainer.DialogueChoiceNodeDatas)
            {
                DialogueChoiceNode tempNode = graphView.CreateDialogueChoiceNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;


                foreach (LanguageGeneric<string> languageGeneric in node.TextType)
                {
                    tempNode.Texts.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClip.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddChoicePort(tempNode, nodePort);
                }
                tempNode.Character = node.Character;
                tempNode.characterPosition = node.AvatarPos;
                tempNode.characterType = node.AvatarType;
                tempNode.DurationShow = node.Duration;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Random Note */
            foreach (RandomNodeData node in _dialogueContainer.RandomNodeDatas)
            {
                RandomNode tempNode = graphView.CreateRandomNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddOutputPort(tempNode, nodePort);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }



            /* Timer Choice Node */
            foreach (TimerChoiceNodeData node in _dialogueContainer.TimerChoiceNodeDatas)
            {
                TimerChoiceNode tempNode = graphView.CreateTimerChoiceNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (LanguageGeneric<string> languageGeneric in node.TextType)
                {
                    tempNode.Texts.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClip.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
                tempNode.Character = node.Character;
                tempNode.characterPosition = node.AvatarPos;
                tempNode.characterType = node.AvatarType;
                tempNode.DurationShow = node.Duration;
                tempNode.ChoiceTime = node.time;

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddChoicePort(tempNode, nodePort);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Dialogue Node */
            foreach (DialogueNodeData node in _dialogueContainer.DialogueNodeDatas)
            {
                DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (LanguageGeneric<string> languageGeneric in node.TextType)
                {
                    tempNode.Texts.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClip.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                tempNode.Character = node.Character;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Character Node */
            foreach (CharacterNodeData node in _dialogueContainer.CharacterNodeDatas) {
                CharacterNode tempNode = graphView.CreateCharacterNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                tempNode.Character = node.Character;
                tempNode.CharacterExpression = node.CharacterExpression;
                tempNode.CharacterSpritePos = node.CharacterSpritePos;
                tempNode.CharacterSpriteSize = node.CharacterSpriteSize;
                tempNode.CharacterEffect = node.CharacterEffect;
                tempNode.DurationShow = node.Duration;

                tempNode.LoadValueInToField();

                graphView.AddElement(tempNode);
            }

            /* Background Sprite Node */
            foreach (BackgroundNodeData node in _dialogueContainer.backgroundDatas) {
                BackgroundNode tempNode = graphView.CreateBackgroundNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                tempNode.BackgroundSprite = node.backgroundSprite;

                tempNode.LoadValueInToField();

                graphView.AddElement(tempNode);
            }


        }

        private void ConnectNodes(DialogueScript _dialogueContainer)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                List<NodeLinkData> connections = _dialogueContainer.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].nodeGuid).ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    BaseNode targetNode = nodes.First(node => node.nodeGuid == targetNodeGuid);

                    if ((nodes[i] is DialogueChoiceNode) == false)
                    {
                        LinkNodesTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                    }
                }
            }

            List<DialogueChoiceNode> dialogueNodes = nodes.FindAll(node => node is DialogueChoiceNode).Cast<DialogueChoiceNode>().ToList();

            foreach (DialogueChoiceNode dialogueNode in dialogueNodes)
            {
                foreach (DialogueNodePort nodePort in dialogueNode.dialogueNodePorts)
                {
                    if (nodePort.InputGuid != string.Empty)
                    {
                        BaseNode targetNode = nodes.First(Node => Node.nodeGuid == nodePort.InputGuid);
                        LinkNodesTogether(nodePort.MyPort, (Port)targetNode.inputContainer[0]);
                    }
                }
            }
        }

        private void LinkNodesTogether(Port _outputPort, Port _inputPort)
        {
            Edge tempEdge = new Edge()
            {
                output = _outputPort,
                input = _inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            graphView.Add(tempEdge);
        }

        #endregion
    }

#endif
}
