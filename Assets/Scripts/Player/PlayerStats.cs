using UnityEngine;
using System;
using System.Collections;

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

    [Header("Current Stats (Read Only)")]
    public float CurrentAttackSpeed { get; private set; }
    public float CurrentDamage { get; private set; }
    public float CurrentLifeSteal { get; private set; }
    public float CurrentArmor { get; private set; }
    public float CurrentDodgeChance { get; private set; }
    public float CurrentMoveSpeed { get; private set; }
    public float CurrentJumpHeight { get; private set; }
    public int CurrentMaxJumps { get; private set; }

    // Events for UI updates when stats change
    public event Action OnStatsChanged;

    // Modifier tracking
    private float attackSpeedModifier = 1.0f;
    private float damageModifier = 1.0f;
    private float moveSpeedModifier = 1.0f;
    private float jumpHeightModifier = 1.0f;

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

        // Notify listeners that stats have changed
        OnStatsChanged?.Invoke();
    }

    // ===== PUBLIC METHODS FOR POWER-UPS =====

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

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        moveSpeedModifier *= multiplier;
        CalculateFinalStats();
        yield return new WaitForSeconds(duration);
        moveSpeedModifier /= multiplier;
        CalculateFinalStats();
    }

    public IEnumerator DamageBoost(float multiplier, float duration)
    {
        // baseDamage *= multiplier;
        // yield return new WaitForSeconds(duration);
        // baseDamage /= multiplier;
        return null;
    }

    public IEnumerator ActivateShield(float duration)
    {
        // isShielded = true;
        // yield return new WaitForSeconds(duration);
        // isShielded = false;
        return null;
    }
}