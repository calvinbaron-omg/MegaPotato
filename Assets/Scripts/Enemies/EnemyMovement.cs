using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float chaseSpeed = 3f;
    public float damageAmount = 20f;
    
    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        // Find player and get physics component
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        // Chase player every physics frame
        ChasePlayer();
    }
    
    void ChasePlayer()
    {
        if (player == null) return;
        
        // Move toward player position
        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        
        // Rotate to face player
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Damage player on collision
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}