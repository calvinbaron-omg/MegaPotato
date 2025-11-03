using UnityEngine;
using System.Collections;

public class SmiteBehavior : MonoBehaviour
{
    private bool allowOverflowDamage = false;
    private float aoeDamage;
    private float aoeRadius;
    private float critChance;
    private float critDamage;
    private int pulseCount = 1;
    private float pulseDelay = 0.05f;
    [SerializeField] private float lifetime = 0.6f;
    private float fadeSpeed = 3f;
    private Material mat;
    private Color startColor;
    private GameObject prefabRef;

    public void Initialize(float dmg, float radius, float critCh, float critMult, int projectiles = 1, GameObject prefabReference = null)
    {
        aoeDamage = dmg;
        aoeRadius = radius;
        critChance = critCh;
        critDamage = critMult;
        pulseCount = Mathf.Max(1, projectiles);
        prefabRef = prefabReference;

        mat = GetComponent<MeshRenderer>()?.material;
        if (mat != null) startColor = mat.color;

        StopAllCoroutines();
        StartCoroutine(DoSmitePulses());
    }

    private IEnumerator DoSmitePulses()
    {
        for (int i = 0; i < pulseCount; i++)
        {
            ApplyAoEDamage();

            if (mat != null)
                mat.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

            yield return new WaitForSeconds(pulseDelay);
        }

        yield return new WaitForSeconds(lifetime);
        ProjectilePool.Instance.Return(prefabRef, gameObject);
    }

    private void Update()
    {
        if (mat == null) return;
        Color c = mat.color;
        c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.deltaTime);
        mat.color = c;
    }

    private void ApplyAoEDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Health health = hit.GetComponent<Health>();
            if (health == null) continue;

            float dmg = aoeDamage;
            float remainingChance = critChance;
            int guaranteedCrits = Mathf.FloorToInt(remainingChance);
            float extraChance = remainingChance - guaranteedCrits;

            if (guaranteedCrits >= 20 && !allowOverflowDamage)
                guaranteedCrits = 20;

            for (int i = 0; i < guaranteedCrits; i++)
                dmg *= critDamage;

            if (Random.value < extraChance)
                dmg *= critDamage;

            bool isCrit = (guaranteedCrits > 0) || (Random.value < extraChance);
            health.TakeDamage(dmg, isCrit, "Smite");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
