using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using DialogueSystem.Editor;
using DialogueSystem.GlobalValue;
using DialogueSystem.Event;
using System.Reflection;
using UnityEditor;
using System.IO;

namespace DialogueSystem.Nodes
{
    public class EventNode : BaseNode
    {
        // 이벤트 데이터를 저장하는 리스트
        private List<EventScriptableObjectData> eventScriptableObjectDatas = new List<EventScriptableObjectData>();

        // 이벤트 데이터 리스트의 접근자를 위한 프로퍼티
        public List<EventScriptableObjectData> EventScriptableObjectDatas { get => eventScriptableObjectDatas; set => eventScriptableObjectDatas = value; }

        private ToolbarMenu addEventButton; // 버튼을 저장할 필드

        // 기본 생성자
        public EventNode()
        {
        }

        // 위치, 에디터 창, 그래프 뷰를 매개변수로 받는 생성자
        public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Event";  // 노드 제목 설정
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            AddValidationContainer();

            addEventButton = CreateButton();
            mainContainer.AddToClassList("mainContainer");
            mainContainer.Add(addEventButton);
            
        }

        public override void LoadValueInToField()
        {
        }

        private ToolbarMenu CreateButton()
        {
            ToolbarMenu button = new ToolbarMenu();
            button.AddToClassList("AddEventBtn");
            button.text = "Add Event";

            button.menu.AppendAction("Empty Field", new Action<DropdownMenuAction>(x => AddScriptableEvent()));
            button.menu.AppendSeparator();

            List<Type> subclasses = SubclassFinder.GetSubclasses<DialogueEvent>();
            for (int i = 1; i < subclasses.Count; i++) {
                int index = i;
                button.menu.AppendAction($"New {subclasses[i].Name}", new Action<DropdownMenuAction>(x => AddNewEvent(subclasses[index])));
            }

            return button;
        }

        public void AddNewEvent(Type type)
        {
            // 새로운 ScriptableObject 생성
            ScriptableObject newObject = ScriptableObject.CreateInstance(type);

            // 경로 구성
            string directoryPath = "Assets/Dialgue System/Events";
            string fileName = $"{type.Name}_{UnityEngine.Random.Range(0, 100000)}.asset";
            string path = Path.Combine(directoryPath, fileName);

            // 디렉토리 존재 확인 및 생성
            if (!Directory.Exists(directoryPath)) {
                try {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception e) {
                    Debug.LogError($"Failed to create directory: {e.Message}");
                    return;
                }
            }
            // 에셋 생성 및 저장
            try {
                AssetDatabase.CreateAsset(newObject, path);  // 에셋으로 저장
                AssetDatabase.SaveAssets();
            }
            catch (Exception e) {
                Debug.LogError($"Failed to create asset: {e.Message}");
                return;
            }

            // 리스트에 추가
            EventScriptableObjectData tmp = new EventScriptableObjectData();
            tmp.DialogueEvent = (DialogueEvent)newObject;
            AddScriptableEvent(tmp);
        }

