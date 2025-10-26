using UnityEngine;
using System.Collections.Generic;

public class SpellFireball : BaseProjectileSpell
{
    [Header("Fireball Settings")]
    [SerializeField] private float burnChance = 0.4f;
    [SerializeField] private float burnDamage = 5f;
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float aoeRadius = 2.5f;

    [Header("Critical Stats")]
    [SerializeField] private float critChance = 0.05f;
    [SerializeField] private float critDamageMultiplier = 2.0f;

    [Header("Projectile Count")]
    [SerializeField] private int projectileCount = 1;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        // Later, for multiple projectiles, loop here and angle slightly.
        GameObject fireball = CreateProjectile(caster, targetPosition);

        FireballBehavior behavior = fireball.AddComponent<FireballBehavior>();
        behavior.Initialize(
            targetPosition,
            projectileSpeed,
            lifetime,
            baseDamage,
            burnChance,
            burnDamage,
            burnDuration,
            aoeRadius,
            critChance,
            critDamageMultiplier
        );
    }

    // ===== Upgrade System Overrides =====
    public override List<SpellStatType> GetUpgradeableStats()
    {
        return new List<SpellStatType>
        {
            SpellStatType.Damage,
            SpellStatType.ProjectileSpeed,
            SpellStatType.Size,
            SpellStatType.CritChance,
            SpellStatType.CritDamage,
            SpellStatType.ProjectileCount
        };
    }

    public override float GetBaseUpgradeValue(SpellStatType statType)
    {
        switch (statType)
        {
            case SpellStatType.Damage: return 0.10f;
            case SpellStatType.ProjectileSpeed: return 0.10f;
            case SpellStatType.Size: return 0.10f;
            case SpellStatType.CritChance: return 0.05f;
            case SpellStatType.CritDamage: return 0.20f;
            default: return 0f;
        }
    }

    public override (bool isFlat, int flatAmount) GetFlatUpgradeInfo(SpellStatType statType)
    {
        if (statType == SpellStatType.ProjectileCount)
            return (true, 1);
        return base.GetFlatUpgradeInfo(statType);
    }

    public override void ApplyStatUpgrade(SpellStatType statType, float effectiveValue, int flatAmountIfAny = 0)
    {
        switch (statType)
        {
            case SpellStatType.Damage:
                baseDamage *= (1f + effectiveValue);
                break;
            case SpellStatType.ProjectileSpeed:
                projectileSpeed *= (1f + effectiveValue);
                break;
            case SpellStatType.Size:
                aoeRadius *= (1f + effectiveValue);
                break;
            case SpellStatType.CritChance:
                critChance += effectiveValue;
                break;
            case SpellStatType.CritDamage:
                critDamageMultiplier *= (1f + effectiveValue);
                break;
            case SpellStatType.ProjectileCount:
                projectileCount += flatAmountIfAny;
                break;
            case SpellStatType.AttackSpeed:
                baseCooldown *= (1f - effectiveValue);
                break;
        }
    }

    // Optional UI helpers
    public float GetCritChance() => critChance;
    public float GetCritDamageMultiplier() => critDamageMultiplier;
    public float GetAoeRadius() => aoeRadius;
    public int GetProjectileCount() => projectileCount;
}
