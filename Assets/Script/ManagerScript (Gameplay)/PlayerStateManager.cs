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

    [Header("Fishing Minigame UI")]
    public GameObject fishingMinigamePanel;
    public GameObject fishingButton;
    public Image fishingProgressBar;

    [Header("Minigame Systems")]
    public RhythmSpawner rhythmSpawner;

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
        if (movementComponent != null)
        {
            movementComponent.StopMoving();
        }

        if (_currentState == MovementState)
        {
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

    public void AddValueScore(int amount)
    {
        totalValueScore += amount;
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

    public void StartZoom(float targetSize)
    {
        StopCoroutine("ZoomRoutine");
        StartCoroutine(ZoomRoutine(targetSize));
    }

    private IEnumerator ZoomRoutine(float targetSize)
    {
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