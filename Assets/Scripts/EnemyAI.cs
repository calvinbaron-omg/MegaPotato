using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float chaseSpeed = 3f;
    
    private Transform player;
    private Rigidbody rb;
    public float damageAmount = 20f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        ChasePlayer();
    }
    
    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        
        // Optional: Make enemy face the player
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
 
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            //FindObjectOfType<GameManager>().GameOver();
        }
    }
    }