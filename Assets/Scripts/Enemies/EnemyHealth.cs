using System;
using UnityEngine;

public class EnemyHealth : Health
{
    private PickupDrop pickupDrop; 
    
    void Start()
    {
        pickupDrop = GetComponent<PickupDrop>(); 
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
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