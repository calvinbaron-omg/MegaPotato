using UnityEngine;

public class BallLightningBehavior : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private float lifetime;
    private float baseDamage;
    private float shockChance;     // chance to stun or apply heavy slow
    private float shockAmount;     // can represent stun intensity or slow %
    private float shockDuration;
    private float aoeRadius;
    private float critChance;
    private float critDamageMultiplier;

    public void Initialize(
        Vector3 targetPos,
        float spd,
        float life,
        float baseDmg,
        float shockCh,
        float shockAmt,
        float shockDur,
        float aoeRad,
        float critCh = 0f,
        float critMult = 1f
    )
    {
        targetPosition = targetPos;
        speed = spd;
        lifetime = life;
        baseDamage = baseDmg;
        shockChance = shockCh;
        shockAmount = shockAmt;
        shockDuration = shockDur;
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
        if (other.CompareTag("Projectile") || other.CompareTag("Player"))
            return;

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

            float damage = baseDamage;

            // Crit roll
            bool isCrit = Random.value < critChance;
            if (isCrit)
                damage *= critDamageMultiplier;

            health.TakeDamage(damage);

            // Shock (stun/slow) roll
            if (Random.value <= shockChance)
                ApplyShock(enemy.gameObject);
        }
    }

    private void ApplyShock(GameObject enemy)
    {
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        // You can treat shockAmount = 1f as a full stun
        status?.ApplySlow(shockAmount, shockDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
