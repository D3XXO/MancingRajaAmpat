using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RhythmSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] notePrefabs;
    public GameObject redNotePrefab;
    public Transform spawnPoint;
    public Transform laneParent;
    public float spawnInterval;
    public Text countdownText;

    [Header("Systems")]
    public RhythmManager rhythmManager;

    private Color _colorGreen;
    private Color _colorYellow;
    private Color _colorBlue;
    private Color _colorOrange;
    private Color _colorRed;
    private Color[] _normalNoteColors;

    private bool _isSpawning = false;
    private Coroutine _spawnRoutine;
    private PlayerStateManager _manager;
    public bool isCountingDown = false;

    void Awake()
    {
        ColorUtility.TryParseHtmlString("#76A973", out _colorGreen);
        ColorUtility.TryParseHtmlString("#E1B05F", out _colorYellow);
        ColorUtility.TryParseHtmlString("#7199C7", out _colorBlue);
        ColorUtility.TryParseHtmlString("#E7964E", out _colorOrange);
        ColorUtility.TryParseHtmlString("#D36666", out _colorRed);

        _normalNoteColors = new Color[] { _colorGreen, _colorYellow, _colorBlue, _colorOrange };
    }

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
            GameObject newNoteObj = null;
            RhythmNote noteComponent = null;

            bool spawnRedNote = Random.value <= 0.2f;

            if (spawnRedNote && redNotePrefab != null)
            {
                newNoteObj = Instantiate(redNotePrefab, laneParent);
                newNoteObj.transform.position = spawnPoint.position;
                
                noteComponent = newNoteObj.GetComponent<RhythmNote>();
                noteComponent.isRedNote = true;

                Image noteImage = newNoteObj.GetComponent<Image>();
                if (noteImage != null) noteImage.color = _colorRed;
            }
            else
            {
                int randomIndex = Random.Range(0, notePrefabs.Length);
                newNoteObj = Instantiate(notePrefabs[randomIndex], laneParent);
                newNoteObj.transform.position = spawnPoint.position;
                
                noteComponent = newNoteObj.GetComponent<RhythmNote>();
                noteComponent.isRedNote = false;

                Image noteImage = newNoteObj.GetComponent<Image>();
                if (noteImage != null)
                {
                    int randomColorIndex = Random.Range(0, _normalNoteColors.Length);
                    noteImage.color = _normalNoteColors[randomColorIndex];
                }
            }

            noteComponent.moveSpeed = DifficultyManager.GetCurrentStrategy().GetDynamicMoveSpeed(_manager.totalValueScore);

            if (rhythmManager != null)
            {
                rhythmManager.allActiveNotes.Add(noteComponent);
            }

            float currentBPM = DifficultyManager.GetCurrentStrategy().GetBPM(_manager.totalValueScore);
            float beatInterval = 60f / currentBPM;
            
            int beatsToWait = Random.Range(1, 3);
            yield return new WaitForSeconds(beatInterval * beatsToWait);
        }
    }
}