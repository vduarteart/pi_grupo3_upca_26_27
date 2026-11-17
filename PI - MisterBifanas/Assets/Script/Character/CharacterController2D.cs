
using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpPower = 5f;

    [Header("Character Settings")]
    [SerializeField] private bool isFacingRight = true;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    
    [SerializeField] private float dashCooldown = 0.6f;
    private float dashCooldownTimer = 0f;

    private bool isDashing = false;
    private float dashTime;

    private float horizontalInput;
    private bool isGrounded = false;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        FlipSprite();
        
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && isGrounded && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isDashing)
        {
            isGrounded = false;
            horizontalInput = 0f;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isJumping", true);
        }
        
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            float dashDirection = isFacingRight ? 1f : -1f;

            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

            dashTime -= Time.fixedDeltaTime;

            if (dashTime <= 0f)
            {
                isDashing = false;
            }

            return;
        }
        
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y );

        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));

        animator.SetFloat(
            "yVelocity",
            isGrounded? -1f: rb.linearVelocity.y
        );
        
        if (animator.GetBool("isJumping") && isGrounded == false && rb.linearVelocity.y == 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
    }

    private void FlipSprite()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && !isGrounded)
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    public void ApplyJumpForce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }
    
    private void StartDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        dashTime = dashDuration;

        animator.SetTrigger("Dash");
    }
}