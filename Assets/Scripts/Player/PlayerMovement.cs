using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;     // Movement speed in units per second
    private Animator animator;   // Reference to Animator for character animations

    void Start()
    {
        // Get the Animator component for controlling animations
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from WASD or arrow keys
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Calculate movement vector and apply to position
        Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Rotate player to face movement direction
        if (move != Vector3.zero)
        {
            transform.forward = move.normalized;
        }
        
        // Update animation parameters based on input
        float inputMagnitude = new Vector3(h, 0, v).magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", inputMagnitude);           // Blend between idle/walk/run
            animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift)); // Sprint state
        }
    }
}