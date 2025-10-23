using UnityEngine;

public class SpellFireball : SpellProjectile
{
    [Header("Fireball Settings")]
    public float baseCooldown = 0.8f;      // Base time between casts (before speed modifiers)
    public float baseRange = 8f;           // Base targeting range (before range bonuses)
    public float burnChance = 0.25f;       // 25% chance to apply burn effect
    public float burnDamage = 5f;          // Damage per burn tick
    public float burnDuration = 3f;        // How long burn effect lasts
    public float aoeRadius = 4f;           // Area of effect explosion radius
    
    [Header("Spell Info")]
    public string spellName = "Fireball";
    public string description = "Launches a fireball that explodes on impact";
    public Sprite icon; // For UI display
    
    // Calculate final cooldown with global attack speed modifier
    public float GetSpellCooldown(float globalAttackSpeed) 
    { 
        return baseCooldown / globalAttackSpeed; 
    }
    
    // Calculate final range with global range bonus
    public float GetSpellRange(float globalRange) 
    { 
        return baseRange + globalRange; 
    }
    
    protected override void OnHitEnemy(GameObject enemy)
    {
        // Deal AOE damage to all enemies within explosion radius
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, aoeRadius);
        
        foreach (Collider hitEnemy in hitEnemies)
        {
            if (hitEnemy.CompareTag("Enemy"))
            {
                Health enemyHealth = hitEnemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    
                    // Chance to apply burn damage over time
                    if (Random.value <= burnChance)
                    {
                        ApplyBurn(hitEnemy.gameObject);
                    }
                }
            }
        }
        
        Destroy(gameObject);
    }
    
    void ApplyBurn(GameObject enemy)
    {
        // Apply burn status effect to enemy
        EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
        if (enemyStatus != null)
        {
            enemyStatus.ApplyBurn(burnDamage, burnDuration);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize AOE radius in Scene view for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}