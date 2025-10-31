using UnityEngine;
using System.Collections;

public class SmiteBehavior : MonoBehaviour
{
    //TODO Acutally make meta unlocks
    [Header("Meta Unlock")]
    private bool allowOverflowDamage = false;
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

            // MEGA BONK
            float remainingChance = critChance;   // e.g. 1.10f for 110%
            int guaranteedCrits = Mathf.FloorToInt(remainingChance); // every full 100% = guaranteed
            float extraChance = remainingChance - guaranteedCrits;
            //Dont allow more than 20 mega bonks
            if(guaranteedCrits >= 20 && allowOverflowDamage == false)
            {
                guaranteedCrits = 20;
            }
            // Apply guaranteed crit layers
            for (int i = 0; i < guaranteedCrits; i++)
                dmg *= critDamage;

            // Roll one more time for the fractional part
            if (Random.value < extraChance)
                dmg *= critDamage;
            // Pass in whether we crit at least once (for visuals)
            bool isCrit = (guaranteedCrits > 0) || (Random.value < extraChance);
            health.TakeDamage(dmg, isCrit);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
