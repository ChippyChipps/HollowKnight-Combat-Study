using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Takes the player's movement intent and converts it into horizontal movement.

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [SerializeField] private float groundAcceleration = 50f;
    [SerializeField] private float groundDeceleration = 60f;

    [SerializeField] private float airAcceleration = 20f;
    [SerializeField] private float airDeceleration = 25f;

    private float currentSpeed;
    private float targetSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private float coyoteTimer;
    private float jumpBufferTimer;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    public int FacingDirection { get; private set; } = 1;
    public bool IsGrounded { get; private set; }

    private bool wasGrounded;
    private bool jumpReduced;
    private bool isJumping;

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
        CheckFlip();

        CalculateTargetSpeed();
        ApplyAcceleration();
        ApplyVelocity();
    }

    //private void Move()
    //{
    //    Vector2 velocity = player.RB.linearVelocity;
    //    velocity.x = player.Input.MoveInput.x * moveSpeed;
    //    player.RB.linearVelocity = velocity;
    //}

    private void CalculateTargetSpeed()
    {
        targetSpeed = player.Input.MoveInput.x * moveSpeed;
    }

    private float GetAccelerationRate()
    {
        if (IsGrounded)
        {
            if (currentSpeed < targetSpeed)
            {
                return groundAcceleration;
            }
            else
            {
                return groundDeceleration;
            }
        }
        else
        {
            if (currentSpeed < targetSpeed)
            {
                return airAcceleration;
            }
            else
            {
                return airDeceleration;
            }
        }
    }

    private void ApplyAcceleration()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, GetAccelerationRate() * Time.fixedDeltaTime);
    }

    private void ApplyVelocity()
    {
        Vector2 velocity = player.RB.linearVelocity;
        velocity.x = currentSpeed;
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

        if (IsGrounded)
        {
            jumpReduced = false;
            isJumping = false;
        }

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
        if (CanJump() && jumpBufferTimer > 0f && !isJumping)
        {
            Jump();
            jumpBufferTimer = 0f;
        }
    }

    private void HandleVariableJump()
    {
        if (!player.Input.JumpHeld && player.RB.linearVelocity.y > 0 && !IsGrounded && !jumpReduced)
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
        else if (jumpBufferTimer > 0f)
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