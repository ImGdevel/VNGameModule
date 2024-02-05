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
    private string chapter; // ���� é�� ��ȣ�Դϴ�.
    
    [SerializeField, ReadOnly]
    public string sceneId; //VNScene ���� �ĺ� ��ȣ�Դϴ�.

    [TextArea(2,2)]
    public string dialogue; // ��ȭ ����
    
    [Space]
    public List<Character> character; //������ ������

    public Sprite backgroundImage; // ��� �̹���
    public AudioClip backgroundMusic; // ��� ����
    
    public VNScene(string chapter, int sceneNumber = 01)
    {
        this.chapter = chapter;
        this.sceneId = "S" + chapter + "_" + sceneNumber.ToString("D3");
    }
}


