using System;
using UnityEngine;

public class EnemyHealth : Health
{
    private PickupDrop pickupDrop; 
    
    void Start()
    {
        pickupDrop = GetComponent<PickupDrop>();
        PlayerLevel playerLevel = GameObject.FindFirstObjectByType<PlayerLevel>(); ;
    }

    public override void TakeDamage(float amount, bool isCrit = false, string source = "Unknown")
    {
        // Log damage source for player DPS tracking
        DamageTracker.Instance?.RegisterDamage(source, amount);

        base.TakeDamage(amount, isCrit, source);
    }


    protected override void HandleDeath()
    {
        // Trigger pickup drops before destroying
        if (pickupDrop != null) 
        {
            pickupDrop.CalculateDrops();
        }
        //itemDrop.Calculate();

        // Add to kill count when enemy dies
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddKill();
        }
        
        // Remove enemy from game
        Destroy(gameObject);
    }
}