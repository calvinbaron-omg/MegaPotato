using UnityEngine;

public class EnemyHealth : Health
{
    public override void TakeDamage(float amount)
    {
        Debug.Log($"Enemy taking {amount} damage. Current health: {currentHealth}");
        base.TakeDamage(amount);
        Debug.Log($"Enemy health after damage: {currentHealth}");
    }

    protected override void HandleDeath()
    {
        Debug.Log($"Enemy HandleDeath called! Health: {currentHealth}");
        
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddKill();
        }
        
        Debug.Log("Destroying enemy...");
        Destroy(gameObject);
    }
}