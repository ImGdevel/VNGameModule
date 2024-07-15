using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using DialogueSystem.Nodes;

namespace DialogueSystem.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueEditorWindow editorWindow; // ��ȭ ������ â�� �����մϴ�.
        private DialogueGraphView graphView; // ��ȭ �׷��� �並 �����մϴ�.

        // ���� �޼���: ������ â�� �׷��� �並 �ʱ�ȭ�մϴ�.
        public void Configure(DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;
        }

        // �˻� Ʈ���� �����մϴ�.
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // �˻� Ʈ�� ��Ʈ�� ����Ʈ�� �����մϴ�.
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 0), // �ֻ��� �׷� ��Ʈ��
                new SearchTreeGroupEntry(new GUIContent("Dialogue", EditorGUIUtility.FindTexture("d_FilterByType")), 1), // �� ��° �׷� ��Ʈ��

                // �پ��� ��带 �׷쿡 �߰��մϴ�.
                AddNodeSearchToGroup("Start Node", new StartNode(), "d_Animation.Play"),
                AddNodeSearchToGroup("Dialogue Node", new DialogueNode(), "d_UnityEditor.HierarchyWindow"),
                AddNodeSearchToGroup("Choice Node", new DialogueChoiceNode(), "d_TreeEditor.Distribution"),
                AddNodeSearchToGroup("End Node", new EndNode(), "d_winbtn_win_close_a@2x")
            };

            return tree;
        }

        // ��带 �׷쿡 �߰��ϴ� ���� �޼���
        private SearchTreeEntry AddNodeSearchToGroup(string _name, BaseNode _baseNode, string IconName)
        {
            Texture2D _icon = EditorGUIUtility.FindTexture(IconName) as Texture2D;
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(_name, _icon)) {
                level = 2, // �� ��° ����
                userData = _baseNode // ����� �����͸� ���� ����
            };

            return tmp;
        }

        // ��带 �߰��ϴ� ���� �޼���
        private SearchTreeEntry AddNodeSearch(string _name, BaseNode _baseNode, string IconName)
        {
            Texture2D _icon = EditorGUIUtility.FindTexture(IconName) as Texture2D;
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(_name, _icon)) {
                level = 1, // ù ��° ����
                userData = _baseNode // ����� �����͸� ���� ����
            };

            return tmp;
        }

        // ��Ʈ�� ���� �� ȣ��Ǵ� �޼���
        public bool OnSelectEntry(SearchTreeEntry _searchTreeEntry, SearchWindowContext _context)
        {
            // ���콺 ��ġ�� �׷��� ���� ��ǥ�� ��ȯ
            Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent,
                _context.screenMousePosition - editorWindow.position.position
            );
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

            return CheckForNodeType(_searchTreeEntry, graphMousePosition);
        }

        // ��� Ÿ�Կ� ���� �׷��� �信 ��带 �߰��ϴ� �޼���
        private bool CheckForNodeType(SearchTreeEntry _searchTreeEntry, Vector2 _pos)
        {
            switch (_searchTreeEntry.userData) {
                case StartNode node:
                    graphView.AddElement(graphView.CreateStartNode(_pos));
                    return true;
                case DialogueNode node:
                    graphView.AddElement(graphView.CreateDialogueNode(_pos));
                    return true;
                case DialogueChoiceNode node:
                    graphView.AddElement(graphView.CreateDialogueChoiceNode(_pos));
                    return true;
                case EndNode node:
                    graphView.AddElement(graphView.CreateEndNode(_pos));
                    return true;
            }
            return false;
        }
    }
}
