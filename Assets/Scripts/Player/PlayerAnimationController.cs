using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    
    void Start()
    {
        // Get reference to the Animator component
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Get player input axes (WASD/Arrow keys)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        // Calculate input magnitude (0 when still, 1 when full input)
        float inputMagnitude = new Vector3(h, 0, v).magnitude;
        
        // Update animator parameters
        animator.SetFloat("Speed", inputMagnitude);           // Controls Idle/Walk/Run blend
        animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift)); // Sprint state
    }
}