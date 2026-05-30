using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FishingZoneSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject fishingZonePrefab;
    public int totalZonesToSpawn;
    public float waitDuration;

    [Header("Coordinate Rules")]
    private float spawnY = -3.5f;
    private float minX = 36f;
    private float maxX = 180f;
    private float zoneWidth = 25f;
    private float minDistanceToMove = 20f;

    [Header("References")]
    public Transform playerTransform;
    public GameObject notificationTextObj;
    private List<GameObject> _spawnedZones = new List<GameObject>();
    private EncyclopediaManager _encyclopediaManager;
    private DialogueManager _dialogueManager;

    private Dictionary<GameObject, float> _zoneTimers = new Dictionary<GameObject, float>();
    private bool _isInitialSpawnDone = false;

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerStateManager>().transform;
        }

        _encyclopediaManager = FindObjectOfType<EncyclopediaManager>();
        _dialogueManager = FindObjectOfType<DialogueManager>();

        _isInitialSpawnDone = false;

        for (int i = 0; i < totalZonesToSpawn; i++)
        {
            SpawnNewZone();
        }

        _isInitialSpawnDone = true;

        if (notificationTextObj != null)
        {
            notificationTextObj.SetActive(false);
        }
    }

    void Update()
    {
        List<GameObject> zonesToDestroy = new List<GameObject>();

        foreach (GameObject zone in _spawnedZones)
        {
            if (zone == null) continue;

            FishingZone zoneScript = zone.GetComponent<FishingZone>();
            if (zoneScript == null) continue;

            bool isUIActive = false;
            if (_encyclopediaManager != null && _encyclopediaManager.encyclopediaPanel != null && _encyclopediaManager.encyclopediaPanel.activeSelf) isUIActive = true;
            if (_dialogueManager != null && _dialogueManager.dialoguePanel != null && _dialogueManager.dialoguePanel.activeSelf) isUIActive = true;

            if (zoneScript.hasBeenEnteredOnce && !zoneScript.isPlayerInside && !isUIActive)
            {
                if (!_zoneTimers.ContainsKey(zone))
                {
                    _zoneTimers[zone] = 0f;
                }

                _zoneTimers[zone] += Time.deltaTime;

                if (_zoneTimers[zone] >= waitDuration)
                {
                    float distanceToPlayer = Mathf.Abs(zone.transform.position.x - playerTransform.position.x);

                    if (distanceToPlayer > minDistanceToMove)
                    {
                        zonesToDestroy.Add(zone);
                    }
                }
            }
        }

        foreach (GameObject zoneToDestroy in zonesToDestroy)
        {
            _spawnedZones.Remove(zoneToDestroy);
            _zoneTimers.Remove(zoneToDestroy);

            FishingZone script = zoneToDestroy.GetComponent<FishingZone>();
            if (script != null) script.DestroyZoneUI();

            Destroy(zoneToDestroy);
            SpawnNewZone();
        }
    }

    private void SpawnNewZone()
    {
        Vector3 newPos = GetRandomValidPosition();
        if (newPos != Vector3.zero)
        {
            GameObject newZone = Instantiate(fishingZonePrefab, newPos, Quaternion.identity, transform);
            
            FishingZone newZoneScript = newZone.GetComponent<FishingZone>();
            if (newZoneScript != null) newZoneScript.spawnerParent = this;

            _spawnedZones.Add(newZone);
            _zoneTimers[newZone] = 0f;

            if (_isInitialSpawnDone)
            {
                TriggerSpawnNotification();
            }
        }
    }

    private void TriggerSpawnNotification()
    {
        if (notificationTextObj != null)
        {
            notificationTextObj.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(HideNotificationRoutine());
        }
    }

    private IEnumerator HideNotificationRoutine()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        
        if (notificationTextObj != null)
        {
            notificationTextObj.SetActive(false);
        }
    }

    public void OnPlayerLeftZone(GameObject zone)
    {
        
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