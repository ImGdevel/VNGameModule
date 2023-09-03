using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundList", menuName = "ScriptableObject/Background List", order = 1)]
public class BackgroundList : ScriptableObject
{
    [Header("Backgrounds")]
    public List<Sprite> backgroundSprites = new List<Sprite>(); // 배경화면 스프라이트 리스트

    // 기타 배경 설정이나 기능을 추가할 수 있습니다.
}
