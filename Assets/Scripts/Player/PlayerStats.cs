using UnityEngine;
using System;
using UnityEngine.Rendering;

public class PlayerStats : MonoBehaviour
{
    // ============================================================
    // 🔔 EVENT
    // ============================================================
    public event Action OnStatsChanged;
    private void NotifyStatsChanged() => OnStatsChanged?.Invoke();

    // ============================================================
    // ⚔️ CORE OFFENSIVE STATS
    // ============================================================
    [Header("Base Offensive Stats")]
    [SerializeField] private float baseDamageMultiplier = 1.0f;      // 100% base damage
    [SerializeField] private float baseAttackSpeedMultiplier = 1.0f; // 100% base attack speed
    [SerializeField] private float baseCritChance = 0.05f;           // 5% base crit chance
    [SerializeField] private float baseCritDamage = 1.5f;            // 150% crit damage
    [SerializeField] private float baseSizeMultiplier = 1.0f;        // Projectile size/AoE multiplier
    [SerializeField] private float baseProjectileCount = 0.0f;       // Extra projectiles
    [SerializeField] private float baseProjectileBounce = 0.0f;      //Extra bounces

    [Header("Base Combat Stats")]
    [SerializeField] private float baseLifeSteal = 0f;               // %
    [SerializeField] private float baseArmor = 0f;                   // %
    [SerializeField] private float baseEvasionChance = 0f;           // %

    // ============================================================
    // 🏃 MOVEMENT & MOBILITY
    // ============================================================
    [Header("Base Movement Stats")]
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseJumpHeight = 8f;
    [SerializeField] private int baseMaxJumps = 1;

    // ============================================================
    // 💰 PICKUPS & ECONOMY
    // ============================================================
    [Header("Base Collection & Economy")]
    [SerializeField] private float basePickupRadiusMultiplier = 1f;  // % radius for pickups
    [SerializeField] private float baseSilverGain = 1f;              // 100% base silver gain
    [SerializeField] private float baseGoldGain = 1f;                // 100% base gold gain

    // ============================================================
    // ⚙️ SYSTEMIC STATS
    // ============================================================
    [Header("Base Systemic Stats")]
    [SerializeField] private float baseDifficultyMultiplier = 1.0f;  // Adjusts global scaling
    [SerializeField] private float baseLuckMultiplier = 1.0f;        // Affects drop quality, rarity, etc.
    [SerializeField] private float baseEliteSpawnChance = 0.05f;     // 5% chance base
    [SerializeField] private float baseExperienceMultiplier= 1.0f;   // 100% base XP gain
    // ============================================================
    // 🔄 RUNTIME BONUSES
    // ============================================================
    private float bonusDamage = 0f;
    private float bonusAttackSpeed = 0f;
    private float bonusCritChance = 0f;
    private float bonusCritDamage = 0f;
    private float bonusSize = 0f;

    private float bonusProjectileCount = 0f;
    private float bonusProjectileBounce = 0f;

    private float bonusLifeSteal = 0f;
    private float bonusArmor = 0f;
    private float bonusEvasion = 0f;

    private float bonusMoveSpeed = 0f;
    private float bonusJumpHeight = 0f;
    private int bonusMaxJumps = 0;

    private float bonusPickupRadius = 0f;
    private float bonusSilverGain = 0f;
    private float bonusGoldGain = 0f;

    private float bonusDifficulty = 0f;
    private float bonusLuck = 0f;
    private float bonusEliteSpawnChance = 0f;
    private float bonusExperience = 0f;

    // ============================================================
    // 🧮 ADD METHODS (Trigger OnStatsChanged)
    // ============================================================
    public void AddDamageBonus(float percent) { bonusDamage += percent; NotifyStatsChanged(); }
    public void AddAttackSpeedBonus(float percent) { bonusAttackSpeed += percent; NotifyStatsChanged(); }
    public void AddCritChanceBonus(float percent) { bonusCritChance += percent; NotifyStatsChanged(); }
    public void AddCritDamageBonus(float percent) { bonusCritDamage += percent; NotifyStatsChanged(); }
    public void AddSizeBonus(float percent) { bonusSize += percent; NotifyStatsChanged(); }

    public void AddProjectileCount(float amount) { bonusProjectileCount += amount; }
    public void AddProjectileBounce(float amount) { bonusProjectileBounce += amount;}

