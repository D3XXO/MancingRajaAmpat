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
    public Animator PlayerAnimator;
    public AudioClip clickButton;

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

    [Header("Value Score")]
    public int totalValueScore;
    public Text valueScoreText;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public bool IsInFishingZone;

    [Header("Movement Boundaries")]
    public GameObject leftMoveButton;
    public GameObject rightMoveButton;
    public float minWorldX;
    public float maxWorldX;

    public MovementState MovementState { get; private set; }
    public WaitingState WaitingState { get; private set; }
    public FishingState FishingState { get; private set; }
    public List<FishData> availableFish;

    [HideInInspector]
    public FishingZoneData currentZoneData;

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
        UpdateVSText();

        SwitchState(MovementState);

        if (fishingMinigamePanel != null) fishingMinigamePanel.SetActive(false);
        if (fishingButton != null) fishingButton.SetActive(false);
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

        DialogueManager dialogue = FindObjectOfType<DialogueManager>();
        if (dialogue != null && dialogue.dialoguePanel != null && dialogue.dialoguePanel.activeSelf) return;

        if (_currentState == MovementState)
        {
            if (movementComponent != null)
            {
                movementComponent.StopMoving();
            }

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

    public void ShowCaughtFish(FishData fish)
    {
        StartCoroutine(DisplayFishRoutine(fish));
    }

    private IEnumerator DisplayFishRoutine(FishData fish)
    {
        caughtFishImage.sprite = fish.fishIcon;
        caughtFishText.text = fish.fishName;

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

    public float GetDynamicMoveSpeed()
    {
        float clampedScore = Mathf.Clamp(totalValueScore, 0f, 750f);
        float t = clampedScore / 750f;
        return Mathf.Lerp(4f, 12f, t);
    }

    public float GetDynamicMinSpawnInterval()
    {
        float clampedScore = Mathf.Clamp(totalValueScore, 0f, 300f);
        float t = clampedScore / 300f;
        return Mathf.Lerp(0.5f, 0.2f, t);
    }

    public float GetDynamicMaxSpawnInterval()
    {
        float clampedScore = Mathf.Clamp(totalValueScore, 0f, 300f);
        float t = clampedScore / 300f;
        return Mathf.Lerp(1.0f, 0.4f, t);
    }

    private void UpdateVSText()
    {
        if (valueScoreText != null)
        {
            valueScoreText.text = "Score: " + totalValueScore;
        }
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
}