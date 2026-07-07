using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // take players movement intent (store intent) and convert it to an actual movement

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashSpeed = 5f;

    [SerializeField] private float coyoteTime = 0.2f; // grace period for jump
    private float coyoteTimer;
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferTimer;

    public int FacingDirection { get; private set; } = 1;
    public bool IsGrounded { get; private set; }
    private bool wasGrounded;
    private bool jumpReduced;
    private bool isJumping;

    [Header("Ground Check")]
    //[SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        CheckGround();

        HandleJumpBuffer();
        HandleVariableJump();
        HandleJump();
    }

    private void FixedUpdate()
    {
        Move();
        CheckFlip();
    }

    private void Move()
    {
        Vector2 velocity = player.RB.linearVelocity;
        velocity.x = player.Input.MoveInput.x * moveSpeed;
        player.RB.linearVelocity = velocity;
    }

    private void CheckFlip()
    {
        float xInput = player.Input.MoveInput.x;

        if (xInput > 0)
        {
            FacingDirection = 1;
            player.SR.flipX = false;
        }
        else if (xInput < 0)
        {
            FacingDirection = -1;
            player.SR.flipX = true;
        }
    }

    private void CheckGround()
    {
        wasGrounded = IsGrounded;

        IsGrounded = Physics2D.OverlapCircle(player.GroundCheck.position, groundCheckRadius, groundLayer);

        // Reset jump state when grounded
        if (IsGrounded)
        {
            jumpReduced = false;
            isJumping = false;
        }

        // Set coyote timer when leaving ground
        if (wasGrounded && !IsGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else if (!IsGrounded && coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if ((IsGrounded || coyoteTimer > 0f) && jumpBufferTimer > 0f && !isJumping)
        {
            Jump();
            jumpBufferTimer = 0f;
        }
    }

    private void HandleVariableJump()
    {
        if (!player.Input.JumpHeld && player.RB.linearVelocity.y > 0 && !IsGrounded)
        {
            Vector2 velocity = player.RB.linearVelocity;
            velocity.y *= 0.75f;
            player.RB.linearVelocity = velocity;

            jumpReduced = true;
            coyoteTimer = 0f;
        }
    }

    private void HandleJumpBuffer()
    {
        if (player.Input.JumpPressed)
        {
            jumpBufferTimer = jumpBufferTime;
            player.Input.ConsumeJumpInput();
        }
        else if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    private bool CanJump()
    {
        return IsGrounded || coyoteTimer > 0f;
    }

    private void Jump()
    {
        Vector2 velocity = player.RB.linearVelocity;
        velocity.y = jumpForce;
        player.RB.linearVelocity = velocity;

        jumpReduced = false;
        coyoteTimer = 0f;
        isJumping = true;
    }

    private void OnDrawGizmosSelected()
    {
        Player player = GetComponent<Player>();

        if (player != null && player.GroundCheck != null)
        {
            Gizmos.DrawWireSphere(player.GroundCheck.position, groundCheckRadius);
        }
    }
}