using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed;
    
    [Header("References")]
    SpriteRenderer spriteRenderer;
    Animator animator;

    private float moveInput = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleSprite();
    }

    private void HandleMovement()
    {
        if (moveInput != 0)
        {
            transform.Translate(Vector2.right * moveInput * moveSpeed * Time.deltaTime);
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