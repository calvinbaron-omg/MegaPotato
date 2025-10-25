using UnityEngine;

public interface ISpell
{
    // Basic spell information
    string SpellName { get; }
    string Description { get; }
    Sprite Icon { get; }
    
    // Core stats
    float BaseDamage { get; }
    float BaseCooldown { get; }
    float BaseRange { get; }
    
    // Spell activation
    void CastSpell(Transform caster, Vector3 targetPosition);
    
    // Calculation modifiers
    float GetActualCooldown(float globalAttackSpeed);
    float GetActualRange(float globalRangeBonus);
}