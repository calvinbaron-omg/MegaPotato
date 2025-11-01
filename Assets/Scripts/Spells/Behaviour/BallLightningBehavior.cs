using UnityEngine;

public class BallLightningBehavior : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    private float lifetime;
    private float baseDamage;
    private float shockChance;
    private float shockAmount;
    private float shockDuration;
    private float aoeRadius;
    private float critChance;
    private float critDamageMultiplier;

    private float extraProjBounce;
    private float extraProjCount;
    private float remainingBounces;

    public void Initialize(
        Vector3 direction,
        float spd,
        float life,
        float baseDmg,
        float shockCh,
        float shockAmt,
        float shockDur,
        float aoeRad,
        float critCh,
        float critMult,
        float projBounce,
        float projCount
    )
    {
        moveDirection = direction.normalized;
        speed = spd;
        lifetime = life;
        baseDamage = baseDmg;
        shockChance = shockCh;
        shockAmount = shockAmt;
        shockDuration = shockDur;
        aoeRadius = aoeRad;
        critChance = critCh;
        critDamageMultiplier = critMult;
        extraProjBounce = Mathf.FloorToInt(projBounce);
        extraProjCount = projCount;
        remainingBounces = extraProjBounce;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Projectile"))
            return;

        if (collision.collider.CompareTag("Enemy"))
        {
            ApplyDamage();

            if (remainingBounces <= 0)
            {
                Destroy(gameObject);
                return;
            }

            remainingBounces--;
            // ⚡ Add slight random deviation for chaotic bounce
            Vector3 normal = collision.contacts[0].normal;
            moveDirection = Vector3.Reflect(moveDirection, normal);
            moveDirection = Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * moveDirection;
            return;
        }

        // Hit environment / walls
        if (remainingBounces > 0)
        {
            remainingBounces--;
            Vector3 normal = collision.contacts[0].normal;
            moveDirection = Vector3.Reflect(moveDirection, normal);
            moveDirection = Quaternion.Euler(0, Random.Range(-10f, 10f), 0) * moveDirection;
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

            // ⚡ Shock effect (slow or stun)
            if (Random.value <= shockChance)
            {
                EnemyStatus status = enemy.GetComponent<EnemyStatus>();
                status?.ApplySlow(shockAmount, shockDuration);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
