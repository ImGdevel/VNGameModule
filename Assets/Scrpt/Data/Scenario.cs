using System.Collections.Generic;


[System.Serializable]
public class Scenario
{
    public int id;
    public List<Dialogue> dialogues;
    // �߰������� �ʿ信 ���� ������, �̺�Ʈ �� �߰�
}

[System.Serializable]
public class Dialogue
{
    public string characterName;
    public string text;
    public string emotion; // ĳ������ ���� ����
}