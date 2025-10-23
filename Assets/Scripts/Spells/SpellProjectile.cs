using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [Header("Base Projectile Settings")]
    public float speed = 8f;           // Movement speed in units per second
    public float damage = 25f;         // Base damage dealt to enemies
    public float lifetime = 3f;        // Time before auto-destruction (safety measure)
    public Vector3 direction;          // Movement direction set by PlayerAutoAttack
    
    void Start()
    {
        // Auto-destroy projectile after lifetime expires (prevents memory leaks)
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Move projectile in set direction each frame
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with other projectiles and the player
        if (other.CompareTag("Projectile") || other.CompareTag("Player")) 
            return;
        
        if (other.CompareTag("Enemy"))
        {
            // Hit enemy - handle damage and effects
            OnHitEnemy(other.gameObject);
        }
        else
        {
            // Hit wall/ground/other object - destroy projectile
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnHitEnemy(GameObject enemy)
    {
        // Base enemy hit behavior - override in child classes for spell-specific effects
        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}