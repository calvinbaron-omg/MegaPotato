using UnityEngine;
using TMPro;

public class PlayerCurrencyUI : MonoBehaviour
{
    [Header("Currency References")]
    public TMP_Text goldText;
    public TMP_Text silverRunText;
    public TMP_Text silverTotalText;

    private PlayerCurrency playerCurrency;

    void Start()
    {
        playerCurrency = FindFirstObjectByType<PlayerCurrency>();
        
        if (playerCurrency != null)
        {
            // Subscribe to currency change events
            PlayerCurrency.OnGoldChanged += UpdateGoldUI;
            PlayerCurrency.OnSilverChanged += UpdateSilverUI;
            
            // Initial update
            UpdateGoldUI(playerCurrency.gold);
            UpdateSilverUI(playerCurrency.totalSilver, playerCurrency.silverThisRun);
        }
    }

    void OnDestroy()
    {
        if (playerCurrency != null)
        {
            PlayerCurrency.OnGoldChanged -= UpdateGoldUI;
            PlayerCurrency.OnSilverChanged -= UpdateSilverUI;
        }
    }

    void UpdateGoldUI(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
    }

    void UpdateSilverUI(int totalSilver, int silverThisRun)
    {
        if (silverRunText != null)
        {
            silverRunText.text = $"Silver (Run): {silverThisRun}";
        }
        
        if (silverTotalText != null)
        {
            silverTotalText.text = $"Silver (Total): {totalSilver}";
        }
    }
}