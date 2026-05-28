using UnityEngine;

[System.Serializable]
public struct TutorialStep
{
    [TextArea(3, 10)]
    public string sentence;
    public Sprite characterSprite;
    public Sprite tutorialImage;
    public Sprite fishingTutorImage;
}

[CreateAssetMenu(fileName = "New Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public TutorialStep[] steps;
}