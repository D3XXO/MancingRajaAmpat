using UnityEngine;

public class MovementState : IPlayerState
{
    private PlayerStateManager _manager;

    public MovementState(PlayerStateManager manager)
    {
        _manager = manager;
    }

    public void Enter()
    {
        _manager.movementButtonsParent.SetActive(true);
        _manager.movementComponent.enabled = true;

        _manager.PlayerAnimator.SetBool("isWaiting", false);
        _manager.PlayerAnimator.SetBool("PlayerUlur", false);
        _manager.PlayerAnimator.SetBool("PlayerTarik", false);
    }

    public void Update()
    {

    }

    public void Exit()
    {
        _manager.movementComponent.StopMoving();

        _manager.movementComponent.enabled = false;
        _manager.movementButtonsParent.SetActive(false);
    }
}