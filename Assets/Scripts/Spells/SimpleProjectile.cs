// using UnityEngine;

// public class SimpleProjectile : MonoBehaviour
// {
//     private Vector3 direction;
//     private float speed;
//     private float damage;
//     private float lifetime;
//     private ISpell spell;
    
//     public void Initialize(Vector3 dir, float spd, float dmg, float life, ISpell spellRef)
//     {
//         direction = dir;
//         speed = spd;
//         damage = dmg;
//         lifetime = life;
//         spell = spellRef;
        
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
//             Health enemyHealth = other.GetComponent<Health>();
//             if (enemyHealth != null)
//             {
//                 enemyHealth.TakeDamage(damage);
//                 OnHitEnemy(other.gameObject);
//             }
//         }
        
//         Destroy(gameObject);
//     }
    
//     protected virtual void OnHitEnemy(GameObject enemy)
//     {
//         // Override in specific spell projectiles for custom behavior
//     }
// }