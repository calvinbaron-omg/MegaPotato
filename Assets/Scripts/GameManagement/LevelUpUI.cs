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
    if (levelUpPanel == null)
    {
        Debug.LogError("levelUpPanel is null!");
        return;
    }
    
    if (levelUpText == null)
    {
        Debug.LogError("levelUpText is null!");
        return;
    }
    
    if (spellOptionsContainer == null)
    {
        Debug.LogError("spellOptionsContainer is null!");
        return;
    }
    
    // Pause the game
    Time.timeScale = 0f;
    
    // Show panel and update text
    levelUpPanel.SetActive(true);
    levelUpText.text = $"LEVEL UP!\nLevel {newLevel}";
    
    // Generate spell options
    GenerateSpellOptions();
    
    // Stop any existing coroutine (safety)
    if (waitRoutine != null)
        StopCoroutine(waitRoutine);
        
    // Start listening for input (as backup selection method)
    waitRoutine = StartCoroutine(WaitForPlayerInput());
}
    
    void GenerateSpellOptions()
    {
        // Clear previous options
        ClearSpellOptions();
        
        if (SpellPoolManager.Instance == null || playerAutoAttack == null)
            return;
            
        // Get spells the player already has (to avoid duplicates)
        List<GameObject> excludedSpells = playerAutoAttack.equippedSpells;
        
        // Get random spell options
        int numberOfOptions = SpellPoolManager.Instance.GetCurrentMaxOptions();
        List<GameObject> spellOptions = SpellPoolManager.Instance.GetRandomSpellOptions(
            numberOfOptions, excludedSpells);
            
        // Create UI for each option
        foreach (GameObject spellPrefab in spellOptions)
        {
            CreateSpellOptionUI(spellPrefab);
        }
        
        // Select first option for keyboard navigation
        if (currentOptions.Count > 0)
        {
            currentOptions[0].SelectOption();
        }
    }
    
    void CreateSpellOptionUI(GameObject spellPrefab)
    {
        if (spellOptionPrefab == null) return;
        
        GameObject optionObject = Instantiate(spellOptionPrefab, spellOptionsContainer);
        SpellOptionUI spellOption = optionObject.GetComponent<SpellOptionUI>();
        
        if (spellOption != null)
        {
            spellOption.Initialize(spellPrefab, this);
            currentOptions.Add(spellOption);
        }
    }
    
    void ClearSpellOptions()
    {
        foreach (SpellOptionUI option in currentOptions)
        {
            if (option != null)
                Destroy(option.gameObject);
        }
        currentOptions.Clear();
        
        // Also destroy any remaining children (safety)
        foreach (Transform child in spellOptionsContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void OnSpellSelected(GameObject selectedSpellPrefab)
    {
        if (playerAutoAttack != null && selectedSpellPrefab != null)
        {
            // Add the spell to player's equipped spells
            playerAutoAttack.AddSpell(selectedSpellPrefab);
            
            // Get spell info for debug
            ISpell spellComponent = selectedSpellPrefab.GetComponent<ISpell>();
            if (spellComponent != null)
            {
                Debug.Log($"Added spell: {spellComponent.SpellName}");
            }
        }
        
        // Resume game
        ResumeGame();
    }
    
    IEnumerator WaitForPlayerInput()
    {
        // Wait until player presses the continue key or makes a selection
        while (!Input.GetKeyDown(continueKey))
        {
            // Also check for number keys to select options quickly
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
        
        // If space is pressed without selection, choose randomly
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
        // Hide the panel and resume the game
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
            
        Time.timeScale = 1f;
        
        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }
        
        ClearSpellOptions();
    }
    
    public void OnContinueButtonPressed()
    {
        // If continue button is pressed without selection, choose randomly
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