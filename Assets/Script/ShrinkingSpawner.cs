using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShrinkingSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject notePrefab;
    public RectTransform spawnPanel;
    public Text countdownText;
    
    [Header("Spacing Settings")]
    public float padding;

    [Header("Accessibility Settings")]
    public string[] safeLabels;
    private Color[] _noteColors;

    private bool _isSpawning;
    private Coroutine _spawnRoutine;
    private PlayerStateManager _manager;

    void Awake()
    {
        Color colorGreen, colorYellow, colorBlue, colorOrange, colorRed;
        ColorUtility.TryParseHtmlString("#76A973", out colorGreen);
        ColorUtility.TryParseHtmlString("#E1B05F", out colorYellow);
        ColorUtility.TryParseHtmlString("#7199C7", out colorBlue);
        ColorUtility.TryParseHtmlString("#E7964E", out colorOrange);
        ColorUtility.TryParseHtmlString("#D36666", out colorRed);

        _noteColors = new Color[] { colorGreen, colorYellow, colorBlue, colorOrange, colorRed };
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
        
        foreach (Transform child in spawnPanel) Destroy(child.gameObject);
    }

    private IEnumerator SpawnRoutine()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            countdownText.gameObject.SetActive(false);
        }

        while (_isSpawning)
        {
            GameObject newNote = Instantiate(notePrefab, spawnPanel);
            RectTransform noteRect = newNote.GetComponent<RectTransform>();
            
            float startScale = 12f;
            float noteWidth = noteRect.rect.width * startScale;
            float noteHeight = noteRect.rect.height * startScale;

            Rect progressBarRect = new Rect();
            if (_manager != null && _manager.shrinkingProgressBar != null)
            {
                RectTransform progressRT = _manager.shrinkingProgressBar.GetComponent<RectTransform>();
                progressBarRect = GetLocalRect(progressRT, spawnPanel);
                
                progressBarRect.xMin -= padding;
                progressBarRect.xMax += padding;
                progressBarRect.yMin -= padding;
                progressBarRect.yMax += padding;
            }

            int maxAttempts = 30;
            Vector2 validPosition = Vector2.zero;
            bool positionFound = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                float randomX = Random.Range(spawnPanel.rect.xMin + (noteWidth / 2), spawnPanel.rect.xMax - (noteWidth / 2));
                float randomY = Random.Range(spawnPanel.rect.yMin + (noteHeight / 2), spawnPanel.rect.yMax - (noteHeight / 2));
                
                Vector2 testPos = new Vector2(randomX, randomY);
                Rect testRect = new Rect(testPos.x - noteWidth / 2, testPos.y - noteHeight / 2, noteWidth, noteHeight);

                bool isOverlapping = false;

                if (progressBarRect.Overlaps(testRect))
                {
                    isOverlapping = true;
                }

                if (!isOverlapping)
                {
                    foreach (Transform child in spawnPanel)
                    {
                        if (child.gameObject == newNote) continue;
                        if (child.GetComponent<ShrinkingNote>() == null) continue;

                        RectTransform childRect = child.GetComponent<RectTransform>();
                        
                        Rect cRect = new Rect(childRect.anchoredPosition.x - noteWidth / 2, childRect.anchoredPosition.y - noteHeight / 2, noteWidth, noteHeight);

                        if (testRect.Overlaps(cRect))
                        {
                            isOverlapping = true;
                            break;
                        }
                    }
                }

                if (!isOverlapping)
                {
                    validPosition = testPos;
                    positionFound = true;
                    break;
                }
            }

            if (positionFound)
            {
                noteRect.anchoredPosition = validPosition;

                Image rootImage = newNote.GetComponent<Image>();
                ShrinkingNote noteComp = newNote.GetComponent<ShrinkingNote>();

                if (noteComp != null)
                {
                    Image targetImage = noteComp.visualTransform != null ? noteComp.visualTransform.GetComponent<Image>() : rootImage;

                    if (targetImage != null)
                    {
                        int colorIndex = Random.Range(0, _noteColors.Length);
                        targetImage.color = _noteColors[colorIndex];
                        if (colorIndex == 4) noteComp.isRedNote = true;
                    }

                    noteComp.SetupNote(startScale, DifficultyManager.GetCurrentStrategy().GetDynamicMoveSpeed(_manager.totalValueScore));

                    if (noteComp.labelText != null)
                    {
                        if (noteComp.isRedNote)
                        {
                            noteComp.labelText.text = "x";
                        }
                        else
                        {
                            noteComp.labelText.text = safeLabels[Random.Range(0, safeLabels.Length)];
                        }
                    }
                }
            }
            else
            {
                Destroy(newNote);
            }

            float currentBPM = DifficultyManager.GetCurrentStrategy().GetBPM(_manager.totalValueScore);
            float beatInterval = 30f / currentBPM;
            int beatsToWait = Random.Range(1, 3);

            yield return new WaitForSeconds(beatInterval * beatsToWait);
        }
    }

    private Rect GetLocalRect(RectTransform targetTransform, RectTransform parentTransform)
    {
        if (targetTransform == null || parentTransform == null) return new Rect();

        Vector3[] corners = new Vector3[4];
        targetTransform.GetWorldCorners(corners);

        Vector3 bottomLeft = parentTransform.InverseTransformPoint(corners[0]);
        Vector3 topRight = parentTransform.InverseTransformPoint(corners[2]);

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }
}