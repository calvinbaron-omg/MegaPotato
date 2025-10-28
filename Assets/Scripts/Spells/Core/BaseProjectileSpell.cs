using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Mono.Cecil.Cil;

public abstract class BaseProjectileSpell : MonoBehaviour, ISpell
{
    [Header("Spell Info")]
    [SerializeField] protected string spellName = "Unnamed Spell";
    [SerializeField] protected string description = "No description.";
    [SerializeField] protected Sprite icon;

   [Header("Base Spell Stats")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float baseAttackSpeed = 1f;
    [SerializeField] protected float baseCritChance = 0f;
    [SerializeField] protected float baseCritDamage = 1.5f;
    [SerializeField] protected float baseSize = 1f;
    [SerializeField] protected float baseAOE = 1f;
    [SerializeField] protected float baseLifetime = 3f;
    [SerializeField] protected float baseProjectileSpeed = 8f;
    [SerializeField] protected float baseRange = 8f;

    [Header("Spell Upgrades")]
    [SerializeField] protected float upgradeDamageMult = 1f;
    [SerializeField] protected float upgradeAttackSpeedMult = 1f;
    [SerializeField] protected float upgradeCritChanceBonus = 0f;
    [SerializeField] protected float upgradeCritDamageMult = 1f;
    [SerializeField] protected float upgradeSizeMult = 1f;

    [Header("Progression")]
    [SerializeField] protected int spellLevel = 1;
    [SerializeField] protected int maxLevel = 50;

    [Header("Prefab Reference")]
    [SerializeField] protected GameObject spellProjectilePrefab;

    // =======================================================
    // LOCAL UPGRADE MULTIPLIERS (reset each run)
    // =======================================================
    // protected float spellDamageMultiplier = 1f;
    // protected float spellAttackSpeedMultiplier = 1f;
    // protected float spellCritChanceBonus = 0f;
    // protected float spellCritDamageMultiplier = 1f;
    // protected float spellSizeMultiplier = 1f;

    // =======================================================
    // DEFAULT SNAPSHOT (for per-run reset)
    // =======================================================
    private struct SpellDefaults
    {
        public float baseDamage;
        public float baseCritChance;
        public float baseCritDamage;
        public float baseSize;
        public float baseAttackSpeed;
        public float baseAOE;
        public int spellLevel;
        public float baseRange;
    }

    private SpellDefaults snapshot;
    private bool hasSnapshot = false;

    // =======================================================
    // ISpell Implementation
    // =======================================================
    public string SpellName => spellName;
    public string Description => description;
    public Sprite Icon => icon;
    public float BaseDamage => baseDamage;
    public float BaseCooldown => 1f / baseAttackSpeed;
    public float BaseRange => baseSize;
    public int SpellLevel => spellLevel;
    public int MaxLevel => maxLevel;

    public float GetActualCooldown(float globalAttackSpeed) => 1f / (baseAttackSpeed * globalAttackSpeed);
    public float GetActualRange(float globalRangeBonus) => baseRange * upgradeSizeMult + globalRangeBonus;

    // =======================================================
    // INITIALIZATION
    // =======================================================
    protected virtual void Awake()
    {
        if (!hasSnapshot)
        {
            snapshot = new SpellDefaults
            {
                baseDamage = baseDamage,
                baseCritChance = baseCritChance,
                baseCritDamage = baseCritDamage,
                baseSize = baseSize,
                baseAttackSpeed = baseAttackSpeed,
                baseAOE = baseAOE,
                spellLevel = 1,
                baseRange = baseRange,
            };
            hasSnapshot = true;
        }
    }

    // =======================================================
    // RESET PER RUN
    // =======================================================
    public virtual void ResetToBaseStats()
    {
        
        // 🔹 Reset only runtime-modified fields
        spellLevel = 1;

        upgradeDamageMult = 1f;
        upgradeAttackSpeedMult = 1f;
        upgradeCritChanceBonus = 0f;
        upgradeCritDamageMult = 1f;
        upgradeSizeMult = 1f;
    }


    // =======================================================
    // LEVELING / UPGRADES
    // =======================================================
    public bool CanLevelUp => spellLevel < maxLevel;

    public virtual void LevelUp()
    {
        if (!CanLevelUp) return;
        spellLevel++;
    }

    public abstract List<SpellStatType> GetUpgradeableStats();
    public abstract float GetBaseUpgradeValue(SpellStatType statType);
    public virtual (bool isFlat, int flatAmount) GetFlatUpgradeInfo(SpellStatType statType) => (false, 0);
    public abstract void ApplyStatUpgrade(SpellStatType statType, float effectiveValue, int flatAmountIfAny = 0);

    public void ApplyUpgradeAndLevel(List<RolledUpgradeChoice> selectedUpgrades)
    {
        
        foreach (var upgrade in selectedUpgrades)
        {
            float baseVal = GetBaseUpgradeValue(upgrade.statType);
            (bool isFlat, int flatAmt) = GetFlatUpgradeInfo(upgrade.statType);
            if (isFlat)
            {
                int scaledFlat = Mathf.RoundToInt(flatAmt * RarityHelper.GetMultiplier(upgrade.rarity));
                ApplyStatUpgrade(upgrade.statType, 0f, scaledFlat);
            }
            else
            {
                float scaledVal = baseVal * RarityHelper.GetMultiplier(upgrade.rarity);
                ApplyStatUpgrade(upgrade.statType, scaledVal, 0);
            }
        
            Debug.Log($"Applied {upgrade.uiText} ({upgrade.rarity}) to {SpellName}");
        }  
        LevelUp();
    }

    // =======================================================
    // ROLL UPGRADE CHOICES  ✅ (for SpellPoolManager)
    // =======================================================
    public List<RolledUpgradeChoice> RollUpgradeChoices(int choiceCount)
    {
        List<RolledUpgradeChoice> results = new List<RolledUpgradeChoice>();
        if (!CanLevelUp) return results;

        Rarity rolledRarity = RarityHelper.RollRarity();
        List<SpellStatType> availableStats = new List<SpellStatType>(GetUpgradeableStats());
        ShuffleList(availableStats);

        for (int i = 0; i < choiceCount && availableStats.Count > 0; i++)
        {
            SpellStatType chosen = availableStats[0];
            availableStats.RemoveAt(0);

            (bool isFlat, int flatAmt) = GetFlatUpgradeInfo(chosen);
            if (isFlat)
            {
                int scaledFlat = Mathf.RoundToInt(flatAmt * RarityHelper.GetMultiplier(rolledRarity));
                string text = SpellUpgradeFormatter.FormatFlatStat(chosen, scaledFlat);
                results.Add(new RolledUpgradeChoice(chosen, rolledRarity, 0f, text));
            }
            else
            {
                float baseVal = GetBaseUpgradeValue(chosen);
                float scaledVal = baseVal * RarityHelper.GetMultiplier(rolledRarity);
                string text = SpellUpgradeFormatter.FormatPercentStat(chosen, scaledVal);
                results.Add(new RolledUpgradeChoice(chosen, rolledRarity, scaledVal, text));
            }
        }

        return results;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = temp;
        }
    }

