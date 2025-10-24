using UnityEngine;

public class ShieldItem : Item
{
    public float duration = 5f;

    protected override void ApplyEffect(PlayerStats player)
    {
        player.StartCoroutine(player.ActivateShield(duration));
    }
}