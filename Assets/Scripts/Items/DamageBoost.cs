using UnityEngine;

public class DamageBoostItem : Item
{
    public float damageMultiplier = 1.5f;
    public float duration = 10f;

    protected override void ApplyEffect(PlayerStats player)
    {
        var tempEffects = player.GetComponent<PlayerTemporaryEffects>();
        if (tempEffects != null)
        {
            tempEffects.ApplyDamageBoost(damageMultiplier, duration);
        }
    }
    
}