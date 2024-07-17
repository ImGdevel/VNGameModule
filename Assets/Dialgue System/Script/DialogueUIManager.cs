using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;
using DialogueSystem.GlobalValue;

namespace DialogueSystem
{
    public class DialogueUIManager : MonoBehaviour
    {
        public static DialogueUIManager Instance;

        [Header("Type Writing")]
        public bool EnableTypeWriting = false;
        public float typingSpeed = 50.0f;

        [Header("Dialogue UI")]
        public bool showSeparateName = false;
        public TextMeshProUGUI nameTextBox;
        public TextMeshProUGUI textBox;
        [Space()]
        public GameObject dialogueCanvas;
        public Slider TimerSlider;
        public GameObject SkipButton;
        public GameObject SpriteLeft;
        public GameObject SpriteRight;

        [Header("Dynamic Dialogue UI")]
        public GameObject ButtonPrefab;
        public GameObject ButtonContainer;

        [HideInInspector] public string prefixText;
        [HideInInspector] public string fullText;
        private string currentText = "";
        private int characterIndex = 0;
        private float lastTypingTime;

        private List<Button> buttons = new List<Button>();
        private List<TextMeshProUGUI> buttonsTexts = new List<TextMeshProUGUI>();



        private void Awake()
        {
            Instance = this;
            if(EnableTypeWriting) lastTypingTime = Time.time;
        }

        private void Update()
        {
            if (characterIndex < fullText.Length && EnableTypeWriting)
            {
                if (Time.time - lastTypingTime > 1.0f / typingSpeed)
                {
                    if (fullText[characterIndex].ToString() == "<")
                    {
                        while(fullText[characterIndex].ToString() != ">")
                        {
                            currentText += fullText[characterIndex];
                            characterIndex++;
                        }
                        currentText += fullText[characterIndex];
                        characterIndex++;
                        textBox.text = currentText;
                    }
                    else { currentText += fullText[characterIndex]; characterIndex++; textBox.text = currentText; }

                    lastTypingTime = Time.time;
                }
            }
            else
            {
                textBox.text = prefixText+fullText;
            }
        }

        public void ResetText(string prefix)
        {
            currentText = prefix;
            prefixText = prefix;
            characterIndex = 0;

            // dodaj t?opcje tutaj
        }

        public void SetFullText(string text)
        {
            string newText = text;


            Regex regex = new Regex(@"\{(.*?)\}");
            MatchEvaluator matchEvaluator = new MatchEvaluator(match =>
            {
                string OldText = match.Groups[1].Value;
                return ChangeReplaceableText(OldText); 
            });

            //Debug.Log(regex.ToString());

            newText = regex.Replace(newText, matchEvaluator);

            fullText = newText;
        }

        public void SetButtons(List<string> _texts, List<UnityAction> _unityActions, bool showTimer)
        {
            foreach (Transform child in ButtonContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < _texts.Count; i++)
            {
                GameObject btn = Instantiate(ButtonPrefab, ButtonContainer.transform);
                btn.transform.Find("Text").GetComponent<TMP_Text>().text = _texts[i];
                btn.gameObject.SetActive(true);
                btn.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                btn.GetComponent<Button>().onClick.AddListener(_unityActions[i]);
            }

            TimerSlider.gameObject.SetActive(showTimer);
        }

        string ChangeReplaceableText(string text)
        {
            GlobalValueManager manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();

            string TextToReplace = "[Error Value]";
            /* Global Value */
            for (int i = 0; i < manager.IntValues.Count; i++) { if (text == manager.IntValues[i].ValueName) TextToReplace = manager.IntValues[i].Value.ToString(); } 
            for (int i = 0; i < manager.FloatValues.Count; i++) { if (text == manager.FloatValues[i].ValueName) TextToReplace = manager.FloatValues[i].Value.ToString(); } 
            for (int i = 0; i < manager.BoolValues.Count; i++) { if (text == manager.BoolValues[i].ValueName) TextToReplace = manager.BoolValues[i].Value.ToString(); } 
            for (int i = 0; i < manager.StringValues.Count; i++) { if (text == manager.StringValues[i].ValueName) TextToReplace = manager.StringValues[i].Value; }

            //
            if(text.Contains(",")) 
            {
                string[] tmp = text.Split(',');
                for (int i = 0; i < manager.IntValues.Count; i++) { if (tmp[0] == manager.IntValues[i].ValueName) TextToReplace = Mathf.Abs(manager.IntValues[i].Value - (int)System.Convert.ChangeType(tmp[1], typeof(int))).ToString(); }
                for (int i = 0; i < manager.FloatValues.Count; i++) { if (tmp[0] == manager.FloatValues[i].ValueName) TextToReplace = Mathf.Abs(manager.FloatValues[i].Value - (int)System.Convert.ChangeType(tmp[1], typeof(int))).ToString(); }
            }

            return TextToReplace;
        }

    }
}
