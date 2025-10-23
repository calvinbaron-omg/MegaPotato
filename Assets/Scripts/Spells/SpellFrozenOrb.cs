using UnityEngine;

public class SpellFrozenOrb : SpellProjectile
{
    [Header("Frozen Orb Settings")]
    public float baseCooldown = 1.2f;      // Base time between casts (before speed modifiers)
    public float baseRange = 8f;           // Base targeting range (before range bonuses)
    public float slowChance = 0.3f;        // 30% chance to apply slow effect
    public float slowAmount = 0.5f;        // 50% movement speed reduction when slowed
    public float slowDuration = 2f;        // How long slow effect lasts
    public float aoeRadius = 3f;           // Area of effect explosion radius
    
    [Header("Spell Info")]
    public string spellName = "Frozen Orb";
    public string description = "Launches an orb of ice that slows enemies";
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
                    
                    // Chance to apply movement slow effect
                    if (Random.value <= slowChance)
                    {
                        ApplySlow(hitEnemy.gameObject);
                    }
                }
            }
        }
        
        Destroy(gameObject);
    }
    
    void ApplySlow(GameObject enemy)
    {
        // Apply slow status effect to enemy
        EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
        if (enemyStatus != null)
        {
            enemyStatus.ApplySlow(slowAmount, slowDuration);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize AOE radius in Scene view for debugging
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}