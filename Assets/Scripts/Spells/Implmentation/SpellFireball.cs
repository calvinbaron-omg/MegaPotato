using UnityEngine;
using System.Collections.Generic;

public class SpellFireball : BaseProjectileSpell
{
    [Header("Fireball Settings")]
    [SerializeField] private float burnChance = 0.4f;
    [SerializeField] private float burnDamage = 5f;
    [SerializeField] private float burnDuration = 3f;

    public override void CastSpell(Transform caster, Vector3 targetPosition, Vector3 targetScale)
    {
        PlayerStats stats = caster.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats missing on caster!");
            return;
        }

        SpellRuntimeStats effective = CalculateEffectiveStats(stats);

        float heightOffset = 1.1f;
        Vector3 start = caster.position + Vector3.up * heightOffset;
        Vector3 end = targetPosition + Vector3.up * heightOffset;
        Vector3 dir = (end - start).normalized;

        // 🔹 Calculate projectile spread pattern
        int projectileCount = Mathf.Max(1, Mathf.FloorToInt(effective.count));
        float baseAngle = 0f;
        float angleStep = 10f; // Fireballs spread wider visually

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset;
            if (i == 0)
            {
                angleOffset = baseAngle; // First fires straight
            }
            else
            {
                int pairIndex = (i + 1) / 2;
                angleOffset = pairIndex * angleStep * (i % 2 == 0 ? -1 : 1);
            }

            Vector3 spreadDir = Quaternion.Euler(0, angleOffset, 0) * dir;

            GameObject projectile = CreateProjectile(caster, spreadDir, heightOffset);
            FireballBehavior behavior = projectile.AddComponent<FireballBehavior>();

            behavior.Initialize(
                spreadDir,
                effective.projectileSpeed,
                effective.lifetime,
                effective.damage,
                burnChance,
                burnDamage,
                burnDuration,
                effective.aoe,
                effective.critChance,
                effective.critDamage,
                effective.bounces,
                effective.count,
                spellProjectilePrefab
            );
        }
    }

    public override List<SpellStatType> GetUpgradeableStats() =>
        new List<SpellStatType> {
            SpellStatType.Damage,
            SpellStatType.Size,
            SpellStatType.CritChance,
            SpellStatType.CritDamage,
            SpellStatType.AttackSpeed,
            SpellStatType.ProjectileCount,
            SpellStatType.ProjectileBounce
        };

    public override float GetBaseUpgradeValue(SpellStatType statType)
    {
        switch (statType)
        {
            case SpellStatType.Damage: return 0.10f;
            case SpellStatType.Size: return 0.10f;
            case SpellStatType.CritChance: return 0.05f;
            case SpellStatType.CritDamage: return 0.20f;
            case SpellStatType.AttackSpeed: return 0.10f;
            case SpellStatType.ProjectileCount: return 1f; // each rarity adds 1-2 extra
            case SpellStatType.ProjectileBounce: return 1f;
            default: return 0f;
        }
    }

    public override (bool isFlat, int flatAmount) GetFlatUpgradeInfo(SpellStatType statType)
    {
        switch (statType)
        {
            case SpellStatType.ProjectileCount: return (true, 1);
            case SpellStatType.ProjectileBounce: return (true, 1);
            default: return base.GetFlatUpgradeInfo(statType);
        }
    }

    public override void ApplyStatUpgrade(SpellStatType statType, float val, float flat = 0)
    {
        switch (statType)
        {
            case SpellStatType.Damage: upgradeDamageMult *= (1f + val); break;
            case SpellStatType.Size: upgradeSizeMult *= (1f + val); break;
            case SpellStatType.CritChance: upgradeCritChanceBonus += val; break;
            case SpellStatType.CritDamage: upgradeCritDamageMult *= (1f + val); break;
            case SpellStatType.AttackSpeed: upgradeAttackSpeedMult *= (1f + val); break;
            case SpellStatType.ProjectileCount: upgradeProjectileCount += flat; break;
            case SpellStatType.ProjectileBounce: upgradeProjectileBounce += flat; break;
        }
    }

}
