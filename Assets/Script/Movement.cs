using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    public void MoveRight()
    {
        moveInput = 1f;
    }

   
    public void MoveLeft()
    {
        moveInput = -1f;
    }

    public void StopMoving()
    {
        moveInput = 0f;
    }
}