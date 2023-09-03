using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpriteList", menuName = "ScriptableObject/Character Sprite List", order = 0)]
public class CharacterSpriteList : ScriptableObject
{
    [System.Serializable]
    public class CharacterExpression
    {
        public string expressionName;
        public Sprite sprite;
    }

    [Header("Character Sprites")]
    public List<CharacterExpression> expressions = new List<CharacterExpression>(); // 캐릭터 표정/행동 스프라이트 리스트

}