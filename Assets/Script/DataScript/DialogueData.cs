using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public string characterName;
    [TextArea(3, 10)]
    public string sentence;
    public Sprite characterSprite;
}

[System.Serializable]
public class DialogueTopic
{
    public string topicName;
    public DialogueLine[] lines;
}

[CreateAssetMenu(fileName = "New Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueTopic[] dialogueGroups;
}