using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] npcPrefabs;
    public int numberOfNPCs = 5;

    [Header("Coordinate Rules")]
    public float spawnY;
    public float minX;
    public float maxX;
    public float minDistanceBetween;

    private List<GameObject> _spawnedNPCs = new List<GameObject>();

    void Start()
    {
        SpawnAllNPCs();
    }

    void SpawnAllNPCs()
    {
        for (int i = 0; i < numberOfNPCs; i++)
        {
            Vector3 pos = GetRandomValidPosition();
            if (pos != Vector3.zero)
            {
                GameObject selectedPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
                GameObject npc = Instantiate(selectedPrefab, pos, Quaternion.identity, transform);
                
                _spawnedNPCs.Add(npc);
            }
        }
    }

    private Vector3 GetRandomValidPosition()
    {
        int maxAttempts = 50;
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(minX, maxX);
            Vector3 potentialPos = new Vector3(randomX, spawnY, 0f);

            bool isOverlapping = false;

            foreach (GameObject existingNPC in _spawnedNPCs)
            {
                if (existingNPC == null) continue;

                float distance = Mathf.Abs(existingNPC.transform.position.x - randomX);
                if (distance < minDistanceBetween)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (!isOverlapping)
            {
                return potentialPos;
            }
        }

        return Vector3.zero;
    }
}