using UnityEngine;
using System.Collections.Generic;

public abstract class BaseProjectileSpell : MonoBehaviour, ISpell
{
    [Header("Spell Information")]
    [SerializeField] protected string spellName = "Unknown Spell";
    [SerializeField] protected string description = "Spell description";
    [SerializeField] protected Sprite icon;
    
    [Header("Base Stats")]
    [SerializeField] protected float baseDamage = 25f;
    [SerializeField] protected float baseCooldown = 1.2f;
    [SerializeField] protected float baseRange = 8f;
    [SerializeField] protected float projectileSpeed = 8f;
    [SerializeField] protected float lifetime = 3f;

    [Header("Progression")]
    [SerializeField] protected int spellLevel = 1;
    [SerializeField] protected int maxLevel = 50;

    [Header("Prefab Reference")]
    [SerializeField] protected GameObject spellPrefab;

    // ISpell Implementation
    public string SpellName => spellName;
    public string Description => description;
    public Sprite Icon => icon;
    public float BaseDamage => baseDamage;
    public float BaseCooldown => baseCooldown;
    public float BaseRange => baseRange;
    public int SpellLevel => spellLevel;
    public int MaxLevel => maxLevel;

    public float GetActualCooldown(float globalAttackSpeed) => baseCooldown / globalAttackSpeed;
    public float GetActualRange(float globalRangeBonus) => baseRange + globalRangeBonus;

    public abstract void CastSpell(Transform caster, Vector3 targetPosition);

    public bool CanLevelUp => spellLevel < maxLevel;

    public virtual void LevelUp()
    {
        if (!CanLevelUp) return;
        spellLevel++;

        // Optional generic scaling
        // You can tune/rem/remove this if you decide
        // all progression should come only from upgrades.
    }

    protected virtual GameObject CreateProjectile(Transform caster, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - caster.position).normalized;
        Vector3 spawnPosition = caster.position + direction * 1f;
        
        GameObject projectile = Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
        EnsureProjectileComponents(projectile);
        return projectile;
    }

    private void EnsureProjectileComponents(GameObject projectile)
    {
        if (projectile.GetComponent<Collider>() == null)
        {
            SphereCollider col = projectile.AddComponent<SphereCollider>();
            col.isTrigger = true;
        }
            
        if (projectile.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
    }

    // ===== UPGRADE SYSTEM HOOKS =====

    // Each spell tells us which stats it CAN upgrade.
    // Example for Frozen Orb: Damage, Size, CritChance, CritDamage, ProjectileSpeed
    public abstract List<SpellStatType> GetUpgradeableStats();

    // Each spell provides a base "per-upgrade" value for a given stat.
    // e.g. CritChance base = 0.05 (5%), Damage base = 0.10 (10%)
    public abstract float GetBaseUpgradeValue(SpellStatType statType);

    // For integer-like stats such as ProjectileCount, we might want +1
    // This returns (isFlat, flatAmount). If isFlat==true we won't % scale.
    public virtual (bool isFlat, int flatAmount) GetFlatUpgradeInfo(SpellStatType statType)
    {
        // Default: not flat
        return (false, 0);
    }

    // Actually apply a rolled upgrade to the spell's internal stats.
    // For % stats, effectiveValue is e.g. 0.15 for +15%
    // For flat stats, effectiveValue will come from flatAmount passed separately.
    public abstract void ApplyStatUpgrade(SpellStatType statType, float effectiveValue, int flatAmountIfAny = 0);

    // Convenience: add level when an upgrade is taken
    public void ApplyUpgradeAndLevel(SpellStatType statType, Rarity rarity)
    {
        // 1. figure out base upgrade
        float baseVal = GetBaseUpgradeValue(statType);
        (bool isFlat, int flatAmount) = GetFlatUpgradeInfo(statType);

        if (isFlat)
        {
            // flat stats: we can choose to also scale with rarity if desired.
            // Example rule: Legendary could give +2 projectiles instead of +1.
            int scaledFlat = Mathf.RoundToInt(flatAmount * RarityHelper.GetMultiplier(rarity));
            ApplyStatUpgrade(statType, 0f, scaledFlat);
        }
        else
        {
            // percentage-like stats, scale by rarity multiplier
            float scaledVal = baseVal * RarityHelper.GetMultiplier(rarity);
            ApplyStatUpgrade(statType, scaledVal, 0);
        }

        // 2. Level up the spell after applying upgrade
        LevelUp();
    }

    // Generate N random upgrade "choices" for UI (WITHOUT applying them yet).
    // This will:
    // - roll rarity once up front
    // - pick unique stats from the spell's upgradeable list
    // - build descriptions
    public List<RolledUpgradeChoice> RollUpgradeChoices(int choiceCount)
    {
        List<RolledUpgradeChoice> results = new List<RolledUpgradeChoice>();

        if (!CanLevelUp)
            return results;

        // 1. roll rarity for THIS upgrade roll
        Rarity rolledRarity = RarityHelper.RollRarity();

        // 2. choose which stats we're offering
        List<SpellStatType> allStats = new List<SpellStatType>(GetUpgradeableStats());
        // Shuffle-pick 'choiceCount'
        for (int i = 0; i < choiceCount && allStats.Count > 0; i++)
        {
            int index = Random.Range(0, allStats.Count);
            SpellStatType chosenStat = allStats[index];
            allStats.RemoveAt(index);

            // 3. build the visual + numeric info for that stat
            (bool isFlat, int flatAmt) = GetFlatUpgradeInfo(chosenStat);
            if (isFlat)
            {
                int scaledFlat = Mathf.RoundToInt(flatAmt * RarityHelper.GetMultiplier(rolledRarity));
                string text = SpellUpgradeFormatter.FormatFlatStat(chosenStat, scaledFlat);

                results.Add(new RolledUpgradeChoice(
                    chosenStat,
                    rolledRarity,
                    0f,
                    text
                ));
            }
            else
            {
                float baseVal = GetBaseUpgradeValue(chosenStat);
                float scaledVal = baseVal * RarityHelper.GetMultiplier(rolledRarity);

                string text = SpellUpgradeFormatter.FormatPercentStat(chosenStat, scaledVal);

                results.Add(new RolledUpgradeChoice(
                    chosenStat,
                    rolledRarity,
                    scaledVal,
                    text
                ));
            }
        }

        return results;
    }
}
