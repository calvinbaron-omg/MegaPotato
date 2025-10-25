using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private float lifetime;
    private float baseDamage;
    private float burnChance;
    private float burnDamage;
    private float burnDuration;
    private float aoeRadius;

    public void Initialize(Vector3 targetPos, float spd, float life, float baseDmg, float burnCh, float burnDmg, float burnDur, float aoeRad)
    {
        targetPosition = targetPos;
        speed = spd;
        lifetime = life;
        baseDamage = baseDmg;
        burnChance = burnCh;
        burnDamage = burnDmg;
        burnDuration = burnDur;
        aoeRadius = aoeRad;
        
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile") || other.CompareTag("Player")) return;
        
        if (other.CompareTag("Enemy"))
        {
            ApplyAOEDamage();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ApplyAOEDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Health health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    // Apply base damage
                    health.TakeDamage(baseDamage);
                    
                    // Apply burn effect
                    if (Random.value <= burnChance)
                    {
                        ApplyBurn(enemy.gameObject);
                    }
                }
            }
        }
    }

    private void ApplyBurn(GameObject enemy)
    {
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        status?.ApplyBurn(burnDamage, burnDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}