using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private bool facingRight = true;
    private float idleBufferTime = 0.1f; // Buffer time in seconds
    private float idleBufferCounter = 0f;
    private KeyCode lastKeyPressed = KeyCode.None;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        HandleIdle();
        FlipSprite();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(horizontalInput, Input.GetAxisRaw("Vertical"));
        animator.SetFloat("Speed", Mathf.Abs(movement.x));

        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            idleBufferCounter = idleBufferTime; // Reset the buffer when there is input
            lastKeyPressed = horizontalInput > 0 ? KeyCode.D : KeyCode.A;
        }

        if (horizontalInput != 0)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
        }
    }

    void HandleIdle()
    {
        if (Mathf.Abs(movement.x) < 0.1f)
        {
            if (idleBufferCounter > 0)
            {
                idleBufferCounter -= Time.deltaTime;
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdle", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdle", true);
            }
        }
        else
        {
            idleBufferCounter = idleBufferTime; // Reset the buffer when there is movement
            animator.SetBool("isIdle", false);
        }
    }

    void FlipSprite()
    {
        if (lastKeyPressed == KeyCode.D && !facingRight)
        {
            Flip();
        }
        else if (lastKeyPressed == KeyCode.A && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
