using UnityEngine;
using System.Collections.Generic;

public class SpellSmite : BaseProjectileSpell
{
    [Header("Smite Settings")]
    [SerializeField] private GameObject smiteImpactPrefab; // prefab with SmiteBehavior attached
    [SerializeField] private float rangeMultiplier = 1.2f;
    [SerializeField] private float multiStrikeDelay = 0.05f; // slight delay between strikes

    public override void CastSpell(Transform caster, Vector3 targetPosition, Vector3 targetScale)
    {
        PlayerStats stats = caster.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats missing on caster!");
            return;
        }

        SpellRuntimeStats effective = CalculateEffectiveStats(stats);
        int strikeCount = Mathf.Max(1, Mathf.FloorToInt(effective.count)); // number of smite hits

        // Find target enemy based on PlayerAutoAttack's position
        Collider[] hits = Physics.OverlapSphere(targetPosition, targetScale.y * 0.5f);
        Transform enemy = null;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                enemy = hit.transform;
                break;
            }
        }
        if (enemy == null) return;

       caster.GetComponent<PlayerAutoAttack>()?.RunSpellCoroutine(SmiteSequence(enemy, effective, strikeCount));

    }

    private System.Collections.IEnumerator SmiteSequence(Transform enemy, SpellRuntimeStats effective, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (enemy == null) yield break;

            Health health = enemy.GetComponent<Health>();
            if (health == null) yield break;

            float damage = effective.damage;
            bool isCrit = Random.value < effective.critChance;
            if (isCrit)
                damage *= effective.critDamage;

            health.TakeDamage(damage, isCrit);

            // Spawn AoE impact each strike
            if (smiteImpactPrefab != null)
            {
                GameObject impact = Instantiate(smiteImpactPrefab, enemy.position, Quaternion.identity);
                SmiteBehavior smite = impact.GetComponent<SmiteBehavior>();
                if (smite != null)
                {
                    smite.Initialize(
                    effective.damage,
                    effective.size,
                    effective.critChance,
                    effective.critDamage,
                    Mathf.FloorToInt(effective.count)
                );

                }
            }

            yield return new WaitForSeconds(multiStrikeDelay);
        }
    }

    public override List<SpellStatType> GetUpgradeableStats() =>
        new List<SpellStatType> {
            SpellStatType.Damage,
            SpellStatType.Size,
            SpellStatType.AttackSpeed,
            SpellStatType.CritChance,
            SpellStatType.CritDamage,
            SpellStatType.ProjectileCount 
        };

    public override float GetBaseUpgradeValue(SpellStatType statType)
    {
        switch (statType)
        {
            case SpellStatType.Damage: return 0.10f;
            case SpellStatType.Size: return 0.10f;
            case SpellStatType.AttackSpeed: return 0.10f;
            case SpellStatType.CritChance: return 0.05f;
            case SpellStatType.CritDamage: return 0.20f;
            case SpellStatType.ProjectileCount: return 1f;
            default: return 0f;
        }
    }

    public override void ApplyStatUpgrade(SpellStatType statType, float val, float flat = 0)
    {
        switch (statType)
        {
            case SpellStatType.Damage: upgradeDamageMult *= (1f + val); break;
            case SpellStatType.Size: upgradeSizeMult *= (1f + val); break;
            case SpellStatType.AttackSpeed: upgradeAttackSpeedMult *= (1f + val); break;
            case SpellStatType.CritChance: upgradeCritChanceBonus += val; break;
            case SpellStatType.CritDamage: upgradeCritDamageMult *= (1f + val); break;
            case SpellStatType.ProjectileCount: upgradeProjectileCount += flat; break;
        }
    }
}
