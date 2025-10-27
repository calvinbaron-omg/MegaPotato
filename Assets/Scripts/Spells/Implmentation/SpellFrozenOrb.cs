using UnityEngine;
using System.Collections.Generic;

public class SpellFrozenOrb : BaseProjectileSpell
{
    [Header("Frozen Orb Settings")]
    [SerializeField] private float slowChance = 0.3f;
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float slowDuration = 2f;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        PlayerStats stats = caster.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats missing on caster!");
            return;
        }

        var effective = CalculateEffectiveStats(stats);

        GameObject orb = CreateProjectile(caster, targetPosition);
        FrozenOrbBehavior behavior = orb.AddComponent<FrozenOrbBehavior>();

        behavior.Initialize(
            targetPosition,
            effective.projectileSpeed,
            lifetime,
            effective.damage,
            slowChance,
            slowAmount,
            slowDuration,
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
            case SpellStatType.Damage: spellDamageMultiplier *= (1f + val); break;
            case SpellStatType.Size: spellSizeMultiplier *= (1f + val); break;
            case SpellStatType.CritChance: spellCritChanceBonus += val; break;
            case SpellStatType.CritDamage: spellCritDamageMultiplier *= (1f + val); break;
            case SpellStatType.AttackSpeed: spellAttackSpeedMultiplier *= (1f + val); break;
        }
    }
}
