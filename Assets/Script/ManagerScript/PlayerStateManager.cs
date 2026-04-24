using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateManager : MonoBehaviour
{
    private IPlayerState _currentState;
    public IPlayerState CurrentState => _currentState;

    [Header("References")]
    public Movement movementComponent;
    public GameObject movementButtonsParent;
    public GameObject ensiklopediaButton;
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
    }

    void Update()
    {
        _currentState?.Update();
    }

    public void SwitchState(IPlayerState newState)
    {
        _currentState?.Exit();

        PlayerAnimator.SetBool("isMoving", false);

        _currentState = newState;
        _currentState.Enter();
    }

    public void OnFishingButtonClicked()
    {
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
}