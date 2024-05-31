using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float groundCheckDistance = 5f; // Distance for ground check raycast
    public float fastFallMultiplier = 2f; // Multiplier for fast falling
    public float gravityScale = 2f; // Normal gravity scale
    public float fallMultiplier = 2.5f; // Gravity scale for faster falling
    public LayerMask groundLayer; // LayerMask for ground detection

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
        rb.gravityScale = gravityScale; // Set the normal gravity scale
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!isDashing)
        {
            Move();
            HandleFastFall(); // Handle fast fall
        }

        HandleDash();
        UpdateDashCooldown();
        CheckGrounded();

        if (!isDashing)
        {
            Jump();
        }

        ApplyGravityScale(); // Apply different gravity scales
    }

    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false; // Prevent jumping again immediately
        }
    }

    void HandleDash()
    {
        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
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
                rb.gravityScale = gravityScale; // Restore normal gravity after the dash
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

    void HandleFastFall()
    {
        if (Input.GetKey(KeyCode.S) && !isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, -Mathf.Abs(rb.velocity.y) * fastFallMultiplier);
        }
    }

    void ApplyGravityScale()
    {
        if (rb.velocity.y < 0 && !isGrounded) // Falling
        {
            rb.gravityScale = gravityScale * fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.W)) // Jumping and not holding the jump button
        {
            rb.gravityScale = gravityScale * (fallMultiplier / 2);
        }
        else // Normal gravity
        {
            rb.gravityScale = gravityScale;
        }
    }

    void CheckGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;

        // Cast a ray downward from the center of the player to check for ground
        RaycastHit2D hit = Physics2D.Raycast(position, direction, groundCheckDistance, groundLayer);

        // Draw the ray in the scene view for debugging
        Debug.DrawRay(position, direction * groundCheckDistance, Color.red);

        isGrounded = hit.collider != null;
    }
}
