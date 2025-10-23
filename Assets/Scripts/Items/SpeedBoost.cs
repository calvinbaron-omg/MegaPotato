using UnityEngine;

public class SpeedBoostItem : Item
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;

    protected override void ApplyEffect(PlayerStats player)
    {
        player.StartCoroutine(player.SpeedBoost(speedMultiplier, duration));
    }
}