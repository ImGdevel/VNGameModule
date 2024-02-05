using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public Sprite sprite;
    public Transform position;

}

[System.Serializable]
public class VNScene
{
    private string chapter; // 현제 챕터 번호입니다.
    
    [SerializeField, ReadOnly]
    public string sceneId; //VNScene 고유 식별 번호입니다.

    [TextArea(2,2)]
    public string dialogue; // 대화 내용
    
    [Space]
    public List<Character> character; //등장할 개릭터

    public Sprite backgroundImage; // 배경 이미지
    public AudioClip backgroundMusic; // 배경 음악
    
    public VNScene(string chapter, int sceneNumber = 01)
    {
        this.chapter = chapter;
        this.sceneId = "S" + chapter + "_" + sceneNumber.ToString("D3");
    }
}


