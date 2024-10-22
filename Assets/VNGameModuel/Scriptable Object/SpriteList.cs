using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite List", menuName = "ScriptableObject/Sprite List", order = 1)]
public class SpriteList : ScriptableObject
{
    public string listName;

    [Header("Sprite")]
    public List<Sprite> spriteList = new List<Sprite>();
}
