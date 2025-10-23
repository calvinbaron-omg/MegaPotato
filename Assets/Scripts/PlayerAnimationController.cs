using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Use the same input values as your movement script
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        // Calculate input magnitude (0-1 range)
        float inputMagnitude = new Vector3(h, 0, v).magnitude;
        
        // Set animation parameters
        animator.SetFloat("Speed", inputMagnitude);
        animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));
    }
}