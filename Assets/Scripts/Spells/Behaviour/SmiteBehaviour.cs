using UnityEngine;
using System.Collections;

public class SmiteBehavior : MonoBehaviour
{
    private float aoeDamage;
    private float aoeRadius;
    private float critChance;
    private float critDamage;
    private int pulseCount = 1;
    private float pulseDelay = 0.05f; // delay between AoE pulses

    [SerializeField] private float lifetime = 0.6f;
    private float fadeSpeed = 3f;
    private Material mat;
    private Color startColor;

    public void Initialize(float dmg, float radius, float critCh, float critMult, int projectiles = 1)
    {
        aoeDamage = dmg;
        aoeRadius = radius;
        critChance = critCh;
        critDamage = critMult;
        pulseCount = Mathf.Max(1, projectiles);

        mat = GetComponent<MeshRenderer>()?.material;
        if (mat != null) startColor = mat.color;

        StartCoroutine(DoSmitePulses());
    }

    private IEnumerator DoSmitePulses()
    {
        for (int i = 0; i < pulseCount; i++)
        {
            ApplyAoEDamage();

            if (mat != null)
            {
                // brief flash for each pulse
                mat.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
            }

            yield return new WaitForSeconds(pulseDelay);
        }

        Destroy(gameObject, lifetime);
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
            bool isCrit = Random.value < critChance;
            if (isCrit)
                dmg *= critDamage;

            health.TakeDamage(dmg, isCrit);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
