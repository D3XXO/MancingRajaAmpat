using UnityEngine;

[System.Serializable]
public class DialogueTopic
{
    public string topicName;
    [TextArea(3, 10)]
    public string[] sentences;
}

[CreateAssetMenu(fileName = "New Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueTopic[] dialogueGroups;
}