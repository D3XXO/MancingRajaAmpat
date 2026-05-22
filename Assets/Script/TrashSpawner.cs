using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject trashPrefab;
    public int totalTrashToSpawn;

    [Header("Coordinate Rules")]
    private float spawnY = -3.5f;
    private float minX = -10f;
    private float maxX = 180f;
    private float trashBufferWidth = 8f;
    private float minDistanceToMove = 25f;

    [Header("References")]
    public Transform playerTransform;

    private List<GameObject> _spawnedTrash = new List<GameObject>();

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerStateManager>().transform;
        }

        for (int i = 0; i < totalTrashToSpawn; i++)
        {
            Vector3 spawnPos = GetRandomValidPosition();
            if (spawnPos != Vector3.zero)
            {
                GameObject trash = Instantiate(trashPrefab, spawnPos, Quaternion.identity, transform);
                
                TrashItem trashScript = trash.GetComponent<TrashItem>();
                if (trashScript != null) trashScript.spawnerParent = this;

                _spawnedTrash.Add(trash);
            }
        }
    }

    public void OnTrashCollected(GameObject collectedTrash)
    {
        _spawnedTrash.Remove(collectedTrash);
        
        Vector3 newPos = GetRandomValidPosition();
        if (newPos != Vector3.zero)
        {
            GameObject newTrash = Instantiate(trashPrefab, newPos, Quaternion.identity, transform);
            TrashItem trashScript = newTrash.GetComponent<TrashItem>();
            if (trashScript != null) trashScript.spawnerParent = this;

            _spawnedTrash.Add(newTrash);
        }
    }

    void Update()
    {
        for (int i = _spawnedTrash.Count - 1; i >= 0; i--)
        {
            GameObject trash = _spawnedTrash[i];
            
            if (trash == null)
            {
                _spawnedTrash.RemoveAt(i);
                continue;
            }

            float distanceToPlayer = Mathf.Abs(trash.transform.position.x - playerTransform.position.x);
            
            if (distanceToPlayer > minDistanceToMove + 15f)
            {
                Vector3 newPos = GetRandomValidPosition();
                if (newPos != Vector3.zero)
                {
                    trash.transform.position = newPos;
                }
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

            if (playerTransform != null && Mathf.Abs(randomX - playerTransform.position.x) < 8f)
            {
                continue;
            }

            bool isOverlapping = false;

            foreach (GameObject existingTrash in _spawnedTrash)
            {
                if (existingTrash == null) continue;

                float distanceBetweenTrash = Mathf.Abs(existingTrash.transform.position.x - randomX);
                if (distanceBetweenTrash < trashBufferWidth)
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