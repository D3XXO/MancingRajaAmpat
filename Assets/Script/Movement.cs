using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed;
    
    [Header("References")]
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;

    private float moveInput = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleSprite();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (this.enabled)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    private void HandleSprite()
    {
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        bool isMoving = moveInput != 0;
        animator.SetBool("isMoving", isMoving);
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