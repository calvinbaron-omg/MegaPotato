using UnityEngine;
using System.Collections.Generic;

public class PlayerAutoAttack : MonoBehaviour
{
    [Header("Global Modifiers")]
    [Range(0.5f, 3.0f)]
    public float globalAttackSpeed = 1.0f;  // Multiplier for all spell cooldowns
    public float globalRangeBonus = 0f;     // Bonus range added to all spells
    
    [Header("Available Spells")]
    public List<GameObject> equippedSpells = new List<GameObject>();  // Currently active spells
    
    // Track each spell's cooldown and components independently
    private Dictionary<GameObject, float> spellCooldowns = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, SpellProjectile> spellComponents = new Dictionary<GameObject, SpellProjectile>();
    
    void Start()
    {
        InitializeSpells();
    }
    
    void InitializeSpells()
    {
        // Set up cooldown tracking and cache spell components for performance
        spellCooldowns.Clear();
        spellComponents.Clear();
        
        foreach (GameObject spellPrefab in equippedSpells)
        {
            if (spellPrefab != null)
            {
                spellCooldowns[spellPrefab] = 0f;
                SpellProjectile spellComp = spellPrefab.GetComponent<SpellProjectile>();
                if (spellComp != null)
                {
                    spellComponents[spellPrefab] = spellComp;
                }
            }
        }
    }
    
    void Update()
    {
        // Auto-attack with all equipped spells
        foreach (GameObject spellPrefab in equippedSpells)
        {
            if (spellPrefab == null) continue;
            
            // Get spell component (cached for performance)
            SpellProjectile spell = GetSpellComponent(spellPrefab);
            if (spell == null) continue;
            
            // Check if spell is off cooldown
            float cooldown = GetSpellCooldown(spell);
            if (Time.time >= spellCooldowns[spellPrefab] + cooldown)
            {
                // Find target and cast spell
                float range = GetSpellRange(spell);
                Transform target = FindNearestTarget(range);
                if (target != null)
                {
                    CastSpell(spellPrefab, target, GetSpellName(spell));
                    spellCooldowns[spellPrefab] = Time.time;
                }
            }
        }
    }
    
    // ===== SPELL MANAGEMENT METHODS =====
    
    public void AddSpell(GameObject spellPrefab)
    {
        // Add new spell to equipped list and initialize tracking
        if (spellPrefab != null && !equippedSpells.Contains(spellPrefab))
        {
            equippedSpells.Add(spellPrefab);
            spellCooldowns[spellPrefab] = 0f;
            
            SpellProjectile spellComp = spellPrefab.GetComponent<SpellProjectile>();
            if (spellComp != null)
            {
                spellComponents[spellPrefab] = spellComp;
            }
        }
    }
    
    public void RemoveSpell(GameObject spellPrefab)
    {
        // Remove spell from equipped list and cleanup tracking
        if (equippedSpells.Contains(spellPrefab))
        {
            equippedSpells.Remove(spellPrefab);
            spellCooldowns.Remove(spellPrefab);
            spellComponents.Remove(spellPrefab);
        }
    }
    
    public bool HasSpell(GameObject spellPrefab)
    {
        return equippedSpells.Contains(spellPrefab);
    }
    
    public int GetEquippedSpellCount()
    {
        return equippedSpells.Count;
    }
    
    // ===== SPELL CALCULATION METHODS =====
    
    SpellProjectile GetSpellComponent(GameObject spellPrefab)
    {
        // Get cached spell component or fetch it fresh
        if (spellComponents.ContainsKey(spellPrefab))
            return spellComponents[spellPrefab];
        
        return spellPrefab.GetComponent<SpellProjectile>();
    }
    
    float GetSpellCooldown(SpellProjectile spell)
    {
        // Calculate final cooldown based on spell type and global speed
        if (spell is SpellFireball fireball)
            return fireball.GetSpellCooldown(globalAttackSpeed);
        else if (spell is SpellFrozenOrb frozenOrb)
            return frozenOrb.GetSpellCooldown(globalAttackSpeed);
        else
            return 1.0f / globalAttackSpeed; // Default cooldown
    }
    
    float GetSpellRange(SpellProjectile spell)
    {
        // Calculate final range based on spell type and global bonus
        if (spell is SpellFireball fireball)
            return fireball.GetSpellRange(globalRangeBonus);
        else if (spell is SpellFrozenOrb frozenOrb)
            return frozenOrb.GetSpellRange(globalRangeBonus);
        else
            return 8f + globalRangeBonus; // Default range
    }
    
    string GetSpellName(SpellProjectile spell)
    {
        // Get display name for the spell
        if (spell is SpellFireball fireball)
            return fireball.spellName;
        else if (spell is SpellFrozenOrb frozenOrb)
            return frozenOrb.spellName;
        else
            return "Unknown Spell";
    }
    
    // ===== CORE FUNCTIONALITY =====
    
    Transform FindNearestTarget(float range)
    {
        // Find closest enemy within spell range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;
        
        float closestDistance = Mathf.Infinity;
        Transform nearestTarget = null;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= range)
            {
                closestDistance = distance;
                nearestTarget = enemy.transform;
            }
        }
        return nearestTarget;
    }
    
    void CastSpell(GameObject spellPrefab, Transform target, string spellName)
    {
        // Create spell instance and set its direction toward target
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + direction * 1f;
        GameObject spell = Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
        
        SpellProjectile spellProjectile = spell.GetComponent<SpellProjectile>();
        if (spellProjectile != null)
        {
            spellProjectile.direction = direction;
        }
    }
    
    // ===== GLOBAL MODIFIERS =====
    
    public void IncreaseAttackSpeed(float amount)
    {
        // Increase global attack speed (for level-ups)
        globalAttackSpeed += amount;
        globalAttackSpeed = Mathf.Clamp(globalAttackSpeed, 0.5f, 3.0f);
    }
    
    public void IncreaseRange(float amount)
    {
        // Increase global range bonus (for level-ups)
        globalRangeBonus += amount;
    }
    
    // ===== UI METHODS =====
    
    public List<string> GetEquippedSpellNames()
    {
        // Get list of all equipped spell names for UI display
        List<string> names = new List<string>();
        foreach (GameObject spellPrefab in equippedSpells)
        {
            SpellProjectile spell = GetSpellComponent(spellPrefab);
            if (spell != null)
                names.Add(GetSpellName(spell));
        }
        return names;
    }
}