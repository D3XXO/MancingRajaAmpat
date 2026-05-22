using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RhythmSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] notePrefabs;
    public Transform spawnPoint;
    public Transform laneParent;
    public float spawnInterval;

    [Header("Systems")]
    public RhythmManager rhythmManager;

    private Color[] _noteColors = new Color[]
    {
        new Color(0.2f, 0.8f, 0.2f), // Hijau
        new Color(1f, 0.9f, 0f),     // Kuning
        new Color(0.2f, 0.6f, 1f),   // Biru
        new Color(1f, 0.5f, 0f),     // Oren
        new Color(1f, 0.2f, 0.2f)    // Merah
    };

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
        yield return new WaitForSeconds(2f);

        while (_isSpawning)
        {
            int randomIndex = Random.Range(0, notePrefabs.Length);
            
            GameObject newNoteObj = Instantiate(notePrefabs[randomIndex], laneParent);
            newNoteObj.transform.position = spawnPoint.position;

            RhythmNote noteComponent = newNoteObj.GetComponent<RhythmNote>();
            noteComponent.moveSpeed = _currentFish.moveSpeed;

            Image noteImage = newNoteObj.GetComponent<Image>();
            if (noteImage != null)
            {
                int randomColorIndex = Random.Range(0, _noteColors.Length);
                noteImage.color = _noteColors[randomColorIndex];

                if (randomColorIndex == 4)
                {
                    noteComponent.isRedNote = true;
                }
            }

            if (rhythmManager != null)
            {
                rhythmManager.allActiveNotes.Add(noteComponent);
            }

            float randomInterval = Random.Range(_currentFish.minSpawnInterval, _currentFish.maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }
}