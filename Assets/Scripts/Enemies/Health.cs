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

    public virtual void TakeDamage(float amount, bool isCrit = false, string source = "Unknown")
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onDamageTaken.Invoke();

        // floating text (unchanged)
        Vector3 spawnPos = transform.position;
        Collider col = GetComponent<Collider>();
        if (col != null)
            spawnPos = col.bounds.center + Vector3.up * col.bounds.extents.y * -1f;
        else
            spawnPos += Vector3.up * 1.5f;

        FloatingDamageTextManager.Instance.Spawn(spawnPos, amount, isCrit);

        if (currentHealth <= 0)
            Die();
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