using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    private float moveInput = 0f;

    void Update()
    {
        if (moveInput != 0)
        {
            transform.Translate(Vector2.right * moveInput * moveSpeed * Time.deltaTime);
        }
    }

    public void StartMoving(float direction)
    {
        moveInput = direction;
    }

    public void StopMoving()
    {
        moveInput = 0f;
    }
}