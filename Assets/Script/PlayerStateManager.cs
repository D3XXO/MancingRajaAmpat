using UnityEngine;
using UnityEngine.UI;

public class PlayerStateManager : MonoBehaviour
{
    private IPlayerState _currentState;

    [Header("References")]
    public Movement movementComponent;
    public GameObject movementButtonsParent;
    public Animator PlayerAnimator;

    [Header("Fishing Minigame UI")]
    public GameObject fishingMinigamePanel;
    public GameObject fishingButton;
    public Image fishingProgressBar;

    [Header("Minigame Systems")]
    public RhythmSpawner rhythmSpawner;

    public MovementState MovementState { get; private set; }
    public WaitingState WaitingState { get; private set; }
    public FishingState FishingState { get; private set; }

    void Awake()
    {
        MovementState = new MovementState(this);
        WaitingState = new WaitingState(this);
        FishingState = new FishingState(this);
        
        if (PlayerAnimator == null) PlayerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
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
}