using UnityEngine;
using System.Collections.Generic;

public class SpellAura : BaseProjectileSpell
{
    [Header("Aura Settings")]
    [SerializeField] private float tickBaseInterval = 1f; // seconds between ticks before attack speed scaling

    public override void CastSpell(Transform caster, Vector3 targetPosition, Vector3 targetScale)
    {
        PlayerStats stats = caster.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats missing on caster!");
            return;
        }

        SpellRuntimeStats effective = CalculateEffectiveStats(stats);

        // If aura already exists, refresh it
        AuraBehavior existingAura = caster.GetComponentInChildren<AuraBehavior>();
        if (existingAura != null)
        {
            existingAura.Refresh(effective);
            return;
        }

        // Spawn aura around the player
        GameObject auraObj = new GameObject("PlayerAura");
        auraObj.transform.SetParent(caster);
        auraObj.transform.localPosition = Vector3.zero;

        AuraBehavior aura = auraObj.AddComponent<AuraBehavior>();
        aura.Initialize(
            effective.damage,
            effective.size,
            effective.critChance,
            effective.critDamage,
            tickBaseInterval,
            effective.attackSpeed
        );
    }

    public override List<SpellStatType> GetUpgradeableStats() =>
        new List<SpellStatType> {
            SpellStatType.Damage,
            SpellStatType.Size,
            SpellStatType.AttackSpeed,
            SpellStatType.CritChance,
            SpellStatType.CritDamage
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
        }
    }
}
