using UnityEngine;
using System.Collections.Generic;

public abstract class BaseProjectileSpell : MonoBehaviour, ISpell
{
    [Header("Spell Info")]
    [SerializeField] protected string spellName = "Unnamed Spell";
    [SerializeField] protected string description = "No description.";
    [SerializeField] protected Sprite icon;

    [Header("Base Stats")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float baseCritChance = 0f;
    [SerializeField] protected float baseCritDamage = 1.5f;   // 150%
    [SerializeField] protected float baseSize = 4f;
    [SerializeField] protected float baseAttackSpeed = 1f;    // affects cooldown & projectile speed
    [SerializeField] protected float baseAOE = 1f;            // internal AoE multiplier (not upgradeable)
    [SerializeField] protected float lifetime = 3f;
    [SerializeField] protected float baseProjectileSpeed = 8f; 
    [SerializeField] protected float baseRange = 8f; 

    [Header("Progression")]
    [SerializeField] protected int spellLevel = 1;
    [SerializeField] protected int maxLevel = 50;

    [Header("Prefab Reference")]
    [SerializeField] protected GameObject spellProjectilePrefab;

    // =======================================================
    // LOCAL UPGRADE MULTIPLIERS (reset each run)
    // =======================================================
    protected float spellDamageMultiplier = 1f;
    protected float spellAttackSpeedMultiplier = 1f;
    protected float spellCritChanceBonus = 0f;
    protected float spellCritDamageMultiplier = 1f;
    protected float spellSizeMultiplier = 1f;

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
    public float GetActualRange(float globalRangeBonus) => baseRange * spellSizeMultiplier + globalRangeBonus;

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
        baseDamage = snapshot.baseDamage;
        baseCritChance = snapshot.baseCritChance;
        baseCritDamage = snapshot.baseCritDamage;
        baseSize = snapshot.baseSize;
        baseAttackSpeed = snapshot.baseAttackSpeed;
        baseAOE = snapshot.baseAOE;
        spellLevel = snapshot.spellLevel;
        baseRange = snapshot.baseRange;
        spellDamageMultiplier = 1f;
        spellAttackSpeedMultiplier = 1f;
        spellCritChanceBonus = 0f;
        spellCritDamageMultiplier = 1f;
        spellSizeMultiplier = 1f;
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

    public void ApplyUpgradeAndLevel(SpellStatType statType, Rarity rarity)
    {
        float baseVal = GetBaseUpgradeValue(statType);
        (bool isFlat, int flatAmt) = GetFlatUpgradeInfo(statType);

        if (isFlat)
        {
            int scaledFlat = Mathf.RoundToInt(flatAmt * RarityHelper.GetMultiplier(rarity));
            ApplyStatUpgrade(statType, 0f, scaledFlat);
        }
        else
        {
            float scaledVal = baseVal * RarityHelper.GetMultiplier(rarity);
            ApplyStatUpgrade(statType, scaledVal, 0);
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
    protected virtual (float damage, float critChance, float critDamage, float attackSpeed, float size, float aoe, float projectileSpeed)
        CalculateEffectiveStats(PlayerStats stats)
    {
        float finalDamage = baseDamage * spellDamageMultiplier * stats.GetDamage();
        float totalMulitiplier = spellDamageMultiplier + (stats.GetDamage() / 100);
        float testDamage = baseDamage * totalMulitiplier;
        float finalCritChance = baseCritChance + spellCritChanceBonus + stats.GetCritChance();
        float finalCritDamage = baseCritDamage * spellCritDamageMultiplier * stats.GetCritDamage();
        float finalAttackSpeed = baseAttackSpeed * spellAttackSpeedMultiplier * stats.GetAttackSpeed();
        float finalSize = baseSize * spellSizeMultiplier;
        float finalAOE = baseAOE * finalSize;

        // ✅ new: projectile speed scales with player attack speed
        float finalProjectileSpeed = baseProjectileSpeed * stats.GetAttackSpeed();

        return (finalDamage, finalCritChance, finalCritDamage, finalAttackSpeed, finalSize, finalAOE, finalProjectileSpeed);
    }


    // =======================================================
    // PROJECTILE SPAWNING
    // =======================================================
    public abstract void CastSpell(Transform caster, Vector3 targetPosition);

    protected virtual GameObject CreateProjectile(Transform caster, Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - caster.position).normalized;
        Vector3 spawnPos = caster.position + dir * 1f;

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
