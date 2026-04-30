using UnityEngine;
using System.Collections;

public class NPCStateManager : MonoBehaviour
{
    private INPCState _currentState;
    
    [Header("Patrol Settings")]
    public float moveSpeed;
    public float patrolRange;
    public float waitTimeMin;
    public float waitTimeMax;
    
    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Optimization")]
    public float activationDistance;
    private Transform _playerTransform;
    private bool _isActive = true;
    
    public Vector3 StartPosition { get; private set; }
    public bool IsInteracting { get; private set; }
    
    public NPCIdleState IdleState { get; private set; }
    public NPCMoveState MoveState { get; private set; }

    void Awake()
    {
        StartPosition = transform.position;
        IdleState = new NPCIdleState(this);
        MoveState = new NPCMoveState(this);

        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null) _playerTransform = player.transform;
    }

    void Start()
    {
        SwitchState(IdleState);
        StartCoroutine(OptimizationCheck());
    }

    void Update()
    {
        if (!_isActive) return;
        _currentState?.Update();
    }

    public void SwitchState(INPCState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void SetInteracting(bool value)
    {
        IsInteracting = value;
        if (IsInteracting) SwitchState(IdleState);
    }

    public void FlipSprite(float direction)
    {
        if (direction > 0) spriteRenderer.flipX = false;
        else if (direction < 0) spriteRenderer.flipX = true;
    }

    private IEnumerator OptimizationCheck()
    {
        while (true)
        {
            if (_playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, _playerTransform.position);
                bool wasActive = _isActive;
                _isActive = (distance <= activationDistance);

                if (animator != null && wasActive != _isActive)
                {
                    animator.enabled = _isActive;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}