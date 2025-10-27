using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelUpPanel;
    public TMP_Text levelUpText;
    public Transform spellOptionsContainer;

    [Header("Spell Option Prefab")]
    public GameObject spellOptionPrefab;

    [Header("Settings")]
    public KeyCode continueKey = KeyCode.Space;

    private PlayerAutoAttack playerAutoAttack;
    private Coroutine waitRoutine;
    private List<SpellOptionUI> currentOptions = new List<SpellOptionUI>();

    void Start()
    {
        playerAutoAttack = FindFirstObjectByType<PlayerAutoAttack>();

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    void OnEnable()
    {
        PlayerLevel.OnLevelUp += ShowLevelUp;
    }

    void OnDisable()
    {
        PlayerLevel.OnLevelUp -= ShowLevelUp;
    }

    void ShowLevelUp(int newLevel)
    {
        if (levelUpPanel == null || spellOptionPrefab == null || spellOptionsContainer == null)
        {
            Debug.LogError("LevelUpUI references missing!");
            return;
        }

        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);
        levelUpText.text = $"LEVEL UP!\nLevel {newLevel}";

        GenerateOptions();

        if (waitRoutine != null)
            StopCoroutine(waitRoutine);
        waitRoutine = StartCoroutine(WaitForPlayerInput());
    }

    void GenerateOptions()
    {
        ClearOptions();

        if (SpellPoolManager.Instance == null || playerAutoAttack == null)
            return;

        List<LevelUpChoice> choices = SpellPoolManager.Instance.GenerateLevelUpOptions(playerAutoAttack);

        foreach (LevelUpChoice choice in choices)
        {
            CreateOption(choice);
        }

        if (currentOptions.Count > 0)
            currentOptions[0].SelectOption();
    }

    void CreateOption(LevelUpChoice choice)
    {
        GameObject optionObject = Instantiate(spellOptionPrefab, spellOptionsContainer);
        SpellOptionUI option = optionObject.GetComponent<SpellOptionUI>();

        if (option == null) return;

        if (choice.type == LevelUpChoiceType.NewSpell)
        {
            option.InitializeNewSpell(choice.spellPrefab, this);
        }
        else if (choice.type == LevelUpChoiceType.Upgrade)
        {
            option.InitializeUpgrade(choice.targetSpell, choice.rolledUpgrades, this);
        }

        currentOptions.Add(option);
    }

    void ClearOptions()
    {
        foreach (var opt in currentOptions)
        {
            if (opt != null)
                Destroy(opt.gameObject);
        }
        currentOptions.Clear();

        foreach (Transform child in spellOptionsContainer)
            Destroy(child.gameObject);
    }

    public void OnSpellSelected(LevelUpChoiceType type, GameObject newSpellPrefab = null, BaseProjectileSpell upgradeTarget = null, List<RolledUpgradeChoice> selectedUpgrades = null)
    {
        if (playerAutoAttack == null)
        {
            ResumeGame();
            return;
        }

        // NEW SPELL SELECTION
        if (type == LevelUpChoiceType.NewSpell && newSpellPrefab != null)
        {
            if (playerAutoAttack.CanEquipNewSpell())
            {
                playerAutoAttack.AddSpell(newSpellPrefab);
                Debug.Log($"Equipped new spell: {newSpellPrefab.name}");
            }
            else
            {
                Debug.Log("Cannot equip new spell — max slots reached!");
            }
        }

        // UPGRADE SELECTION with Multiple Upgrades
        else if (type == LevelUpChoiceType.Upgrade && upgradeTarget != null && selectedUpgrades != null && selectedUpgrades.Count > 0)
        {
            // Find the active instance of this spell on the player
            BaseProjectileSpell activeInstance = null;
            foreach (ISpell spell in playerAutoAttack.GetEquippedSpells())
            {
                if (spell is BaseProjectileSpell instance && instance.SpellName == upgradeTarget.SpellName)
                {
                    activeInstance = instance;
                    break;
                }
            }

            if (activeInstance != null)
            {
                activeInstance.ApplyUpgradeAndLevel(selectedUpgrades);
            }
            else
            {
                Debug.LogWarning($"No active instance found for {upgradeTarget.SpellName}");
            }
        }



        ResumeGame();
    }

    IEnumerator WaitForPlayerInput()
    {
        while (!Input.GetKeyDown(continueKey))
        {
            for (int i = 0; i < currentOptions.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    currentOptions[i].OnSelectButtonClicked();
                    yield break;
                }
            }
            yield return null;
        }

        if (currentOptions.Count > 0)
        {
            int randomIndex = Random.Range(0, currentOptions.Count);
            currentOptions[randomIndex].OnSelectButtonClicked();
        }
        else
        {
            ResumeGame();
        }
    }

    void ResumeGame()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        Time.timeScale = 1f;

        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }

        ClearOptions();
    }

    public void OnContinueButtonPressed()
    {
        if (currentOptions.Count > 0)
        {
            int randomIndex = Random.Range(0, currentOptions.Count);
            currentOptions[randomIndex].OnSelectButtonClicked();
        }
        else
        {
            ResumeGame();
        }
    }
}
