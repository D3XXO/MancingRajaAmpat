using UnityEngine;

[CreateAssetMenu(fileName = "New Fish Data", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishID;
    public string fishName;
    public Sprite fishIcon;

    [TextArea(3, 10)]
    public string description;

    [Header("Rhythm Settings")]
    public float moveSpeed;

    [Header("Spawn Interval Range")]
    public float minSpawnInterval;
    public float maxSpawnInterval;
}