        public void AddScriptableEvent(EventScriptableObjectData paramidaEventScriptableObjectData = null)
        {
            EventScriptableObjectData tmpDialogueEventSO = new EventScriptableObjectData();
            if (paramidaEventScriptableObjectData != null) {
                tmpDialogueEventSO.DialogueEvent = paramidaEventScriptableObjectData.DialogueEvent;
            }
            eventScriptableObjectDatas.Add(tmpDialogueEventSO);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            Box headerBox = new Box();
            headerBox.AddToClassList("HeaderBox");
            headerBox.style.flexDirection = FlexDirection.Column;

            Box topRowBox = new Box();
            topRowBox.AddToClassList("TopRowBox");
            topRowBox.style.flexDirection = FlexDirection.Row;
            topRowBox.style.alignItems = Align.Center;

            Label titleField = new Label("Default Event");
            titleField.AddToClassList("label_texts");
            titleField.AddToClassList("EventTitle");
            titleField.style.flexGrow = 1;
            topRowBox.Add(titleField);



            Box bottomRowBox = new Box();
            bottomRowBox.AddToClassList("BottomRowBox");
            bottomRowBox.style.flexDirection = FlexDirection.Row;

            ObjectField objectField = new ObjectField() {
                objectType = typeof(DialogueEvent),
                allowSceneObjects = false,
                value = null,
            };
            objectField.AddToClassList("EventSO");
            bottomRowBox.Add(objectField);

            headerBox.Add(topRowBox);
            headerBox.Add(bottomRowBox);

            boxContainer.Add(headerBox);

            Box ValueBox = new Box();
            ValueBox.name = UnityEngine.Random.Range(1, 999999999).ToString();

            objectField.RegisterValueChangedCallback(value => {
                tmpDialogueEventSO.DialogueEvent = value.newValue as DialogueEvent;

                if (tmpDialogueEventSO.DialogueEvent != null) {
                    titleField.text = tmpDialogueEventSO.DialogueEvent.name;
                }
                else {
                    titleField.text = "Default Event";
                }

                if (mainContainer.Children().OfType<VisualElement>().Any(child => child.name == ValueBox.name)) {
                    mainContainer.RemoveAt(mainContainer.IndexOf(ValueBox));
                }
                eventScriptableObjectDatas.RemoveAt(eventScriptableObjectDatas.IndexOf(tmpDialogueEventSO));
                AddScriptableEvent(tmpDialogueEventSO);
                mainContainer.RemoveAt(mainContainer.IndexOf(boxContainer));
            });
            objectField.SetValueWithoutNotify(tmpDialogueEventSO.DialogueEvent);

            Box contentBox = new Box();
            contentBox.AddToClassList("ContentBox");
            contentBox.Add(ValueBox);
            boxContainer.Add(contentBox);

            mainContainer.Add(boxContainer);

            GenerateFields(ValueBox, tmpDialogueEventSO);

            VisualElement line = new VisualElement();
            line.style.height = 1; 
            line.style.backgroundColor = new Color(0, 0f, 0f, 0.9f); 
            line.style.marginTop = 3; 
            line.style.marginBottom = 0;   
            mainContainer.Add(line);


            Button btn = new Button() {
                text = "X",
            };
            btn.clicked += () => {
                DeleteBox(boxContainer);
                DeleteBox(ValueBox);
                mainContainer.Remove(line);
                eventScriptableObjectDatas.Remove(tmpDialogueEventSO);
            };
            btn.AddToClassList("EventBtn");
            topRowBox.Add(btn);


            if (addEventButton != null) {
                mainContainer.Remove(addEventButton);
            }
            mainContainer.Add(addEventButton);

            RefreshExpandedState();
        }

        // 박스를 삭제하는 메서드
        private void DeleteBox(Box boxContainer)
        {
            if (boxContainer != null && mainContainer.Contains(boxContainer)) {
                mainContainer.Remove(boxContainer);
            }
            
        }

