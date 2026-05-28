using UnityEngine;

[System.Serializable]
public class FishingZoneData
{
    [Range(0, 100)] public float normalChance;
    [Range(0, 100)] public float endemicChance;
    [Range(0, 100)] public float rareChance;

    [HideInInspector] public int selectedMinigame;
}