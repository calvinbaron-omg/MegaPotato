using UnityEngine;

public class EnemyHealth : Health
{
    public override void TakeDamage(float amount)
    {
        // Apply damage using base health system
        base.TakeDamage(amount);
    }

    protected override void HandleDeath()
    {
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