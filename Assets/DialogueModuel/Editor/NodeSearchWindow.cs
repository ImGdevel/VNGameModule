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
        private DialogueEditorWindow editorWindow; // 대화 편집기 창을 참조합니다.
        private DialogueGraphView graphView; // 대화 그래프 뷰를 참조합니다.

        // 설정 메서드: 에디터 창과 그래프 뷰를 초기화합니다.
        public void Configure(DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;
        }

        // 검색 트리를 생성합니다.
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // 검색 트리 엔트리 리스트를 생성합니다.
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node", EditorGUIUtility.FindTexture("d_FilterByType")), 0), // 두 번째 그룹 엔트리

                // 다양한 노드를 그룹에 추가합니다.
                AddNodeSearchToGroup("Start Node", new StartNode(), "d_Animation.Play"),
                AddNodeSearchToGroup("Dialogue Node", new DialogueNode(), "d_UnityEditor.HierarchyWindow"),
                AddNodeSearchToGroup("Choice Node", new DialogueChoiceNode(), "d_TreeEditor.Distribution"),
                AddNodeSearchToGroup("End Node", new EndNode(), "d_winbtn_win_close_a@2x")
            };

            return tree;
        }

        // 노드를 그룹에 추가하는 헬퍼 메서드
        private SearchTreeEntry AddNodeSearchToGroup(string _name, BaseNode _baseNode, string IconName)
        {
            Texture2D _icon = EditorGUIUtility.FindTexture(IconName) as Texture2D;
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(_name, _icon)) {
                level = 1, // 두 번째 레벨
                userData = _baseNode // 사용자 데이터를 노드로 설정
            };

            return tmp;
        }

        // 노드를 추가하는 헬퍼 메서드
        private SearchTreeEntry AddNodeSearch(string _name, BaseNode _baseNode, string IconName)
        {
            Texture2D _icon = EditorGUIUtility.FindTexture(IconName) as Texture2D;
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(_name, _icon)) {
                level = 0, // 첫 번째 레벨
                userData = _baseNode // 사용자 데이터를 노드로 설정
            };

            return tmp;
        }

        // 엔트리 선택 시 호출되는 메서드
        public bool OnSelectEntry(SearchTreeEntry _searchTreeEntry, SearchWindowContext _context)
        {
            // 마우스 위치를 그래프 뷰의 좌표로 변환
            Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent,
                _context.screenMousePosition - editorWindow.position.position
            );
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

            return CheckForNodeType(_searchTreeEntry, graphMousePosition);
        }

        // 노드 타입에 따라 그래프 뷰에 노드를 추가하는 메서드
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
