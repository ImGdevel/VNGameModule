using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;
using MeetAndTalk.GlobalValue;

namespace MeetAndTalk
{
    public class DialogueUIManager : MonoBehaviour
    {
        public static DialogueUIManager Instance;

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
            SpriteLeft.SetActive(false);
            SpriteRight.SetActive(false);
        }

        private void Update()
        {

                textBox.text = prefixText+fullText;
            
        }

        public void ResetText(string prefix)
        {
            currentText = prefix;
            prefixText = prefix;
            characterIndex = 0;
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

    }
}
