using UnityEngine;

public class ShieldItem : Item
{
    public float duration = 5f;

     protected override void ApplyEffect(PlayerStats player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ActivateInvincibility(duration);
            Debug.Log($"[ShieldItem] Player is invincible for {duration} seconds.");
        }
        else
        {
            
        }
    }
}
