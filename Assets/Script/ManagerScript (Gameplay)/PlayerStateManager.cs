using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class PlayerStateManager : MonoBehaviour
{
    private IPlayerState _currentState;
    public IPlayerState CurrentState => _currentState;

    [Header("References")]
    public Movement movementComponent;
    public GameObject movementButtonsParent;
    public GameObject ensiklopediaButton;
    public GameObject pauseButton;
    public GameObject homeButton;
    public GameObject interactButton;
    public Text difficultyText;
    public Animator PlayerAnimator;
    public AudioClip clickButton;
    public Image fadePanel;

    [Header("Fishing Minigame UI")]
    public GameObject fishingMinigamePanel;
    public GameObject fishingButton;
    public Image rhythmProgressBar;
    public Image shrinkingProgressBar;

    [Header("Minigame Systems")]
    public RhythmSpawner rhythmSpawner;
    public ShrinkingSpawner shrinkingSpawner;

    [Header("Caught Fish UI")]
    public GameObject caughtFishPanel;
    public Image caughtFishImage;
    public Text caughtFishText;
    public Text streakText;
    public GameObject newFishIndicator;

    [Header("Value Score")]
    public int totalValueScore;
    public Text valueScoreText;
    public int currentWinStreak;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    [HideInInspector] public bool IsInFishingZone;

    [Header("Movement Boundaries")]
    public GameObject leftMoveButton;
    public GameObject rightMoveButton;
    public float minWorldX;
    public float maxWorldX;

    public MovementState MovementState { get; private set; }
    public WaitingState WaitingState { get; private set; }
    public FishingState FishingState { get; private set; }
    public List<FishData> availableFish;

    [HideInInspector] public FishingZoneData currentZoneData;
    [HideInInspector] public FishingZone currentFishingZone;
    [HideInInspector] public FishingZone activeStreakZone;
    [HideInInspector] public bool isTeleporting = false;

    public void SetButtonVisualState(GameObject buttonObj, bool isInteractable)
    {
        if (buttonObj == null) return;
        
        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null) btn.interactable = isInteractable;

        Image img = buttonObj.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = isInteractable ? 1f : 0.1f;
            img.color = c;
        }

        Outline[] outlines = buttonObj.GetComponentsInChildren<Outline>(true);
        foreach (Outline outline in outlines)
        {
            if (outline != null)
            {
                Color oc = outline.effectColor;
                oc.a = isInteractable ? 1f : 0f;
                outline.effectColor = oc;
            }
        }
    }

    void Awake()
    {
        MovementState = new MovementState(this);
        WaitingState = new WaitingState(this);
        FishingState = new FishingState(this);
        
        if (PlayerAnimator == null) PlayerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        totalValueScore = PlayerPrefs.GetInt("TotalValueScore", 0);

        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float savedX = PlayerPrefs.GetFloat("PlayerPosX");
            Vector2 loadedPosition = transform.position;
            loadedPosition.x = savedX;
            transform.position = loadedPosition;
        }

        UpdateVSText();
        UpdateDifficultyText();

        SwitchState(MovementState);

        if (fishingMinigamePanel != null) fishingMinigamePanel.SetActive(false);

        if (fishingButton != null) 
        {
            fishingButton.SetActive(true);
            SetButtonVisualState(fishingButton, false);
        }
        if (interactButton != null) 
        {
            interactButton.SetActive(true);
            SetButtonVisualState(interactButton, false);
        }
    }

    void Update()
    {
        _currentState?.Update();
    }

    public void SwitchState(IPlayerState newState)
    {
        _currentState?.Exit();

        if (movementComponent != null)
        {
            movementComponent.StopMoving();
        }

        PlayerAnimator.SetBool("isMoving", false);

        _currentState = newState;
        _currentState.Enter();
    }

    public void OnFishingButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        if (isTeleporting) return;

        DialogueManager dialogue = FindObjectOfType<DialogueManager>();
        if (dialogue != null && dialogue.dialoguePanel != null && dialogue.dialoguePanel.activeSelf) return;

        if (_currentState == MovementState)
        {
            if (movementComponent != null) movementComponent.StopMoving();
            if (movementButtonsParent != null) movementButtonsParent.SetActive(false);

            if (ensiklopediaButton != null) ensiklopediaButton.SetActive(false);
            if (pauseButton != null) pauseButton.SetActive(false);
            if (homeButton != null) homeButton.SetActive(false);
            if (interactButton != null) interactButton.SetActive(false);
            if (fishingButton != null) fishingButton.SetActive(false);

            NPCStateManager[] allNPCs = FindObjectsOfType<NPCStateManager>();
            foreach (NPCStateManager npc in allNPCs)
            {
                if (npc != null)
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, npc.transform.position);
                    
                    if (distanceToPlayer <= npc.maxX)
                    {
                        npc.ForceMoveAwayFrom(transform.position);
                    }
                }
            }

            SwitchState(WaitingState);
        }
        else if (_currentState == WaitingState)
        {
            SwitchState(MovementState);
        }
    }

    public void ShowCaughtFish(FishData fish, int multiplier, bool isNewCatch = false)
    {
        StartCoroutine(DisplayFishRoutine(fish, multiplier, isNewCatch));
    }

    private IEnumerator DisplayFishRoutine(FishData fish, int multiplier, bool isNewCatch)
    {
        caughtFishImage.sprite = fish.fishIcon;
        caughtFishText.text = fish.fishName;

        if (newFishIndicator != null)
        {
            newFishIndicator.SetActive(isNewCatch);
        }

        if (streakText != null)
        {
            if (DifficultyManager.GetCurrentLevel() == DifficultyLevel.Easy)
            {
                if (currentWinStreak > 1)
                {
                    streakText.text = "STREAK!";
                    streakText.gameObject.SetActive(true);
                }
                else
                {
                    streakText.gameObject.SetActive(false);
                }
            }
            else
            {
                if (multiplier > 1)
                {
                    streakText.text = "STREAK!! Score X" + multiplier;
                    streakText.gameObject.SetActive(true);
                }
                else
                {
                    streakText.gameObject.SetActive(false);
                }
            }
        }

        caughtFishPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        caughtFishPanel.SetActive(false);
    }

    public void AddValueScore(int amount, bool forceUpdate = false)
    {
        totalValueScore = Mathf.Max(0, totalValueScore + amount);

        if (forceUpdate && IsInFishingZone)
        {
            FishingZone[] allZones = FindObjectsOfType<FishingZone>();
            foreach (FishingZone zone in allZones)
            {
                if (zone != null)
                {
                    zone.ForceUpdateChances(totalValueScore, availableFish);
                }
            }
        }

        PlayerPrefs.SetInt("TotalValueScore", totalValueScore);
        PlayerPrefs.Save();
        UpdateVSText();
    }

    private void UpdateVSText()
    {
        if (valueScoreText != null)
        {
            valueScoreText.text = "Score: " + totalValueScore;
        }
    }

    public void UpdateDifficultyText()
    {
        if (difficultyText != null)
        {
            DifficultyLevel currentLevel = DifficultyManager.GetCurrentLevel();
            
            difficultyText.text = currentLevel.ToString() + " Mode";
        }
    }

    public void ResetStreak()
    {
        currentWinStreak = 0;
    }

    public void StartZoom(float targetSize)
    {
        StopCoroutine("ZoomRoutine");
        StartCoroutine(ZoomRoutine(targetSize));
    }

    private IEnumerator ZoomRoutine(float targetSize)
    {
        if (fishingButton != null)
        {
            fishingButton.SetActive(false);
        }

        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsed = 0;
        float duration = 1.0f;

        while (elapsed < duration)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;

        if (fishingButton != null && IsInFishingZone && _currentState == MovementState)
        {
            fishingButton.SetActive(true);
        }
    }

    public void TriggerShake(float intensity, float time)
    {
        StartCoroutine(ShakeRoutine(intensity, time));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = 0f;
    }

    public void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.Save();
    }

    private void OnDisable()
    {
        SavePlayerPosition();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            SavePlayerPosition();
        }
    }

    public void TeleportHome()
    {
        if (_currentState == MovementState && !isTeleporting)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(clickButton);
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        if (movementComponent != null) movementComponent.StopMoving();
        PlayerAnimator.SetBool("isMoving", false);

        if (movementButtonsParent != null) movementButtonsParent.SetActive(false);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(false);
        if (homeButton != null) homeButton.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(false);
        if (fishingButton != null) fishingButton.SetActive(false);
        if (interactButton != null) interactButton.SetActive(false);

        float fadeDuration = 1f;
        float elapsed = 0f;

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadePanel.color = c;
                yield return null;
            }
            c.a = 1f;
            fadePanel.color = c;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        Vector3 newPos = transform.position;
        newPos.x = -2f;
        newPos.y = -3f;
        transform.position = newPos;
        
        SavePlayerPosition();

        yield return new WaitForSeconds(0.2f);

        elapsed = 0f;
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                fadePanel.color = c;
                yield return null;
            }
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        if (movementButtonsParent != null) movementButtonsParent.SetActive(true);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(true);
        if (homeButton != null) homeButton.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(true);
        if (fishingButton != null) fishingButton.SetActive(true); 
        if (interactButton != null) interactButton.SetActive(true);

        isTeleporting = false;
    }
}