using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    private float lifetime;
    private float baseDamage;
    private float burnChance;
    private float burnDamage;
    private float burnDuration;
    private float aoeRadius;
    private float critChance;
    private float critDamageMultiplier;

    public void Initialize(
        Vector3 direction,
        float spd,
        float life,
        float baseDmg,
        float burnCh,
        float burnDmg,
        float burnDur,
        float aoeRad,
        float critCh = 0f,
        float critMult = 1f
    )
    {
        moveDirection = direction.normalized;
        speed = spd;
        lifetime = life;
        baseDamage = baseDmg;
        burnChance = burnCh;
        burnDamage = burnDmg;
        burnDuration = burnDur;
        aoeRadius = aoeRad;
        critChance = critCh;
        critDamageMultiplier = critMult;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile") || other.CompareTag("Player")) return;

        if (other.CompareTag("Enemy"))
        {
            ApplyDamage();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ApplyDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (!enemy.CompareTag("Enemy")) continue;

            Health health = enemy.GetComponent<Health>();
            if (health == null) continue;

            float damage = baseDamage;
            bool isCrit = Random.value < critChance;
            if (isCrit)
                damage *= critDamageMultiplier;

            health.TakeDamage(damage, isCrit);

            if (Random.value <= burnChance)
            {
                EnemyStatus status = enemy.GetComponent<EnemyStatus>();
                status?.ApplyBurn(burnDamage, burnDuration);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
