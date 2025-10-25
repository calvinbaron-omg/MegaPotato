using UnityEngine;
using System.Collections.Generic;

public class PlayerAutoAttack : MonoBehaviour
{
    [Header("Global Modifiers")]
    [Range(0.5f, 3.0f)]
    public float globalAttackSpeed = 1.0f;
    public float globalRangeBonus = 0f;
    
    [Header("Available Spells")]
    public List<GameObject> equippedSpells = new List<GameObject>(); // Spell prefabs
    
    // Track spell instances and cooldowns
    private Dictionary<GameObject, ISpell> spellInstances = new Dictionary<GameObject, ISpell>();
    private Dictionary<GameObject, float> spellCooldowns = new Dictionary<GameObject, float>();
    
    void Start()
    {
        InitializeSpells();
    }
    
    void InitializeSpells()
    {
        spellInstances.Clear();
        spellCooldowns.Clear();
        
        foreach (GameObject spellPrefab in equippedSpells)
        {
            if (spellPrefab != null)
            {
                // Get the ISpell component from the prefab
                ISpell spell = spellPrefab.GetComponent<ISpell>();
                if (spell != null)
                {
                    spellInstances[spellPrefab] = spell;
                    spellCooldowns[spellPrefab] = 0f;
                }
                else
                {
                    Debug.LogError($"No ISpell component found on {spellPrefab.name}");
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
            
            if (spellInstances.TryGetValue(spellPrefab, out ISpell spell))
            {
                // Check if spell is off cooldown
                float cooldown = spell.GetActualCooldown(globalAttackSpeed);
                
                // Debug log to see what's happening
                if (Time.time >= spellCooldowns[spellPrefab] + cooldown)
                {
                    // Find target and cast spell
                    float range = spell.GetActualRange(globalRangeBonus);
                    Transform target = FindNearestTarget(range);
                    if (target != null)
                    {
                        spell.CastSpell(transform, target.position);
                        spellCooldowns[spellPrefab] = Time.time;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Spell instance not found for {spellPrefab.name}");
            }
        }
    }
    
    // ===== SPELL MANAGEMENT METHODS =====
    
    public void AddSpell(GameObject spellPrefab)
    {
        if (spellPrefab != null && !equippedSpells.Contains(spellPrefab))
        {
            equippedSpells.Add(spellPrefab);
            
            ISpell spell = spellPrefab.GetComponent<ISpell>();
            if (spell != null)
            {
                spellInstances[spellPrefab] = spell;
                spellCooldowns[spellPrefab] = 0f;
            }
        }
    }
    
    public void RemoveSpell(GameObject spellPrefab)
    {
        if (equippedSpells.Contains(spellPrefab))
        {
            equippedSpells.Remove(spellPrefab);
            spellInstances.Remove(spellPrefab);
            spellCooldowns.Remove(spellPrefab);
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
    
    // ===== TARGETING =====
    
    Transform FindNearestTarget(float range)
    {
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
    
    // ===== GLOBAL MODIFIERS =====
    
    public void IncreaseAttackSpeed(float amount)
    {
        globalAttackSpeed += amount;
        globalAttackSpeed = Mathf.Clamp(globalAttackSpeed, 0.5f, 3.0f);
    }
    
    public void IncreaseRange(float amount)
    {
        globalRangeBonus += amount;
    }
    
    // ===== UI METHODS =====
    
    public List<string> GetEquippedSpellNames()
    {
        List<string> names = new List<string>();
        foreach (GameObject spellPrefab in equippedSpells)
        {
            if (spellInstances.TryGetValue(spellPrefab, out ISpell spell))
            {
                names.Add(spell.SpellName);
            }
        }
        return names;
    }
    
    public List<ISpell> GetEquippedSpells()
    {
        List<ISpell> spells = new List<ISpell>();
        foreach (var spell in spellInstances.Values)
        {
            spells.Add(spell);
        }
        return spells;
    }
}