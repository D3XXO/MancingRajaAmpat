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

        if (_manager.IsInFishingZone && _manager.fishingButton != null)
        _manager.fishingButton.SetActive(true);

        if (_manager.ensiklopediaButton != null)
        _manager.ensiklopediaButton.SetActive(true);

        if (_manager.pauseButton != null)
        _manager.pauseButton.SetActive(true);

        _manager.PlayerAnimator.SetBool("isWaiting", false);
        _manager.PlayerAnimator.SetBool("PlayerUlur", false);
        _manager.PlayerAnimator.SetBool("PlayerTarik", false);

        _manager.StartZoom(5f);
    }

    public void Update()
    {
        float currentX = _manager.transform.position.x;

        if (currentX <= _manager.minWorldX)
        {
            if (_manager.leftMoveButton.activeSelf)
            {
                _manager.leftMoveButton.SetActive(false);
                _manager.movementComponent.StopMoving();
            }
        }
        else
        {
            if (!_manager.leftMoveButton.activeSelf) _manager.leftMoveButton.SetActive(true);
        }

        if (currentX >= _manager.maxWorldX)
        {
            if (_manager.rightMoveButton.activeSelf)
            {
                _manager.rightMoveButton.SetActive(false);
                _manager.movementComponent.StopMoving();
            }
        }
        else
        {
            if (!_manager.rightMoveButton.activeSelf) _manager.rightMoveButton.SetActive(true);
        }
    }

    public void Exit()
    {
        _manager.movementComponent.StopMoving();

        _manager.movementButtonsParent.SetActive(false);
        _manager.ensiklopediaButton.SetActive(true);

        if (_manager.ensiklopediaButton != null)
        _manager.ensiklopediaButton.SetActive(false);

        if (_manager.pauseButton != null) 
        _manager.pauseButton.SetActive(false);
    }
}