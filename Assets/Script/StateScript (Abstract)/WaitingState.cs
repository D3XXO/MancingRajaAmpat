using UnityEngine;

public class WaitingState : IPlayerState
{
    private PlayerStateManager _manager;
    private float _waitTime;
    private float _timer;

    public WaitingState(PlayerStateManager manager)
    {
        _manager = manager;
    }

    public void Enter()
    {
        _manager.PlayerAnimator.SetTrigger("PlayerCast");
        _manager.PlayerAnimator.SetBool("isWaiting", true);
        _manager.ensiklopediaButton.SetActive(false);

        _waitTime = Random.Range(3f, 5f);
        _timer = 0;
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _waitTime)
        {
            _manager.SwitchState(_manager.FishingState);
        }
    }

    public void Exit()
    {
        _manager.PlayerAnimator.ResetTrigger("PlayerCast");
    }
}