using UnityEngine;

[System.Serializable]
public class FishingZoneData
{
    public string zoneName;
    [Range(0, 100)] public float normalChance;
    [Range(0, 100)] public float endemicChance;
    [Range(0, 100)] public float rareChance;
}