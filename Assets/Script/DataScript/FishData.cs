using UnityEngine;

public enum FishRarity { Normal, Endemic, Rare }

[CreateAssetMenu(fileName = "New Fish Data")]
public class FishData : ScriptableObject
{
    public string fishID;
    public string fishName;
    public Sprite fishIcon;
    public FishRarity rarity;

    [Header("Informasi Ikan")]
    [TextArea(3, 10)]
    public string generalDescription;

    [TextArea(3, 10)]
    public string funFact;

    [Header("Value Score Mechanics")]
    public int minVSRequirement;
    public int vsValue;
    public int minusScore;

    [HideInInspector]
    public string customAudioPath;
    [HideInInspector]
    public AudioClip customAudioClip;
}