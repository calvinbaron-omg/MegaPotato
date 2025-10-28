using UnityEngine;
using System.Collections.Generic;

public class SpellBallLightning : BaseProjectileSpell
{
    [Header("Ball Lightning Settings")]
    [SerializeField] private float shockChance = 0.25f;
    [SerializeField] private float shockAmount = 1.0f;
    [SerializeField] private float shockDuration = 1.5f;

    
    public override void CastSpell(Transform caster, Vector3 targetPosition)
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

        GameObject fireball = CreateProjectile(caster, dir, heightOffset);
        FireballBehavior behavior = fireball.AddComponent<FireballBehavior>();

        behavior.Initialize(
            dir,
            effective.projectileSpeed,
            effective.lifetime,
            effective.damage,
            shockChance,
            shockAmount,
            shockDuration,
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
