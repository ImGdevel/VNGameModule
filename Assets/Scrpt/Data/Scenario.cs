using System.Collections.Generic;
using System;

namespace VisualNovelGame
{
    [Serializable]
    public class Scenario
    {
        public List<Dialogue> dialogues = new List<Dialogue>();
        public List<Choice> choices = new List<Choice>();
    }

    [Serializable]
    public class Dialogue
    {
        public string GUID;
        public string text;

        public Dialogue(string guid, string text)
        {
            GUID = guid;
            this.text = text;
        }
    }

    [Serializable]
    public class Choice
    {
        public string GUID;
        public string ChoiceText;
        public List<string> nextDialogueGUIDs = new List<string>();

        public Choice(string guid, string choiceText)
        {
            GUID = guid;
            ChoiceText = choiceText;
        }
    }
}
