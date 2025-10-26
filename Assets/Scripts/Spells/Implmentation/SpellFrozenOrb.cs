using UnityEngine;
using System.Collections.Generic;

public class SpellFrozenOrb : BaseProjectileSpell
{
    [Header("Frozen Orb Settings")]
    [SerializeField] private float slowChance = 0.3f;
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float aoeRadius = 3f;

    [Header("Critical Stats")]
    [SerializeField] private float critChance = 0.05f;           // 5% base crit
    [SerializeField] private float critDamageMultiplier = 2.0f;  // 200% damage on crit (i.e. +100%)

    // OPTIONAL: if Frozen Orb can fire multiple projectiles per cast later
    [Header("Projectile Count")]
    [SerializeField] private int projectileCount = 1;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        // This example still just creates one orb. If you later add projectileCount,
        // you'd loop projectileCount times and maybe angle them slightly.
        GameObject frozenOrb = CreateProjectile(caster, targetPosition);

        FrozenOrbBehavior behavior = frozenOrb.AddComponent<FrozenOrbBehavior>();
        behavior.Initialize(
            targetPosition,
            projectileSpeed,
            lifetime,
            baseDamage,
            slowChance,
            slowAmount,
            slowDuration,
            aoeRadius,
            critChance,
            critDamageMultiplier
        );
    }

    // ===== Upgrade System Overrides =====

    // Which stats can Frozen Orb upgrade?
    public override List<SpellStatType> GetUpgradeableStats()
    {
        // You can include/exclude things easily per spell
        List<SpellStatType> stats = new List<SpellStatType>()
        {
            SpellStatType.Damage,
            SpellStatType.ProjectileSpeed,
            SpellStatType.Size,
            SpellStatType.CritChance,
            SpellStatType.CritDamage,
            // SpellStatType.ProjectileCount, // include once you want +projectiles
        };

        return stats;
    }

    // Base per-upgrade value for each stat, BEFORE rarity multiplier.
    // e.g. Damage 0.10f means "10% damage" at Common.
    public override float GetBaseUpgradeValue(SpellStatType statType)
    {
        switch (statType)
        {
            case SpellStatType.Damage:
                return 0.10f; // +10% damage (Common)
            case SpellStatType.ProjectileSpeed:
                return 0.10f; // +10% projectile speed
            case SpellStatType.Size:
                return 0.10f; // +10% AoE radius/size
            case SpellStatType.CritChance:
                return 0.05f; // +5% crit chance
            case SpellStatType.CritDamage:
                return 0.20f; // +20% crit damage multiplier
            case SpellStatType.AttackSpeed:
                // Frozen Orb doesn't really care about attack speed itself
                // (attack speed is more global), so we could return 0
                return 0f;
            case SpellStatType.ProjectileCount:
                // not percentage-based; handled in GetFlatUpgradeInfo()
                return 0f;
            default:
                return 0f;
        }
    }

    // If a stat is flat (like +1 projectile), define it here.
    public override (bool isFlat, int flatAmount) GetFlatUpgradeInfo(SpellStatType statType)
    {
        if (statType == SpellStatType.ProjectileCount)
        {
            // Base "Common" upgrade is +1 projectile
            return (true, 1);
        }

        return base.GetFlatUpgradeInfo(statType);
    }

    // Actually apply the rolled upgrade to this spell's live stats.
    public override void ApplyStatUpgrade(SpellStatType statType, float effectiveValue, int flatAmountIfAny = 0)
    {
        switch (statType)
        {
            case SpellStatType.Damage:
                // scale damage multiplicatively
                baseDamage *= (1f + effectiveValue);
                break;

            case SpellStatType.ProjectileSpeed:
                projectileSpeed *= (1f + effectiveValue);
                break;

            case SpellStatType.Size:
                // Increase AoE radius for Frozen Orb = "size"
                aoeRadius *= (1f + effectiveValue);
                break;

            case SpellStatType.CritChance:
                // critChance is additive. 0.05f means +5% absolute.
                critChance += effectiveValue;
                break;

            case SpellStatType.CritDamage:
                // critDamageMultiplier increases multiplicatively.
                // if base is 2.0 and effectiveValue=0.20f (20%), result 2.4
                critDamageMultiplier *= (1f + effectiveValue);
                break;

            case SpellStatType.ProjectileCount:
                if (flatAmountIfAny > 0)
                {
                    projectileCount += flatAmountIfAny;
                }
                break;

            case SpellStatType.AttackSpeed:
                // Frozen Orb doesn't really own attack speed (that's globalAttackSpeed on PlayerAutoAttack)
                // You *could* hook this to reduce baseCooldown instead to simulate "cast more often"
                baseCooldown *= (1f - effectiveValue); 
                break;
        }
    }

    // (Optional) expose some read info to UI if you want,
    // like current critChance, level, etc.
    public float GetCritChance() => critChance;
    public float GetCritDamageMultiplier() => critDamageMultiplier;
    public float GetAoeRadius() => aoeRadius;
    public int GetProjectileCount() => projectileCount;
}
