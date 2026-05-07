using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float decelerationSpeed;
    
    [Header("References")]
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;

    private float moveInput = 0f;
    private float targetDirection = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleSprite();
    }

    private void HandleMovement()
    {
        moveInput = Mathf.MoveTowards(moveInput, targetDirection, decelerationSpeed * Time.deltaTime);
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void HandleSprite()
    {
        if (moveInput > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        bool isMoving = Mathf.Abs(moveInput) > 0.01f;
        animator.SetBool("isMoving", isMoving);
    }

    public void StartMoving(float direction)
    {
        targetDirection = direction;
    }

    public void StopMoving()
    {
        targetDirection = 0f;
    }
}