    // =======================================================
    // COMBINED STAT CALCULATION
    // =======================================================
    protected virtual SpellRuntimeStats CalculateEffectiveStats(PlayerStats player)
    {
        // 1. Combine base stats with spell's own upgrade multipliers
        float upgradedDamage        = baseDamage * upgradeDamageMult;
        float upgradedAttackSpeed   = baseAttackSpeed * upgradeAttackSpeedMult;
        float upgradedCritChance    = baseCritChance + upgradeCritChanceBonus;
        float upgradedCritDamage    = baseCritDamage * upgradeCritDamageMult;
        float upgradedSize          = baseSize * upgradeSizeMult;

        // 2. Apply player global modifiers (from PlayerStats)
        float finalDamage = upgradedDamage * player.GetDamagePercent();
        float finalAttackSpeed = upgradedAttackSpeed * player.GetAttackSpeed();
        float finalCritChance = upgradedCritChance + player.GetCritChance();
        float finalCritDamage = upgradedCritDamage * player.GetCritDamage();
        float finalSize = upgradedSize * player.GetSizeMultiplier();

        // 3. Derived stats
        float finalAOE = baseAOE * finalSize;
        float finalProjectileSpeed = baseProjectileSpeed * finalAttackSpeed;
        float finalRange = baseRange * finalSize;
        float finalLifetime = baseLifetime; // lifetime never scales

        return new SpellRuntimeStats(
            damage: finalDamage,
            attackSpeed: finalAttackSpeed,
            critChance: finalCritChance,
            critDamage: finalCritDamage,
            size: finalSize,
            aoe: finalAOE,
            projectileSpeed: finalProjectileSpeed,
            range: finalRange,
            lifetime: finalLifetime
        );
    }




    // =======================================================
    // PROJECTILE SPAWNING
    // =======================================================
    public abstract void CastSpell(Transform caster, Vector3 targetPosition);

    // direction should already be normalized
    protected virtual GameObject CreateProjectile(Transform caster, Vector3 direction, float heightOffset = 1.1f)
    {
        // spawn where the "muzzle" would be: player height + forward offset
        Vector3 spawnPos =
            caster.position
            + Vector3.up * heightOffset
            + direction * 1f; // 1 unit in front
        Debug.Log("Projectile Spawn Pos: " + spawnPos.y);
        GameObject proj = Instantiate(spellProjectilePrefab, spawnPos, Quaternion.identity);
        EnsureProjectileComponents(proj);
        return proj;
    }



    private void EnsureProjectileComponents(GameObject proj)
    {
        if (proj.GetComponent<Collider>() == null)
        {
            SphereCollider col = proj.AddComponent<SphereCollider>();
            col.isTrigger = true;
        }

        if (proj.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = proj.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
    }
}
