using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using DialogueSystem.Editor;
using DialogueSystem.Localization;
using DialogueSystem.Event;

namespace DialogueSystem.Nodes
{
    public class BackgroundNode : BaseNode
    {
        private Sprite backgroundSprite = null;

        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        public Sprite BackgroundSprite { get => backgroundSprite; set => backgroundSprite = value; }

        private TextField texts_Field;
        private ObjectField audioClips_Field;
        private ObjectField character_Field;
        private Foldout characterFoldout;

        public BackgroundNode()
        {
        }

        public BackgroundNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Dialogue";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            AddValidationContainer();



            /* TEXT BOX */


            /* BackgroundSprite CLIPS */
            Label label_character = new Label("BackgroundSprite SO");
            label_character.AddToClassList("label_name");
            label_character.AddToClassList("Label");
            mainContainer.Add(label_character);
            character_Field = new ObjectField() {
                objectType = typeof(Sprite),
                allowSceneObjects = false,
            };
            character_Field.RegisterValueChangedCallback(value => {
                backgroundSprite = value.newValue as Sprite;
            });
            character_Field.SetValueWithoutNotify(backgroundSprite);
            mainContainer.Add(character_Field);

        }


        /// <summary>
        /// 언어 설정 불러오기
        /// </summary>
        public void ReloadLanguage()
        {
        }

        /// <summary>
        /// 필드 복구
        /// </summary>
        public override void LoadValueInToField()
        {
            character_Field.SetValueWithoutNotify(backgroundSprite);
        }

        /// <summary>
        /// 노트 유효성 에러체크 및 경고
        /// </summary>
        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port input = inputContainer.Query<Port>().First();
            if (!input.connected) warning.Add("Node cannot be called");

            Port output = outputContainer.Query<Port>().First();
            if (!output.connected) error.Add("Output does not lead to any node");


            ErrorList = error;
            WarningList = warning;

        }
    }
}
