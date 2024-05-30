using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float originalGravityScale;
    private float dashStartY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (!isDashing)
        {
            Move();
            Jump();
        }

        HandleDash();
        UpdateDashCooldown();
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void HandleDash()
    {
        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            float moveInput = Input.GetAxis("Horizontal");
            if (moveInput != 0) // Dashing in the direction of movement
            {
                StartDash(new Vector2(moveInput, 0).normalized);
            }
        }

        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                dashTimeLeft -= Time.deltaTime;
                rb.velocity = new Vector2(rb.velocity.x, 0); // Lock the y-position during the dash
                rb.gravityScale = 0; // Disable gravity during the dash
                rb.position = new Vector2(rb.position.x, dashStartY); // Maintain the y-position
            }
            else
            {
                isDashing = false;
                rb.gravityScale = originalGravityScale; // Restore gravity after the dash
                StartDashCooldown();
            }
        }
    }

    void StartDash(Vector2 direction)
    {
        isDashing = true;
        canDash = false; // Disable dashing immediately when starting a dash
        dashTimeLeft = dashDuration;
        dashStartY = rb.position.y; // Lock the y-position at the start of the dash
        rb.velocity = direction * dashSpeed;
    }

    void StartDashCooldown()
    {
        dashCooldownTimer = dashCooldown;
    }

    void UpdateDashCooldown()
    {
        if (!canDash && !isDashing)
        {
            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
            else
            {
                canDash = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
