using UnityEngine;

public class SpeedBoostItem : Item
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;

    protected override void ApplyEffect(PlayerStats player)
    {
        var tempEffects = player.GetComponent<PlayerTemporaryEffects>();
        if (tempEffects != null)
        {
            tempEffects.ApplySpeedBoost(speedMultiplier, duration);
        }
    }

}