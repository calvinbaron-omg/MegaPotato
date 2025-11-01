using UnityEngine;
using System.Collections;

public class AuraBehavior : MonoBehaviour
{
    private float damage;
    private float radius;
    private float critChance;
    private float critDamage;
    private float tickInterval;

    private float timer;

    private SphereCollider auraCollider;
    private ParticleSystem auraVFX;

    public void Initialize(
        float dmg,
        float size,
        float critCh,
        float critMult,
        float baseTick,
        float attackSpeed
    )
    {
        damage = dmg;
        radius = size;
        critChance = critCh;
        critDamage = critMult;

        // Attack speed increases tick rate (faster ticks = smaller interval)
        tickInterval = baseTick / attackSpeed;

        SetupCollider();
        SetupVFX();
    }

    void SetupCollider()
    {
        auraCollider = gameObject.AddComponent<SphereCollider>();
        auraCollider.isTrigger = true;
        auraCollider.radius = radius; // base radius

        // Explicitly cancel out parent scaling so it's world accurate
        float inverseParentScale = 1f / transform.lossyScale.x;
        auraCollider.radius *= inverseParentScale;
    }

    public void Refresh(SpellRuntimeStats newStats)
    {
        damage = newStats.damage;
        radius = newStats.size;
        critChance = newStats.critChance;
        critDamage = newStats.critDamage;
        tickInterval = 1f / newStats.attackSpeed;

        if (auraCollider != null)
        {
            float inverseParentScale = 1f / transform.lossyScale.x;
            auraCollider.radius = radius * inverseParentScale;
        }

        if (auraVFX != null)
        {
            auraVFX.transform.localScale = Vector3.one * (radius * 2f);
        }

        // Update line renderer circle too (keep visual synced)
        UpdateRingVisual();
    }


    void SetupVFX()
    {
        // Create an empty child object for the ring
        GameObject ring = new GameObject("AuraRing");
        ring.transform.SetParent(transform);
        ring.transform.localPosition = Vector3.zero;
        ring.transform.localRotation = Quaternion.identity;

        // Add a LineRenderer
        LineRenderer lr = ring.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = 64; // smooth circle
        lr.widthMultiplier = 0.1f; // line thickness

        // Use a simple unlit transparent material
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = new Color(0.2f, 0.8f, 1f, 0.8f); // cyan glow
        lr.material = mat;

        // Draw a circle using points
        float step = 2 * Mathf.PI / lr.positionCount;
        Vector3[] positions = new Vector3[lr.positionCount];
        for (int i = 0; i < lr.positionCount; i++)
        {
            float angle = i * step;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            positions[i] = new Vector3(x, 0f, z); // flat on the ground
        }
        lr.SetPositions(positions);
    }

    void UpdateRingVisual()
    {
        LineRenderer lr = GetComponentInChildren<LineRenderer>();
        if (lr == null) return;

        float step = 2 * Mathf.PI / lr.positionCount;
        Vector3[] positions = new Vector3[lr.positionCount];
        for (int i = 0; i < lr.positionCount; i++)
        {
            float angle = i * step;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            positions[i] = new Vector3(x, 0f, z);
        }
        lr.SetPositions(positions);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            TickDamage();
            timer = 0f;
        }
    }

    void TickDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Health health = hit.GetComponent<Health>();
            if (health == null) continue;

            float dmg = damage;
            bool isCrit = Random.value < critChance;
            if (isCrit)
                dmg *= critDamage;

            health.TakeDamage(dmg, isCrit);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
