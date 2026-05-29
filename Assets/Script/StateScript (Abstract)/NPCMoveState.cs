using UnityEngine;

public class NPCMoveState : INPCState
{
    private NPCStateManager _npc;
    private int _direction = 1;

    public NPCMoveState(NPCStateManager npc) => _npc = npc;

    public void SetDirection(int newDirection)
    {
        _direction = newDirection;
    }

    public void Enter()
    {
        _npc.animator.SetBool("npcDayung", true);
        _npc.FlipSprite(_direction);
    }

    public void Update()
    {
        float currentX = _npc.transform.position.x;
        float limitKanan = _npc.maxX;
        float limitKiri = _npc.minX;

        _npc.transform.Translate(Vector2.right * _direction * _npc.moveSpeed * Time.deltaTime);

        if ((_direction > 0 && currentX >= limitKanan) || (_direction < 0 && currentX <= limitKiri))
        {
            float clampedX = Mathf.Clamp(currentX, limitKiri, limitKanan);
            _npc.transform.position = new Vector3(clampedX, _npc.transform.position.y, _npc.transform.position.z);

            _direction *= -1;
            
            _npc.SwitchState(_npc.IdleState);
        }
    }

    public void Exit() { }
}