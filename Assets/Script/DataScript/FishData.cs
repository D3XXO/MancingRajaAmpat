using UnityEngine;

[CreateAssetMenu(fileName = "New Fish Data", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    
    [Header("Rhythm Settings")]
    public float moveSpeed;
    
    [Header("Spawn Interval Range")]
    public float minSpawnInterval;
    public float maxSpawnInterval;
}