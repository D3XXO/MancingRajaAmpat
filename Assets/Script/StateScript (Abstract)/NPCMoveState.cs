using UnityEngine;

public class NPCMoveState : INPCState
{
    private NPCStateManager _npc;
    private int _direction = 1;

    public NPCMoveState(NPCStateManager npc) => _npc = npc;

    public void Enter()
    {
        _npc.animator.SetBool("npcDayung", true);
        _npc.FlipSprite(_direction);
    }

    public void Update()
    {
        float currentX = _npc.transform.position.x;
        float limitKanan = _npc.StartPosition.x + _npc.patrolRange;
        float limitKiri = _npc.StartPosition.x - _npc.patrolRange;

        _npc.transform.Translate(Vector2.right * _direction * _npc.moveSpeed * Time.deltaTime);

        if ((_direction > 0 && currentX >= limitKanan) || (_direction < 0 && currentX <= limitKiri))
        {
            _direction *= -1;
            _npc.SwitchState(_npc.IdleState);
        }
    }

    public void Exit() { }
}