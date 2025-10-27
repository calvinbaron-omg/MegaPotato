using UnityEngine;
using System.Collections.Generic;

public class PlayerAutoAttack : MonoBehaviour
{
    [Header("Global Modifiers")]
    [Range(0.5f, 3.0f)]
    public float globalAttackSpeed = 1.0f;
    public float globalRangeBonus = 0f;

    [Header("Equipped Spell Settings")]
    [SerializeField] private int maxEquippedSpells = 2; // ✅ default 2, can scale later via meta-progression
    public int MaxEquippedSpells => maxEquippedSpells;

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
            if (spellPrefab == null) continue;

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

    void Update()
    {
        foreach (GameObject spellPrefab in equippedSpells)
        {
            if (spellPrefab == null) continue;

            if (spellInstances.TryGetValue(spellPrefab, out ISpell spell))
            {
                float cooldown = spell.GetActualCooldown(globalAttackSpeed);

                if (Time.time >= spellCooldowns[spellPrefab] + cooldown)
                {
                    float range = spell.GetActualRange(globalRangeBonus);
                    Transform target = FindNearestTarget(range);

                    if (target != null)
                    {
                        spell.CastSpell(transform, target.position);
                        spellCooldowns[spellPrefab] = Time.time;
                    }
                }
            }
        }
    }

    // ===== SPELL MANAGEMENT =====

    public void AddSpell(GameObject spellPrefab)
    {
        if (spellPrefab == null) return;

        if (equippedSpells.Contains(spellPrefab))
        {
            Debug.Log($"{spellPrefab.name} is already equipped.");
            return;
        }

        if (equippedSpells.Count >= maxEquippedSpells)
        {
            Debug.Log($"Cannot equip {spellPrefab.name}: Max equipped spells reached ({maxEquippedSpells}).");
            return;
        }

        equippedSpells.Add(spellPrefab);

        ISpell spell = spellPrefab.GetComponent<ISpell>();
        if (spell != null)
        {
            spellInstances[spellPrefab] = spell;
            spellCooldowns[spellPrefab] = 0f;
        }
    }

    public void RemoveSpell(GameObject spellPrefab)
    {
        if (!equippedSpells.Contains(spellPrefab)) return;

        equippedSpells.Remove(spellPrefab);
        spellInstances.Remove(spellPrefab);
        spellCooldowns.Remove(spellPrefab);
    }

    public bool HasSpell(GameObject spellPrefab)
    {
        return equippedSpells.Contains(spellPrefab);
    }

    public bool CanEquipNewSpell()
    {
        return equippedSpells.Count < maxEquippedSpells;
    }

    public int GetEquippedSpellCount()
    {
        return equippedSpells.Count;
    }

    // ===== UPGRADE SUPPORT =====

    // Returns a list of upgradeable spells (those that implement BaseProjectileSpell and are below max level)
    public List<BaseProjectileSpell> GetUpgradeableSpells()
    {
        List<BaseProjectileSpell> upgradable = new List<BaseProjectileSpell>();

        foreach (GameObject prefab in equippedSpells)
        {
            if (prefab == null) continue;

            BaseProjectileSpell spell = prefab.GetComponent<BaseProjectileSpell>();
            if (spell != null && spell.CanLevelUp)
                upgradable.Add(spell);
        }

        return upgradable;
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
        globalAttackSpeed = Mathf.Clamp(globalAttackSpeed + amount, 0.5f, 3.0f);
    }

    public void IncreaseRange(float amount)
    {
        globalRangeBonus += amount;
    }

    // ===== UI HELPERS =====

    public List<string> GetEquippedSpellNames()
    {
        List<string> names = new List<string>();
        foreach (GameObject spellPrefab in equippedSpells)
        {
            if (spellInstances.TryGetValue(spellPrefab, out ISpell spell))
                names.Add(spell.SpellName);
        }
        return names;
    }

    public List<ISpell> GetEquippedSpells()
    {
        List<ISpell> spells = new List<ISpell>();
        foreach (var spell in spellInstances.Values)
            spells.Add(spell);
        return spells;
    }

    // ===== META PROGRESSION HOOK =====

    // Called externally when meta progression upgrades max spell slots
    public void SetMaxEquippedSpells(int newMax)
    {
        maxEquippedSpells = Mathf.Max(1, newMax);
        Debug.Log($"Max equipped spells updated to {maxEquippedSpells}");
    }
}
