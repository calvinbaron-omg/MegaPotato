using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statsText;

    [Header("Player Reference")]
    public PlayerStats playerStats;

    [Header("Display Settings")]
    public bool showArmour = true;
    public bool showMoveSpeed = true;
    public bool showAttackSpeed = true;
    public bool showDamagePercent = true;
    public bool showCritChance = true;
    public bool showCritDamage = true;

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (statsText == null)
            statsText = GetComponent<TextMeshProUGUI>();

        // Subscribe to the stats changed event
        if (playerStats != null)
        {
            playerStats.OnStatsChanged += UpdateStatsDisplay;
        }

        // Initial update
        UpdateStatsDisplay();
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateStatsDisplay;
        }
    }

    public void UpdateStatsDisplay()
    {
        if (playerStats == null || statsText == null) return;

        string displayText = "";

        if (showArmour)
            displayText += $"Armour: {playerStats.CurrentArmor}\n";

        if (showMoveSpeed)
            displayText += $"Move Speed: {playerStats.CurrentMoveSpeed}\n";

        if (showAttackSpeed)
            displayText += $"Attack Speed: {playerStats.CurrentAttackSpeed}\n";

        if (showDamagePercent)
            displayText += $"Damage %: {playerStats.CurrentDamagePercent}\n";

        if (showCritChance)
            displayText += $"Crit Chance: {playerStats.CurrentCritChance}\n";

        if (showCritDamage)
            displayText += $"Crit Damange: {playerStats.CurrentCritDamage}\n";
            
        statsText.text = displayText;
    }
}