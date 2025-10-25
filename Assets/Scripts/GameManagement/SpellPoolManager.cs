using UnityEngine;
using System.Collections.Generic;

public class SpellPoolManager : MonoBehaviour
{
    [Header("All Available Spells")]
    public List<GameObject> allSpellPrefabs = new List<GameObject>();
    
    [Header("Level Up Settings")]
    public int baseSpellOptions = 3;
    public int maxSpellOptions = 5;
    
    private List<GameObject> unlockedSpells = new List<GameObject>();
    
    public static SpellPoolManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSpellPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeSpellPool()
    {
        // Start with all spells unlocked (you can modify this for progression)
        unlockedSpells = new List<GameObject>(allSpellPrefabs);
    }
    
    public List<GameObject> GetRandomSpellOptions(int numberOfOptions, List<GameObject> excludedSpells = null)
    {
        List<GameObject> availableSpells = new List<GameObject>(unlockedSpells);
        
        // Remove spells the player already has
        if (excludedSpells != null)
        {
            foreach (var excludedSpell in excludedSpells)
            {
                availableSpells.Remove(excludedSpell);
            }
        }
        
        // If we don't have enough spells, return what we have
        if (availableSpells.Count <= numberOfOptions)
        {
            return availableSpells;
        }
        
        // Get random spells
        List<GameObject> selectedSpells = new List<GameObject>();
        List<GameObject> tempList = new List<GameObject>(availableSpells);
        
        for (int i = 0; i < numberOfOptions; i++)
        {
            if (tempList.Count == 0) break;
            
            int randomIndex = Random.Range(0, tempList.Count);
            selectedSpells.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        
        return selectedSpells;
    }
    
    public int GetCurrentMaxOptions()
    {
        // You can add meta-progression logic here later
        return baseSpellOptions;
    }
    
    public void UnlockNewSpell(GameObject spellPrefab)
    {
        if (!unlockedSpells.Contains(spellPrefab))
        {
            unlockedSpells.Add(spellPrefab);
        }
    }
}