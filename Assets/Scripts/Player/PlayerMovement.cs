using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    public float baseJumpForce = 8f;
    
    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.3f;
    
    private Animator animator;
    private Rigidbody rb;
    private PlayerStats playerStats;
    private bool isGrounded;
    private bool wasGrounded;
    private int currentJumps;
    private int ungroundedFrames = 0;
    private int requiredUngroundedFrames = 3;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerStats = GetComponent<PlayerStats>();
        currentJumps = 0;
        wasGrounded = true;
    }

    [Header("Animation Settings")]
public float animationSmoothing = 0.1f;

private float currentSpeedParameter = 0f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        float inputMagnitude = new Vector3(h, 0, v).magnitude;
        
        // Smooth the animation parameter
        currentSpeedParameter = Mathf.Lerp(currentSpeedParameter, inputMagnitude, animationSmoothing);
        
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeedParameter);
        }
        
        // Rest of your movement code...
        float currentSpeed = playerStats != null ? playerStats.GetMoveSpeed() : baseSpeed;
        Vector3 move = new Vector3(h, 0, v) * currentSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (move != Vector3.zero)
        {
            transform.forward = move.normalized;
        }
        
        HandleJump();
    }

    void FixedUpdate()
    {
        // Do ground detection in FixedUpdate for consistency with physics
        HandleGroundDetection();
    }

    void HandleGroundDetection()
    {
        wasGrounded = isGrounded;
        
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col == null) return;
        
        float bottomOfCharacter = transform.position.y + col.center.y - col.height * 0.5f;
        Vector3 groundCheckOrigin = new Vector3(transform.position.x, bottomOfCharacter, transform.position.z);
        
        // Use multiple raycasts for more reliable ground detection
        float raycastSpacing = col.radius * 0.8f;
        int raysHit = 0;
        
        // Cast multiple rays in a small pattern around the character's feet
        Vector3[] rayOrigins = new Vector3[]
        {
            groundCheckOrigin, // Center
            groundCheckOrigin + new Vector3(raycastSpacing, 0, 0), // Right
            groundCheckOrigin + new Vector3(-raycastSpacing, 0, 0), // Left
            groundCheckOrigin + new Vector3(0, 0, raycastSpacing), // Forward
            groundCheckOrigin + new Vector3(0, 0, -raycastSpacing) // Back
        };
        
        foreach (Vector3 origin in rayOrigins)
        {
            if (Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer))
            {
                raysHit++;
            }
        }
        
        // Consider grounded if at least 1 ray hits the ground
        bool frameGrounded = raysHit >= 1;
        
        // Ground buffer system prevents flickering between grounded/ungrounded states
        // Requires multiple consecutive frames of ungrounded before switching state
        if (frameGrounded)
        {
            ungroundedFrames = 0;
            isGrounded = true;
        }
        else
        {
            ungroundedFrames++;
            if (ungroundedFrames >= requiredUngroundedFrames)
            {
                isGrounded = false;
            }
        }
        
        // Update animator parameters
        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        }
        
        // Handle landing - only if we were airborne and now grounded
        if (!wasGrounded && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            if (animator != null)
            {
                animator.SetTrigger("Land");
                animator.ResetTrigger("Jump");
            }
            currentJumps = 0;
        }
        
        // Reset jumps when properly grounded and not moving upward
        if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            currentJumps = 0;
        }
    }

    void HandleJump()
    {
        // Get jump stats from PlayerStats
        float jumpForce = playerStats != null ? playerStats.GetJumpHeight() : baseJumpForce;
        int maxJumps = playerStats != null ? playerStats.GetMaxJumps() : 1;
        
        // Jump when Space is pressed and has jumps remaining
        if (Input.GetKeyDown(KeyCode.Space) && currentJumps < maxJumps)
        {
            // Apply jump force
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            
            // Update animation
            if (animator != null)
            {
                animator.ResetTrigger("Land");
                animator.SetTrigger("Jump");
                animator.SetBool("IsGrounded", false);
            }
            
            currentJumps++;
            isGrounded = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize ground detection in Scene view
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col == null) return;
        
        float bottomOfCharacter = transform.position.y + col.center.y - col.height * 0.5f;
        Vector3 groundCheckOrigin = new Vector3(transform.position.x, bottomOfCharacter, transform.position.z);
        float raycastSpacing = col.radius * 0.8f;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        
        // Draw multiple raycast origins
        Vector3[] rayOrigins = new Vector3[]
        {
            groundCheckOrigin,
            groundCheckOrigin + new Vector3(raycastSpacing, 0, 0),
            groundCheckOrigin + new Vector3(-raycastSpacing, 0, 0),
            groundCheckOrigin + new Vector3(0, 0, raycastSpacing),
            groundCheckOrigin + new Vector3(0, 0, -raycastSpacing)
        };
        
        foreach (Vector3 origin in rayOrigins)
        {
            Gizmos.DrawWireSphere(origin, 0.05f);
            Gizmos.DrawRay(origin, Vector3.down * groundCheckDistance);
        }
    }
}