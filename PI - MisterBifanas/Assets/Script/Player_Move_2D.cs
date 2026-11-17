using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Player_Move_2D : MonoBehaviour
{
    public float horizontal;
    public float speed = 4f;
    public float jumpPower = 8f;
    public bool isFacingRight = true;

    public bool dashAvailable = true;
    public bool dashing;
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCooldown = 0.5f;

    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;
    
    [SerializeField] private Animator animator;
    
    private bool isJumping;

    public InputActionReference jump;
    public InputActionReference dash;
    
    // Update is called once per frame
    void Update()
    {
        if (dashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        
        if (horizontal != 0f) 
        {
            animator.SetBool("IsRunning", true);
        }
        
        if (jump.action.WasPressedThisFrame() && IsGrounded())
        {
            animator.SetBool("IsJumping", true);

            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, jumpPower);
        }

        if (dash.action.WasPressedThisFrame() && dashAvailable)
        {
            StartCoroutine(DashCoroutine());
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (dashing)
        {
            return;
        }

        rigidbody.linearVelocity = new Vector2(horizontal * speed, rigidbody.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            animator.SetBool("isJumping", false);
        }
    }

    private void Flip()
    {
        if( isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public IEnumerator DashCoroutine()
    {
        animator.SetTrigger("Dash");
        dashAvailable = false;
        dashing = true;
        float normalGravity = rigidbody.gravityScale;
        rigidbody.gravityScale = 0f;
        rigidbody.linearVelocity = new Vector2(transform.localScale.x * dashPower, 0f);
        yield return new WaitForSeconds(dashTime);
        rigidbody.gravityScale = normalGravity;
        dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        dashAvailable = true;
    }
}