    public void AddLifeSteal(float percent) { bonusLifeSteal += percent; NotifyStatsChanged(); }
    public void AddArmor(float percent) { bonusArmor += percent; NotifyStatsChanged(); }
    public void AddEvasionChance(float percent) { bonusEvasion += percent; NotifyStatsChanged(); }

    public void AddMoveSpeed(float percent) { bonusMoveSpeed += percent; NotifyStatsChanged(); }
    public void AddJumpHeight(float percent) { bonusJumpHeight += percent; NotifyStatsChanged(); }
    public void AddExtraJump(int extraJumps = 1) { bonusMaxJumps += extraJumps; NotifyStatsChanged(); }

    public void AddPickupRadius(float percent) { bonusPickupRadius += percent; NotifyStatsChanged(); }
    public void AddSilverGain(float percent) { bonusSilverGain += percent; NotifyStatsChanged(); }
    public void AddGoldGain(float percent) { bonusGoldGain += percent; NotifyStatsChanged(); }

    public void AddDifficulty(float percent) { bonusDifficulty += percent; NotifyStatsChanged(); }
    public void AddLuck(float percent) { bonusLuck += percent; NotifyStatsChanged(); }
    public void AddEliteSpawnChance(float percent) { bonusEliteSpawnChance += percent; NotifyStatsChanged(); }
    public void AddExperience(float percent) { bonusExperience += percent; NotifyStatsChanged(); }


    // ============================================================
    // 📊 GETTERS (Final Values)
    // ============================================================
    // Offensive
    public float GetDamagePercent() => baseDamageMultiplier * (1f + bonusDamage);
    public float GetAttackSpeed() => baseAttackSpeedMultiplier * (1f + bonusAttackSpeed);
    public float GetCritChance() => baseCritChance + bonusCritChance;
    public float GetCritDamage() => baseCritDamage * (1f + bonusCritDamage);
    public float GetSizeMultiplier() => baseSizeMultiplier * (1f + bonusSize);

    public float GetProjectileCount() => baseProjectileCount + bonusProjectileCount;
    public float GetProjectileBounce() => baseProjectileBounce + bonusProjectileBounce;

    // Combat
    public float GetLifeSteal() => baseLifeSteal + bonusLifeSteal;
    public float GetArmor() => baseArmor + bonusArmor;
    public float GetEvasionChance() => baseEvasionChance + bonusEvasion;

    // Movement
    public float GetMoveSpeed() => baseMoveSpeed * (1f + bonusMoveSpeed);
    public float GetJumpHeight() => baseJumpHeight * (1f + bonusJumpHeight);
    public int GetMaxJumps() => baseMaxJumps + bonusMaxJumps;

    // Collection & Economy
    public float GetPickupRadiusMultiplier() => basePickupRadiusMultiplier * (1f + bonusPickupRadius);
    public float GetSilverGain() => baseSilverGain * (1f + bonusSilverGain);
    public float GetGoldGain() => baseGoldGain * (1f + bonusGoldGain);

    // Systemic
    public float GetDifficultyMultiplier() => baseDifficultyMultiplier * (1f + bonusDifficulty);
    public float GetLuckMultiplier() => baseLuckMultiplier * (1f + bonusLuck);
    public float GetEliteSpawnChance() => baseEliteSpawnChance + bonusEliteSpawnChance;
    public float GetExperienceMultiplier() => baseExperienceMultiplier + bonusExperience;


    // ============================================================
    // ♻️ RESET (New Run)
    // ============================================================
    public void ResetToBaseValues()
    {
        bonusDamage = 0f;
        bonusAttackSpeed = 0f;
        bonusCritChance = 0f;
        bonusCritDamage = 0f;
        bonusSize = 0f;
        bonusProjectileBounce = 0f;
        bonusProjectileCount = 0f;
        bonusLifeSteal = 0f;
        bonusArmor = 0f;
        bonusEvasion = 0f;

        bonusMoveSpeed = 0f;
        bonusJumpHeight = 0f;
        bonusMaxJumps = 0;

        bonusPickupRadius = 0f;
        bonusSilverGain = 0f;
        bonusGoldGain = 0f;

        bonusDifficulty = 0f;
        bonusLuck = 0f;
        bonusEliteSpawnChance = 0f;
        bonusExperience = 0f;

        NotifyStatsChanged();
    }
}
