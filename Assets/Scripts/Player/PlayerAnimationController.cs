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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        float inputMagnitude = new Vector3(h, 0, v).magnitude;
        
        // Only set Speed parameter - removed IsRunning
        if (animator != null)
        {
            animator.SetFloat("Speed", inputMagnitude);
        }
    }
}