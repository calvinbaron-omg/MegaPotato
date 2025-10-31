using UnityEngine;
using System.Collections.Generic;

public class SpellFrozenOrb : BaseProjectileSpell
{
    [Header("Frozen Orb Settings")]
    [SerializeField] private float slowChance = 0.3f;
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float slowDuration = 2f;

   
    public override void CastSpell(Transform caster, Vector3 targetPosition, Vector3 targetScale)
    {
        PlayerStats stats = caster.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats missing on caster!");
            return;
        }
        //PLayer y bottom = 0.1 jump y goes to 3 or 4
        //Enemy y bottom = 1 
        //Some upgrades create wonky effective stats. Will need to check it. 
        SpellRuntimeStats effective = CalculateEffectiveStats(stats);

        float heightOffset = 1.1f;
        Vector3 start = caster.position + Vector3.up * heightOffset;
        //Green enemies are at targetpostition y = +- 0.2 that is 0.1 * scale.y(2)
        //Red enemies are at targetpostition y = +- 0.1 that is 0.1 * scale.y(1)
        float enemyScaleOffset = (0.1f * targetScale.y) - 0.1f;
        Vector3 endTemp = targetPosition + Vector3.up * heightOffset;
        Vector3 end = new Vector3(endTemp.x, endTemp.y - enemyScaleOffset, endTemp.z);
        Vector3 dir = (end - start).normalized;

        int projectileCount = Mathf.Max(1, Mathf.FloorToInt(effective.count));
        float baseAngle = 0f;         // the center direction
        float angleStep = 10f;        // degrees between projectiles (tweak per spell)

        // alternate angles: 0°, +10°, -10°, +20°, -20°, etc.
        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset;

            if (i == 0)
            {
                angleOffset = baseAngle; // first one goes straight
            }
            else
            {
                int pairIndex = (i + 1) / 2; // increases every two projectiles
                angleOffset = pairIndex * angleStep * (i % 2 == 0 ? -1 : 1);
            }

            Vector3 spreadDir = Quaternion.Euler(0, angleOffset, 0) * dir;

            GameObject projectile = CreateProjectile(caster, spreadDir, heightOffset);
            FrozenOrbBehavior behavior = projectile.AddComponent<FrozenOrbBehavior>();

            behavior.Initialize(
                spreadDir,
                effective.projectileSpeed,
                effective.lifetime,
                effective.damage,
                slowChance,
                slowAmount,
                slowDuration,
                effective.aoe,
                effective.critChance,
                effective.critDamage,
                effective.bounces,
                effective.count
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
            SpellStatType.ProjectileBounce,
            SpellStatType.ProjectileCount
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
            case SpellStatType.ProjectileCount: return 0f; // each rarity adds 1-2 extra
            case SpellStatType.ProjectileBounce: return 0f;
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
