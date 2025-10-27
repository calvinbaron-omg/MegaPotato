using UnityEngine;

public class FrozenOrbBehavior : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private float lifetime;
    private float baseDamage;
    private float slowChance;
    private float slowAmount;
    private float slowDuration;
    private float aoeRadius;
    private float critChance;
    private float critDamageMultiplier;

    public void Initialize(
        Vector3 targetPos,
        float spd,
        float life,
        float baseDmg,
        float slowCh,
        float slowAmt,
        float slowDur,
        float aoeRad,
        float critCh,
        float critMult
    )
    {
        targetPosition = targetPos;
        speed = spd;
        lifetime = life;
        baseDamage = baseDmg;
        slowChance = slowCh;
        slowAmount = slowAmt;
        slowDuration = slowDur;
        aoeRadius = aoeRad;
        critChance = critCh;
        critDamageMultiplier = critMult;

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
            ApplyAOEEffect();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ApplyAOEEffect()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (!enemy.CompareTag("Enemy")) continue;
            
            Health health = enemy.GetComponent<Health>();
            if (health == null) continue;

            // crit roll
            float damageToApply = baseDamage;
            bool isCrit = Random.value < critChance;
            if (isCrit)
            {
                damageToApply *= critDamageMultiplier;
                // TODO: spawn crit VFX / popup text etc.
            }

            health.TakeDamage(damageToApply);

            // slow roll
            if (Random.value <= slowChance)
            {
                ApplySlow(enemy.gameObject);
            }
        }
    }

    private void ApplySlow(GameObject enemy)
    {
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        status?.ApplySlow(slowAmount, slowDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
