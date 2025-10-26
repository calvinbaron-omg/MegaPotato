using UnityEngine;
using System.Collections.Generic;

public class SpellPoolManager : MonoBehaviour
{
    [Header("All Available Spells")]
    public List<GameObject> allSpellPrefabs = new List<GameObject>();

    [Header("Level Up Settings")]
    public int baseSpellOptions = 3;
    public int maxSpellOptions = 5;

    public static SpellPoolManager Instance;

    private List<GameObject> unlockedSpells = new List<GameObject>();

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
        // Start with all spells unlocked (you can restrict this later)
        unlockedSpells = new List<GameObject>(allSpellPrefabs);
    }

    public int GetCurrentMaxOptions() => baseSpellOptions;

    public void UnlockNewSpell(GameObject spellPrefab)
    {
        if (!unlockedSpells.Contains(spellPrefab))
            unlockedSpells.Add(spellPrefab);
    }

    // =====================================================
    // 🔮 MAIN LEVEL-UP LOGIC
    // =====================================================
    public List<LevelUpChoice> GenerateLevelUpOptions(PlayerAutoAttack player)
    {
        List<LevelUpChoice> finalOptions = new List<LevelUpChoice>();

        if (player == null)
        {
            Debug.LogError("SpellPoolManager: Player reference missing!");
            return finalOptions;
        }

        int totalOptions = GetCurrentMaxOptions();

        bool canEquipNewSpell = player.CanEquipNewSpell();
        List<BaseProjectileSpell> upgradableSpells = player.GetUpgradeableSpells();

        // Track which spells we’ve already used for upgrades
        HashSet<BaseProjectileSpell> usedUpgrades = new HashSet<BaseProjectileSpell>();

        // =====================================================
        // CASE 1: Player can still learn new spells (mix new + upgrades)
        // =====================================================
        if (canEquipNewSpell)
        {
            // Split the total between new spells and upgrades
            int newSpellCount = Mathf.Clamp(totalOptions / 2, 1, totalOptions);
            int upgradeCount = totalOptions - newSpellCount;

            // Prepare shuffled upgradable spell list (to ensure random and unique)
            List<BaseProjectileSpell> shuffledUpgradable = new List<BaseProjectileSpell>(upgradableSpells);
            ShuffleList(shuffledUpgradable);

            // Limit upgrades to unique spells only
            upgradeCount = Mathf.Min(upgradeCount, shuffledUpgradable.Count);

            // Add UPGRADE options first (since those depend on current loadout)
            for (int i = 0; i < upgradeCount; i++)
            {
                BaseProjectileSpell target = shuffledUpgradable[i];
                usedUpgrades.Add(target);

                List<RolledUpgradeChoice> rolled = target.RollUpgradeChoices(2);
                if (rolled.Count > 0)
                {
                    finalOptions.Add(new LevelUpChoice
                    {
                        type = LevelUpChoiceType.Upgrade,
                        targetSpell = target,
                        rolledUpgrades = rolled
                    });
                }
            }

            // If we didn’t have enough upgradable spells, fill the rest with NEW spells
            int remaining = totalOptions - finalOptions.Count;
            if (remaining > 0)
            {
                List<GameObject> newSpells = GetRandomSpellOptions(remaining, player.equippedSpells);
                foreach (var spellPrefab in newSpells)
                {
                    finalOptions.Add(new LevelUpChoice
                    {
                        type = LevelUpChoiceType.NewSpell,
                        spellPrefab = spellPrefab,
                        rolledUpgrades = null,
                        targetSpell = null
                    });
                }
            }
        }
        // =====================================================
        // CASE 2: Player at max spells → upgrades only
        // =====================================================
        else
        {
            // Only upgrades, unique per spell
            List<BaseProjectileSpell> shuffledUpgradable = new List<BaseProjectileSpell>(upgradableSpells);
            ShuffleList(shuffledUpgradable);

            int upgradeOptions = Mathf.Min(totalOptions, shuffledUpgradable.Count);

            for (int i = 0; i < upgradeOptions; i++)
            {
                BaseProjectileSpell target = shuffledUpgradable[i];
                usedUpgrades.Add(target);

                List<RolledUpgradeChoice> rolled = target.RollUpgradeChoices(2);
                if (rolled.Count > 0)
                {
                    finalOptions.Add(new LevelUpChoice
                    {
                        type = LevelUpChoiceType.Upgrade,
                        targetSpell = target,
                        rolledUpgrades = rolled
                    });
                }
            }
        }

        return finalOptions;
    }

    // =====================================================
    // 🧙 SPELL POOL HELPERS
    // =====================================================
    public List<GameObject> GetRandomSpellOptions(int numberOfOptions, List<GameObject> excludedSpells = null)
    {
        List<GameObject> availableSpells = new List<GameObject>(unlockedSpells);

        // Remove already equipped spells
        if (excludedSpells != null)
        {
            foreach (var excluded in excludedSpells)
                availableSpells.Remove(excluded);
        }

        List<GameObject> selected = new List<GameObject>();
        List<GameObject> tempList = new List<GameObject>(availableSpells);

        for (int i = 0; i < numberOfOptions && tempList.Count > 0; i++)
        {
            int index = Random.Range(0, tempList.Count);
            selected.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        return selected;
    }

    // =====================================================
    // 🧩 UTILITIES
    // =====================================================
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

// =====================================================
// 🧩 DATA STRUCTURES
// =====================================================
public enum LevelUpChoiceType
{
    NewSpell,
    Upgrade
}

[System.Serializable]
public class LevelUpChoice
{
    public LevelUpChoiceType type;

    // For new spells
    public GameObject spellPrefab;

    // For upgrades
    public BaseProjectileSpell targetSpell;
    public List<RolledUpgradeChoice> rolledUpgrades;
}
