using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // REMOVE the automatic rotation toward movement
        // if (move != Vector3.zero)
        //     transform.forward = move;
        
        // Animation control - use input magnitude directly
        float inputMagnitude = new Vector3(h, 0, v).magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", inputMagnitude);
            animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));
        }
    }
}