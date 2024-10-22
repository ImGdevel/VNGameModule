using System.Collections.Generic;
using System;
using UnityEngine;

namespace VisualNovelGame
{
    [Serializable]
    public class Scenario
    {
        public string chapterId;
        public Dictionary<string, Scene> scenes;
        public Dictionary<string, Character> charactor;
    }

    public enum SceneType
    {
        Dialogue,
        ChoiceDialogue,
        Event,
        EventCG,
        RandomEvent,
        IfEvent,
        CharacterCG,
    }

    public class Scene
    {
        public string id;
        public SceneType type;

        public Scene(string id, SceneType type)
        {
            this.id = id;
            this.type = type;
        }
    }

    [Serializable]
    public class Dialogue : Scene
    {
        public string text;
        public string nextDialogue;
        public string character;
        public string emotion;

        public Dialogue(string id, string nextDialogue, string text)
            : base(id, SceneType.Dialogue)
        {
            this.text = text;
            this.nextDialogue = nextDialogue;
        }
    }

    [Serializable]
    public class ChoiceDialogue : Scene
    {
        public string text;
        public List<ChoiceData> nextDialogues;

        public ChoiceDialogue(string id,  List<ChoiceData> nextDialogues, string text = "")
            : base(id, SceneType.ChoiceDialogue)
        {
            this.text = text;
            this.nextDialogues = nextDialogues;
        }

        public class ChoiceData
        {
            public string text;
            public string nextDialogue;
        }
    }

    public class Event : Scene
    {

        public Event(string id)
            :base(id, SceneType.Event)
        {

        }
    }

    public class RandomEvent : Scene
    {
        public int randomSelectionCount;
        public List<string> nextDialogues = new List<string>();

        public RandomEvent(string id, List<string> nextDialogues, int selection = 0)
            : base(id, SceneType.Dialogue)
        {
            this.nextDialogues = nextDialogues;
            this.randomSelectionCount = (selection == 0) ? nextDialogues.Count : selection;
        }
    }

    public class IfEvent : Scene
    {
        public int randomSelectionCount;
        public List<IfSelection> nextDialogues;


        public IfEvent(string id, List<IfSelection> nextDialogues)
            : base(id, SceneType.Dialogue)
        {
            this.nextDialogues = nextDialogues;
        }

        public class IfSelection
        {
            public int condition;
            public int value;
            public Comparison comparison;
            public string nextDialogue;
        }
    }

    public class EventCG : Scene
    {
        string eventCg;

        public EventCG(string id, string eventCg)
            : base(id, SceneType.EventCG)
        {
            this.eventCg = eventCg;
        }
    }

    public class CharacterCG : Scene
    {
        string characterCG;

        public CharacterCG(string id, string characterCG)
            : base(id, SceneType.EventCG)
        {
            this.characterCG = characterCG;
        }
    }

    public enum Comparison
    {
        More,
        Less,
        Equal
    }


    [System.Serializable]
    public class Character
    {
        public string name;
        public Sprite portrait;
    }

    [System.Serializable]
    public class Background
    {
        public string name;
        public Sprite portrait;
    }


}
