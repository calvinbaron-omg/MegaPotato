using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    
    public UnityEvent onDamageTaken;
    public UnityEvent onDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        onDamageTaken.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onDeath.Invoke();
        HandleDeath();
    }

    protected virtual void HandleDeath()
    {
        // Base death behavior - can be overridden
    }
}