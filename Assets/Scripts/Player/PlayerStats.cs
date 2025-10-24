using UnityEngine;
using System;

[Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Combat Stats")]
    [SerializeField] private float baseAttackSpeed = 1.0f;
    [SerializeField] private float baseDamage = 25f;
    [SerializeField] private float baseLifeSteal = 0f;
    [SerializeField] private float baseArmor = 0f;
    [SerializeField] private float baseDodgeChance = 0f;

    [Header("Movement Stats")]
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseJumpHeight = 8f;
    [SerializeField] private int baseMaxJumps = 1;

    [Header("Collection Stats")]
    [SerializeField] private float baseCollectionRadius = 1f; // Multiplier base
    [SerializeField] private float baseXPMultiplier = 1f;
    [SerializeField] private float baseGoldMultiplier = 1f;
    [SerializeField] private float baseSilverMultiplier = 1f;

    [Header("Current Stats (Read Only)")]
    public float CurrentAttackSpeed { get; private set; }
    public float CurrentDamage { get; private set; }
    public float CurrentLifeSteal { get; private set; }
    public float CurrentArmor { get; private set; }
    public float CurrentDodgeChance { get; private set; }
    public float CurrentMoveSpeed { get; private set; }
    public float CurrentJumpHeight { get; private set; }
    public int CurrentMaxJumps { get; private set; }
    public float CurrentCollectionRadiusMultiplier { get; private set; }
    public float CurrentXPMultiplier { get; private set; }
    public float CurrentGoldMultiplier { get; private set; }
    public float CurrentSilverMultiplier { get; private set; }

    // Events for UI updates when stats change
    public event Action OnStatsChanged;

    // Modifier tracking
    private float attackSpeedModifier = 1.0f;
    private float damageModifier = 1.0f;
    private float moveSpeedModifier = 1.0f;
    private float jumpHeightModifier = 1.0f;
    private float collectionRadiusModifier = 1.0f;
    private float xpMultiplierModifier = 1.0f;
    private float goldMultiplierModifier = 1.0f;
    private float silverMultiplierModifier = 1.0f;

     void Start()
    {
        CalculateFinalStats();
    }

    void CalculateFinalStats()
    {
        // Calculate final stats with all modifiers
        CurrentAttackSpeed = baseAttackSpeed * attackSpeedModifier;
        CurrentDamage = baseDamage * damageModifier;
        CurrentLifeSteal = baseLifeSteal;
        CurrentArmor = baseArmor;
        CurrentDodgeChance = baseDodgeChance;
        CurrentMoveSpeed = baseMoveSpeed * moveSpeedModifier;
        CurrentJumpHeight = baseJumpHeight * jumpHeightModifier;
        CurrentMaxJumps = baseMaxJumps;
        
        // New collection multipliers
        CurrentCollectionRadiusMultiplier = baseCollectionRadius * collectionRadiusModifier;
        CurrentXPMultiplier = baseXPMultiplier * xpMultiplierModifier;
        CurrentGoldMultiplier = baseGoldMultiplier * goldMultiplierModifier;
        CurrentSilverMultiplier = baseSilverMultiplier * silverMultiplierModifier;

        // Notify listeners that stats have changed
        OnStatsChanged?.Invoke();
    }

    // ===== PUBLIC METHODS FOR POWER-UPS =====
    public void AddCollectionRadius(float multiplier)
    {
        collectionRadiusModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddXPMultiplier(float multiplier)
    {
        xpMultiplierModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddGoldMultiplier(float multiplier)
    {
        goldMultiplierModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddSilverMultiplier(float multiplier)
    {
        silverMultiplierModifier += multiplier;
        CalculateFinalStats();
    }
    public void AddAttackSpeed(float multiplier)
    {
        attackSpeedModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddDamage(float multiplier)
    {
        damageModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddLifeSteal(float amount)
    {
        baseLifeSteal += amount;
        CalculateFinalStats();
    }

    public void AddArmor(float amount)
    {
        baseArmor += amount;
        CalculateFinalStats();
    }

    public void AddDodgeChance(float amount)
    {
        baseDodgeChance = Mathf.Clamp(baseDodgeChance + amount, 0f, 0.8f); // Cap at 80%
        CalculateFinalStats();
    }

    public void AddMoveSpeed(float multiplier)
    {
        moveSpeedModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddJumpHeight(float multiplier)
    {
        jumpHeightModifier += multiplier;
        CalculateFinalStats();
    }

    public void AddExtraJump()
    {
        baseMaxJumps++;
        CalculateFinalStats();
    }

    // ===== GETTER METHODS FOR OTHER SCRIPTS =====

    public float GetCollectionRadiusMultiplier() => CurrentCollectionRadiusMultiplier;
    public float GetXPMultiplier() => CurrentXPMultiplier;
    public float GetGoldMultiplier() => CurrentGoldMultiplier;
    public float GetSilverMultiplier() => CurrentSilverMultiplier;
    public float GetAttackSpeed() => CurrentAttackSpeed;
    public float GetDamage() => CurrentDamage;
    public float GetLifeSteal() => CurrentLifeSteal;
    public float GetArmor() => CurrentArmor;
    public float GetDodgeChance() => CurrentDodgeChance;
    public float GetMoveSpeed() => CurrentMoveSpeed;
    public float GetJumpHeight() => CurrentJumpHeight;
    public int GetMaxJumps() => CurrentMaxJumps;

    // ===== UTILITY METHODS =====

    public bool TryDodge()
    {
        return UnityEngine.Random.value <= CurrentDodgeChance;
    }

    public float CalculateDamageReduction(float incomingDamage)
    {
        // Simple armor formula: reduces damage by armor percentage
        float reduction = incomingDamage * (CurrentArmor / 100f);
        return Mathf.Max(incomingDamage - reduction, 1f); // Minimum 1 damage
    }

    public float CalculateLifeSteal(float damageDealt)
    {
        return damageDealt * (CurrentLifeSteal / 100f);
    }
}