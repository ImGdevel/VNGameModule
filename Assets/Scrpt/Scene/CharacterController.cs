using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    CharacterSpriteList[] characterSpriteList;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    private List<List<CharacterSpriteList.CharacterExpression>> expressions;

    private void Start() {
        foreach (var item in characterSpriteList) {
            expressions.Add(item.expressions);
        }

        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetExpression(int characterNumber,string expressionName) {
        CharacterSpriteList.CharacterExpression expression = expressions[characterNumber].Find(exp => exp.expressionName == expressionName);
        if (expression != null) {
            spriteRenderer.sprite = expression.sprite;
        }
        else {
            Debug.LogWarning("Expression not found: " + expressionName);
        }
    }
}
