using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using DialogueSystem.GlobalValue;
using DialogueSystem.Localization;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/New Character")]
    public class DialogueCharacter : ScriptableObject
    {
        [Header("Name")]
        public List<LanguageGeneric<string>> characterName;
        public GlobalValueClass CustomizedName;
        public bool UseGlobalValue = false;
        [Header("Name Color")]
        public Color textColor = new Color(.8f, .8f, .8f, 1);
        [Header("Avatars")]
        public List<CharacterSprite> Avatars;

        public string HexColor()
        {
            return $"#{ColorUtility.ToHtmlStringRGB(textColor)}";
        }

        private void OnValidate()
        {
            Validate();

        }

        public void Validate()
        {
#if UNITY_EDITOR
            // Check if the game is not currently playing or about to change play mode
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (characterName != null)
                {
                    if (characterName.Count < System.Enum.GetNames(typeof(LocalizationEnum)).Length)
                    {
                        foreach (LocalizationEnum language in (LocalizationEnum[])System.Enum.GetValues(typeof(LocalizationEnum)))
                        {
                            characterName.Add(new LanguageGeneric<string>
                            {
                                languageEnum = language,
                                LanguageGenericType = ""
                            });
                        }
                    }
                }
                else
                {
                    characterName = new List<LanguageGeneric<string>>();
                    Debug.Log("New");
                }
                if (Avatars != null)
                {
                    if (Avatars.Count < System.Enum.GetNames(typeof(CharacterExpression)).Length)
                    {
                        foreach (CharacterExpression language in (CharacterExpression[])System.Enum.GetValues(typeof(CharacterExpression)))
                        {
                            Avatars.Add(new CharacterSprite
                            {
                                type = language,
                                Sprite = null,
                            });
                        }
                    }
                }
                else
                {
                    Avatars = new List<CharacterSprite>();
                    Debug.Log("New");
                }
            }
#endif
        }


        public string GetName()
        {
            LocalizationManager _manager = (LocalizationManager)Resources.Load("Languages");
            if (_manager != null)
            {
                return characterName.Find(text => text.languageEnum == _manager.SelectedLang()).LanguageGenericType;
            }
            else
            {
                return "Can't find Localization Manager in scene";
            }
        }

        public Sprite GetCharacterSprite(CharacterPosition position, CharacterExpression type)
        {
            CharacterSprite cs = Avatars[(int)type];

            return cs.Sprite;
        }

        public Sprite GetCharacterSprite(CharacterExpression type)
        {
            CharacterSprite cs = Avatars[(int)type];

            return cs.Sprite;
        }
    }
}

[System.Serializable]
public class CharacterSprite
{
    public CharacterExpression type;
    public Sprite Sprite;
}

[System.Serializable]
public enum CharacterPosition { Center, Right, Left }

[System.Serializable]
public enum CharacterExpression { Normal = 0, Smile = 1, Suprized = 2, Disgust = 3, Crying = 4, Angry = 5 }
