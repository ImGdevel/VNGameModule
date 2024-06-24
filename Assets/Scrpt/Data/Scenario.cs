using System.Collections.Generic;


[System.Serializable]
public class Scenario
{
    public int id;
    public List<Dialogue> dialogues;
    // 추가적으로 필요에 따라 선택지, 이벤트 등 추가
}

[System.Serializable]
public class Dialogue
{
    public string characterName;
    public string text;
    public string emotion; // 캐릭터의 감정 상태
}