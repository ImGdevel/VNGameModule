using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using MeetAndTalk.GlobalValue;
using MeetAndTalk.Localization;

namespace MeetAndTalk
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue Character")]
    public class DialogueCharacterSO : ScriptableObject
    {
        public List<LanguageGeneric<string>> characterName;
        public Color textColor = new Color(.8f, .8f, .8f, 1);

        public string HexColor()
        {
            return $"#{ColorUtility.ToHtmlStringRGB(textColor)}";
        }

        private void OnValidate()
        {
            if (characterName.Count != System.Enum.GetNames(typeof(LocalizationEnum)).Length)
            {
                // Mniej
                if (characterName.Count < System.Enum.GetNames(typeof(LocalizationEnum)).Length)
                {
                    for (int i = characterName.Count; i < System.Enum.GetNames(typeof(LocalizationEnum)).Length; i++)
                    {
                        characterName.Add(new LanguageGeneric<string>());
                        characterName[i].languageEnum = (LocalizationEnum)i;
                        characterName[i].LanguageGenericType = "";
                    }
                }
                // Wi?ej
                if (characterName.Count > System.Enum.GetNames(typeof(LocalizationEnum)).Length)
                {
                    for (int i = 0; i < characterName.Count - System.Enum.GetNames(typeof(LocalizationEnum)).Length; i++)
                    {
                        characterName.Remove(characterName[characterName.Count - 1]);
                    }
                }
            }
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
    }
}
[System.Serializable]
public enum AvatarPosition { None, Left, Right }

[System.Serializable]
public enum AvatarType { Normal = 0, Smile = 1, Suprized = 2, Disgust = 3, Crying = 4, Angry = 5 }
