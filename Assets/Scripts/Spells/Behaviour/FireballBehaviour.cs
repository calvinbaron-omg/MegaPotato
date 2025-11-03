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
    private float extraProjBounce;
    private float extraProjCount;
    private float remainingBounces;

    private float lifeTimer;
    private bool active;
    private GameObject prefabRef;

    public void Initialize(
        Vector3 direction,
        float spd,
        float life,
        float baseDmg,
        float burnCh,
        float burnDmg,
        float burnDur,
        float aoeRad,
        float critCh,
        float critMult,
        float projBounce,
        float projCount,
        GameObject prefabReference = null)
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
        extraProjBounce = Mathf.FloorToInt(projBounce);
        extraProjCount = projCount;
        remainingBounces = extraProjBounce;
        lifeTimer = 0f;
        active = true;
        prefabRef = prefabReference;
    }

    void Update()
    {
        if (!active) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
        {
            ReturnToPool();
            return;
        }

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!active) return;
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Projectile"))
            return;

        if (collision.collider.CompareTag("Enemy"))
        {
            ApplyDamage();

            if (remainingBounces <= 0)
            {
                ReturnToPool();
                return;
            }

            remainingBounces--;
            moveDirection = Vector3.Reflect(moveDirection, collision.contacts[0].normal);
            return;
        }

        if (remainingBounces > 0)
        {
            remainingBounces--;
            moveDirection = Vector3.Reflect(moveDirection, collision.contacts[0].normal);
        }
        else
        {
            ReturnToPool();
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

            health.TakeDamage(damage, isCrit, "Fire Ball");

            if (Random.value <= burnChance)
            {
                EnemyStatus status = enemy.GetComponent<EnemyStatus>();
                status?.ApplyBurn(burnDamage, burnDuration);
            }
        }
    }

    private void ReturnToPool()
    {
        active = false;
        ProjectilePool.Instance.Return(prefabRef, gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
