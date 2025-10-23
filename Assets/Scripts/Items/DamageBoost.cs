using UnityEngine;

public class DamageBoostItem : Item
{
    public float damageMultiplier = 1.5f;
    public float duration = 10f;

    protected override void ApplyEffect(PlayerStats player)
    {
        player.StartCoroutine(player.DamageBoost(damageMultiplier, duration));
    }
}