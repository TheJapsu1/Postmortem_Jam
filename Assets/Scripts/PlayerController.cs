using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Running")]
    public float accelerationSpeed = 10f;
    public float maxRunSpeed = 15f;
    public float stoppingDrag = 4f;

    [Header("Jumping & falling")]
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float coyoteTime = 0.2f;

    [Header("Grounded check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayers;
    public Transform[] groundCheckPoints;

    private Rigidbody2D rb;
    private Animator[] animators;
    private SpriteRenderer[] renderers;
    
    private float horizontalInput;
    private float cachedGravityScale;
    public float timeSinceGrounded = 0f;
    
    private bool isJumpQueued;
    private bool isGrounded;
    private bool isRunning;
    
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    private static readonly int YVelocity = Animator.StringToHash("YVelocity");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        renderers = GetComponentsInChildren<SpriteRenderer>();

        cachedGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if(GameController.Singleton.inEnd) return;
        
        GetInput();

        foreach (Animator animator in animators)
        {
            animator.SetBool(Grounded, isGrounded);
            animator.SetBool(Running, isRunning);
            animator.SetFloat(XVelocity, Mathf.Abs(rb.velocity.x));
            animator.SetFloat(YVelocity, rb.velocity.y);
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.flipX = rb.velocity.x < 0;
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        
        HandleMovement();
        
        HandleJumping();
        
        // Clamp the speed
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxRunSpeed, maxRunSpeed), rb.velocity.y);
    }

    private void CheckGrounded()
    {
        isGrounded = false;
        
        // Loop and raycast all the ground check points.
        foreach (Transform groundCheckPoint in groundCheckPoints)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                groundCheckPoint.position, 
                Vector2.down,
                groundCheckDistance,
                groundLayers);

            if (hit.collider == null) continue;
            
            isGrounded = true;
                
            break;
        }

        if (isGrounded)
        {
            timeSinceGrounded = 0;
        }
        else
        {
            timeSinceGrounded += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Moves the player according to player input, and modifies the physics to get a better gamefeel.
    /// </summary>
    private void HandleMovement()
    {
        // Add the horizontal force
        rb.AddForce(Vector2.right * horizontalInput * accelerationSpeed);
        
        Vector2 cachedVelocity = rb.velocity;
        
        bool changingDirection =
            (horizontalInput < 0 && cachedVelocity.x > 0) ||
            (horizontalInput > 0 && cachedVelocity.x < 0);
        
        // If we are stopping or changing the movement direction
        if (Mathf.Abs(horizontalInput) < 0.2f || changingDirection)
        {
            // We apply drag manually on the x axis, as Unity does not allow per-axis drag.
            cachedVelocity.x *= 1 / stoppingDrag;
        }
        else
        {
            rb.drag = 0;
        }

        rb.velocity = cachedVelocity;

        isRunning = Mathf.Abs(horizontalInput) > 0.1f;
    }

    /// <summary>
    /// Applies jumping force based on player input, and modifies the physics to get a better gamefeel.
    /// </summary>
    private void HandleJumping()
    {
        // Jump if there's a jump queued
        if (isJumpQueued)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            isJumpQueued = false;
        }
        
        // Change the gravity scale based on if we are jumping / falling.
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier * cachedGravityScale;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier * cachedGravityScale;
        }
        else
        {
            rb.gravityScale = cachedGravityScale;
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (timeSinceGrounded < coyoteTime && Input.GetButtonDown("Jump"))
        {
            isJumpQueued = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if(groundCheckPoints == null) return;

        foreach (Transform point in groundCheckPoints)
        {
            Vector2 cachedPos = point.position;
            Gizmos.DrawLine(cachedPos, cachedPos + Vector2.down * groundCheckDistance);
        }
    }
}
