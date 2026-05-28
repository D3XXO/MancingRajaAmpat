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
    public Text countdownText;

    [Header("Systems")]
    public RhythmManager rhythmManager;

    [Header("Accessibility Settings")]
    public string[] safeLabels;

    private Color[] _noteColors = new Color[]
    {
        new Color(0.2f, 0.8f, 0.2f), // Hijau
        new Color(1f, 0.9f, 0f),     // Kuning
        new Color(0.2f, 0.6f, 1f),   // Biru
        new Color(1f, 0.5f, 0f),     // Oren
        new Color(1f, 0.2f, 0.2f)    // Merah
    };

    private bool _isSpawning = false;
    private Coroutine _spawnRoutine;
    private PlayerStateManager _manager;
    public bool isCountingDown = false;

    void Start()
    {
        _manager = FindObjectOfType<PlayerStateManager>();
    }

    public void StartSpawning(FishData fish)
    {
        if (_manager == null) _manager = FindObjectOfType<PlayerStateManager>();

        if (_isSpawning) return;
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
        if (countdownText != null)
        {
            isCountingDown = true;
            countdownText.gameObject.SetActive(true);
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            countdownText.gameObject.SetActive(false);
            isCountingDown = false;
        }

        while (_isSpawning)
        {
            int randomIndex = Random.Range(0, notePrefabs.Length);
            
            GameObject newNoteObj = Instantiate(notePrefabs[randomIndex], laneParent);
            newNoteObj.transform.position = spawnPoint.position;

            RhythmNote noteComponent = newNoteObj.GetComponent<RhythmNote>();
            noteComponent.moveSpeed = _manager.GetDynamicMoveSpeed();

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

            if (noteComponent.labelText != null)
            {
                if (noteComponent.isRedNote)
                {
                    noteComponent.labelText.text = "x";
                    noteComponent.labelText.color = Color.red;
                }
                else
                {
                    noteComponent.labelText.text = safeLabels[Random.Range(0, safeLabels.Length)];
                    noteComponent.labelText.color = Color.white;
                }
            }

            if (rhythmManager != null)
            {
                rhythmManager.allActiveNotes.Add(noteComponent);
            }

            float randomInterval = Random.Range(_manager.GetDynamicMinSpawnInterval(), _manager.GetDynamicMaxSpawnInterval());
            yield return new WaitForSeconds(randomInterval);
        }
    }
}