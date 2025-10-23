using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float damage = 25f;
    public float lifetime = 3f;
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit: {other.name} with tag: {other.tag}");
        
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                Debug.Log("Dealing damage to enemy");
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Remove the "Projectile" tag check since it doesn't exist
        else if (!other.CompareTag("Player")) // Only ignore player collisions
        {
            Destroy(gameObject); // Hit wall/ground/other object
            //comment
        }
    }
}