        // 필드를 생성하는 메서드
        public void GenerateFields(VisualElement ValueBox, EventScriptableObjectData paramidaEventScriptableObjectData = null)
        {

            if (paramidaEventScriptableObjectData != null) {
                if (paramidaEventScriptableObjectData.DialogueEvent != null) {
                    Type scriptableObjectType = paramidaEventScriptableObjectData.DialogueEvent.GetType();
                    FieldInfo[] fields = scriptableObjectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    for (int i = 0; i < fields.Length; i++) {
                        int index = i;
                        Box boxContainer2 = new Box();
                        boxContainer2.AddToClassList("EventBox");

                        // 필드 타입에 따라 다른 UI 요소 생성
                        if (fields[i].FieldType == typeof(int) && fields[i].IsPublic) {
                            IntegerField objectField2 = new IntegerField() {
                                value = (int)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(float) && fields[i].IsPublic) {
                            FloatField objectField2 = new FloatField() {
                                value = (float)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(string) && fields[i].IsPublic) {
                            TextField objectField2 = new TextField() {
                                value = (string)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(bool) && fields[i].IsPublic) {
                            Toggle objectField2 = new Toggle() {
                                value = (bool)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector2) && fields[i].IsPublic) {
                            Vector2Field objectField2 = new Vector2Field() {
                                value = (Vector2)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector3) && fields[i].IsPublic) {
                            Vector3Field objectField2 = new Vector3Field() {
                                value = (Vector3)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector4) && fields[i].IsPublic) {
                            Vector4Field objectField2 = new Vector4Field() {
                                value = (Vector4)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }

                        else if (fields[i].FieldType.IsEnum && fields[i].IsPublic) {
                            Enum enumValue = (Enum)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent);
                            EnumField objectField2 = new EnumField(enumValue) {
                                value = (Enum)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType.IsSubclassOf(typeof(UnityEngine.Object)) && fields[i].IsPublic) {
                            ObjectField objectField2 = new ObjectField() {
                                objectType = fields[i].FieldType,
                                allowSceneObjects = false,
                                value = (UnityEngine.Object)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");
                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);
                            boxContainer2.Add(objectField2);

                        }
                        else if (fields[i].FieldType == typeof(Color) && fields[i].IsPublic) {
                            ColorField objectField2 = new ColorField() {
                                value = (Color)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Gradient) && fields[i].IsPublic) {
                            GradientField objectField2 = new GradientField() {
                                value = (Gradient)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(AnimationCurve) && fields[i].IsPublic) {
                            CurveField objectField2 = new CurveField() {
                                value = (AnimationCurve)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector2Int) && fields[i].IsPublic) {
                            Vector2IntField objectField2 = new Vector2IntField() {
                                value = (Vector2Int)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector3Int) && fields[i].IsPublic) {
                            Vector3IntField objectField2 = new Vector3IntField() {
                                value = (Vector3Int)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEvent, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEvent);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(GlobalValueOperationClass) && fields[i].IsPublic) {
                            GlobalValueOperationClass globalValueOperation = (GlobalValueOperationClass)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEvent);


                            List<string> valueNames = new List<string>();
                            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
                            manager.LoadFile();
                            for (int x = 0; x < manager.IntValues.Count; x++) { valueNames.Add(manager.IntValues[x].ValueName); }
                            for (int x = 0; x < manager.FloatValues.Count; x++) { valueNames.Add(manager.FloatValues[x].ValueName); }
                            for (int x = 0; x < manager.BoolValues.Count; x++) { valueNames.Add(manager.BoolValues[x].ValueName); }
                            int PopupIndex = 0;
                            for (int x = 0; x < valueNames.Count; x++) { if (valueNames[x] == globalValueOperation.ValueName) { PopupIndex = x; } }

                            PopupField<string> ValueNameField = new PopupField<string>("Value Name") {
                                choices = valueNames,
                                index = PopupIndex
                            };
                            ValueNameField.AddToClassList("EventSO");
                            ValueNameField.RegisterValueChangedCallback(x => {
                                globalValueOperation.ValueName = x.newValue;
                                Debug.Log(x.newValue);
                                fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, ValueNameField.value);
                            });

                            //bool isBool = false;
                            //for (int x = 0; x < manager.BoolValues.Count; x++) { if (ValueNameField.value == manager.BoolValues[x].ValueName) { isBool = true; } }

                            Enum enumValue = (Enum)globalValueOperation.Operation;
                            EnumField OperationField = new EnumField("Operation", enumValue) {
                                value = (Enum)globalValueOperation.Operation
                            };
                            OperationField.AddToClassList("EventSO");
                            OperationField.RegisterValueChangedCallback(x => {
                                globalValueOperation.Operation = (GlobalValueOperations)x.newValue;
                                fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, OperationField.value);
                            });


                            TextField OperationValueField = new TextField() {
                                value = globalValueOperation != null ? globalValueOperation.OperationValue : "",
                                label = "Operation Value"
                            };
                            OperationValueField.AddToClassList("EventSO");
                            OperationValueField.RegisterValueChangedCallback(x => {
                                globalValueOperation.OperationValue = x.newValue;
                                fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEvent, OperationValueField.value);
                            });

                            Box boxContainer = new Box();
                            boxContainer.AddToClassList("EventBox");

                            ValueBox.Add(ValueNameField);
                            ValueBox.Add(OperationField);
                            ValueBox.Add(OperationValueField);
                            boxContainer2.Add(boxContainer);
                        }

                        else {
                            if (fields[i].IsPublic) {
                                Label objectField2 = new Label($"Event doesn't support {fields[i].FieldType.ToString()}");
                                objectField2.AddToClassList("EventSO");
                                boxContainer2.Add(objectField2);
                            }
                        }

                        ValueBox.Add(boxContainer2);
                        mainContainer.Add(ValueBox);
                    }
                }
            }
        }

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
