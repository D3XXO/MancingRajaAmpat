using UnityEngine;

public class NPCIdleState : INPCState
{
    private NPCStateManager _npc;
    private float _timer;
    private float _waitDuration;

    public NPCIdleState(NPCStateManager npc) => _npc = npc;

    public void Enter()
    {
        _npc.animator.SetBool("npcDayung", false);
        _npc.animator.SetBool("npcIdle", true);
        _waitDuration = Random.Range(_npc.waitTimeMin, _npc.waitTimeMax);
        _timer = 0;
    }

    public void Update()
    {
        if (_npc.IsInteracting) return;

        if (_npc.isPlayerFishing)
        {
            _npc.SwitchState(_npc.MoveState);
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= _waitDuration)
        {
            _npc.SwitchState(_npc.MoveState);
        }
    }

    public void Exit() => _npc.animator.SetBool("npcIdle", false);
}