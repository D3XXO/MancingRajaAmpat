using UnityEngine;
using System.Collections;

public class RhythmSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] notePrefabs;
    public Transform spawnPoint;
    public Transform laneParent;
    public float spawnInterval;

    [Header("Systems")]
    public RhythmManager rhythmManager;

    private FishData _currentFish;
    private bool _isSpawning = false;
    private Coroutine _spawnRoutine;

    public void StartSpawning(FishData fish)
    {
        if (_isSpawning) return;
        _currentFish = fish;
        _isSpawning = true;
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        
        foreach (Transform child in laneParent)
        {
            if (child.GetComponent<RhythmNote>()) Destroy(child.gameObject);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (_isSpawning)
        {
            int randomIndex = Random.Range(0, notePrefabs.Length);
            
            GameObject newNoteObj = Instantiate(notePrefabs[randomIndex], laneParent);
            newNoteObj.transform.position = spawnPoint.position;

            RhythmNote noteComponent = newNoteObj.GetComponent<RhythmNote>();
            noteComponent.moveSpeed = _currentFish.moveSpeed;

            if (rhythmManager != null)
            {
                rhythmManager.allActiveNotes.Add(noteComponent);
            }

            float randomInterval = Random.Range(_currentFish.minSpawnInterval, _currentFish.maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}