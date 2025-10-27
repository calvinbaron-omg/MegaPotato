using UnityEngine;
using System.Collections.Generic;

public class SpellFireball : BaseProjectileSpell
{
    [Header("Fireball Settings")]
    [SerializeField] private float burnChance = 0.4f;
    [SerializeField] private float burnDamage = 5f;
    [SerializeField] private float burnDuration = 3f;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        PlayerStats stats = caster.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats missing on caster!");
            return;
        }

        SpellRuntimeStats effective = CalculateEffectiveStats(stats);

        GameObject fireball = CreateProjectile(caster, targetPosition);
        FireballBehavior behavior = fireball.AddComponent<FireballBehavior>();

        behavior.Initialize(
            targetPosition,
            effective.projectileSpeed,
            effective.lifetime,
            effective.damage,
            burnChance,
            burnDamage,
            burnDuration,
            effective.aoe,
            effective.critChance,
            effective.critDamage
        );
    }


    public override List<SpellStatType> GetUpgradeableStats() =>
        new List<SpellStatType> {
            SpellStatType.Damage,
            SpellStatType.Size,
            SpellStatType.CritChance,
            SpellStatType.CritDamage,
            SpellStatType.AttackSpeed
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
            default: return 0f;
        }
    }

    public override void ApplyStatUpgrade(SpellStatType statType, float val, int flat = 0)
    {
        switch (statType)
        {
            case SpellStatType.Damage: upgradeDamageMult *= (1f + val); break;
            case SpellStatType.Size: upgradeSizeMult *= (1f + val); break;
            case SpellStatType.CritChance: upgradeCritChanceBonus += val; break;
            case SpellStatType.CritDamage: upgradeCritDamageMult *= (1f + val); break;
            case SpellStatType.AttackSpeed: upgradeAttackSpeedMult *= (1f + val); break;
        }
    }
}
