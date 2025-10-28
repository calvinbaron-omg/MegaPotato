using UnityEngine;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statsText;

    [Header("Player Reference")]
    public PlayerStats playerStats;

    [Header("Display Settings")]
    public bool showOffensive = true;
    public bool showCombat = true;
    public bool showMovement = true;
    public bool showCollection = true;
    public bool showSystemic = true;

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (statsText == null)
            statsText = GetComponent<TextMeshProUGUI>();

        // Subscribe to stat change event
        if (playerStats != null)
            playerStats.OnStatsChanged += UpdateStatsDisplay;

        UpdateStatsDisplay();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateStatsDisplay;
    }

    public void UpdateStatsDisplay()
    {
        if (playerStats == null || statsText == null) return;

        string displayText = "";

        // ============================================================
        // ⚔️ OFFENSIVE STATS
        // ============================================================
        if (showOffensive)
        {
            displayText += "<b><color=#FFD700>COMBAT STATS</color></b>\n";
            displayText += $"Damage Multiplier: x{playerStats.GetDamagePercent():0.00}\n";
            displayText += $"Attack Speed Multiplier: x{playerStats.GetAttackSpeed():0.00}\n";
            displayText += $"Crit Chance: {(playerStats.GetCritChance() * 100f):0.#}%\n";
            displayText += $"Crit Damage: {(playerStats.GetCritDamage() * 100f):0.#}%\n"; 
            displayText += $"Size Multiplier: x{playerStats.GetSizeMultiplier():0.00}\n\n";
        }

        // ============================================================
        // 🛡️ COMBAT STATS
        // ============================================================
        if (showCombat)
        {
            //displayText += "<b><color=#80C0FF>COMBAT STATS</color></b>\n";
            displayText += $"Life Steal: {(playerStats.GetLifeSteal() * 100f):0.#}%\n";
            displayText += $"Armor: {(playerStats.GetArmor() * 100f):0.#}%\n";
            displayText += $"Evasion Chance: {(playerStats.GetEvasionChance() * 100f):0.#}%\n\n";
        }

        // ============================================================
        // 🏃 MOVEMENT & MOBILITY
        // ============================================================
        if (showMovement)
        {
            displayText += "<b><color=#98FB98>MOVEMENT</color></b>\n";
            displayText += $"Move Speed: {playerStats.GetMoveSpeed():0.00}\n";
            displayText += $"Jump Height: {playerStats.GetJumpHeight():0.00}\n";
            displayText += $"Max Jumps: {playerStats.GetMaxJumps()}\n\n";
        }

        // ============================================================
        // 💰 COLLECTION & ECONOMY
        // ============================================================
        if (showCollection)
        {
            displayText += "<b><color=#FFDAB9>SYSTEMS</color></b>\n";
            displayText += $"Pickup Radius: x{playerStats.GetPickupRadiusMultiplier():0.00}\n";
            displayText += $"Silver Gain: x{playerStats.GetSilverGain():0.00}\n";
            displayText += $"Gold Gain: x{playerStats.GetGoldGain():0.00}\n";
            displayText += $"Experience Multiplier: x{playerStats.GetExperienceMultiplier():0.00}\n\n";
        }

        // ============================================================
        // ⚙️ SYSTEMIC
        // ============================================================
        if (showSystemic)
        {
            //displayText += "<b><color=#FFB6C1>SYSTEMIC</color></b>\n";
            displayText += $"Difficulty: x{playerStats.GetDifficultyMultiplier():0.00}\n";
            displayText += $"Luck: x{playerStats.GetLuckMultiplier():0.00}\n";
            displayText += $"Elite Spawn Chance: {(playerStats.GetEliteSpawnChance() * 100f):0.#}%\n";
        }

        statsText.text = displayText;
    }
}
