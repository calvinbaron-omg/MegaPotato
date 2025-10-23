using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    
    public UnityEvent onDamageTaken;  // Fires when object takes damage
    public UnityEvent onDeath;        // Fires when health reaches zero

    protected virtual void Awake()
    {
        // Initialize health to maximum value
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        // Reduce health and clamp between 0 and max
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Trigger damage event for visual/audio effects
        onDamageTaken.Invoke();

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Trigger death event and handle death logic
        onDeath.Invoke();
        HandleDeath();
    }

    protected virtual void HandleDeath()
    {
        // Base death behavior - override in child classes for specific logic
        // (e.g., player game over, enemy destruction, etc.)
    }
}