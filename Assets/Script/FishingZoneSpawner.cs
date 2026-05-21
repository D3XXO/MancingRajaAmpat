using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FishingZoneSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject fishingZonePrefab;
    public int totalZonesToSpawn;

    [Header("Coordinate Rules")]
    private float spawnY = -4f;
    private float minX = 36f;
    private float maxX = 180f;
    private float zoneWidth = 12f;
    private float minDistanceToMove = 20f;

    [Header("References")]
    public Transform playerTransform;

    private List<GameObject> _spawnedZones = new List<GameObject>();

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerStateManager>().transform;
        }

        for (int i = 0; i < totalZonesToSpawn; i++)
        {
            Vector3 spawnPos = GetRandomValidPosition();
            if (spawnPos != Vector3.zero)
            {
                GameObject zone = Instantiate(fishingZonePrefab, spawnPos, Quaternion.identity, transform);
                
                FishingZone zoneScript = zone.GetComponent<FishingZone>();
                if (zoneScript != null) zoneScript.spawnerParent = this;

                _spawnedZones.Add(zone);
            }
        }
    }

    public void OnPlayerLeftZone(GameObject zone)
    {
        StartCoroutine(WaitAndRepositionRoutine(zone));
    }

    private IEnumerator WaitAndRepositionRoutine(GameObject zone)
    {
        FishingZone zoneScript = zone.GetComponent<FishingZone>();
        
        while (zoneScript != null && !zoneScript.isPlayerInside)
        {
            float distanceToPlayer = Mathf.Abs(zone.transform.position.x - playerTransform.position.x);

            if (distanceToPlayer > minDistanceToMove)
            {
                Vector3 newPos = GetRandomValidPosition();
                if (newPos != Vector3.zero)
                {
                    zone.transform.position = newPos;
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.5f);
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

            foreach (GameObject existingZone in _spawnedZones)
            {
                if (existingZone == null) continue;

                float distanceBetweenZones = Mathf.Abs(existingZone.transform.position.x - randomX);
                
                if (distanceBetweenZones < zoneWidth)
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