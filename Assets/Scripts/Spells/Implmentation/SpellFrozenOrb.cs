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
        //PLayer y bottom = 0.1 jump y goes to 3 or 4
        //Enemy y bottom = 1 
        SpellRuntimeStats effective = CalculateEffectiveStats(stats);

        float heightOffset = 1.1f;
        Vector3 start = caster.position + Vector3.up * heightOffset;
        //Vector3 end = targetPosition + Vector3.up * heightOffset;
        //This wont work if we add enemies on vertical structures
        //Will need to figure out why some enemies have a y of 1, or 1.5 when the player has a y of 0.1 on the same plane
        Vector3 end = new Vector3(targetPosition.x, 0.1f + heightOffset, targetPosition.z);
        Vector3 dir = (end - start).normalized;

        GameObject fireball = CreateProjectile(caster, dir, heightOffset);
        FireballBehavior behavior = fireball.AddComponent<FireballBehavior>();

        behavior.Initialize(
            dir,
                effective.projectileSpeed,
                effective.lifetime,
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
            case SpellStatType.Damage: upgradeDamageMult *= (1f + val); break;
            case SpellStatType.Size: upgradeSizeMult *= (1f + val); break;
            case SpellStatType.CritChance: upgradeCritChanceBonus += val; break;
            case SpellStatType.CritDamage: upgradeCritDamageMult *= (1f + val); break;
            case SpellStatType.AttackSpeed: upgradeAttackSpeedMult *= (1f + val); break;
        }
    }
}
