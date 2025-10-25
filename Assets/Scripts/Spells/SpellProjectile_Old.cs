// using UnityEngine;

// public class SpellProjectile : MonoBehaviour
// {
//     [Header("Base Projectile Settings")]
//     public float speed = 8f;
//     public float damage = 25f;
//     public float lifetime = 3f;
//     public Vector3 direction;
    
//     void Start()
//     {
//         Destroy(gameObject, lifetime);
//     }
    
//     void Update()
//     {
//         transform.Translate(direction * speed * Time.deltaTime, Space.World);
//     }
    
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Projectile") || other.CompareTag("Player")) 
//             return;
        
//         if (other.CompareTag("Enemy"))
//         {
//             OnHitEnemy(other.gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
    
//     protected virtual void OnHitEnemy(GameObject enemy)
//     {
//         Health enemyHealth = enemy.GetComponent<Health>();
//         if (enemyHealth != null)
//         {
//             enemyHealth.TakeDamage(damage);
//         }
//         Destroy(gameObject);
//     }
// }