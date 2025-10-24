using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour
{
    [Header("XP Bar References")]
    public Image xpBarFill;  
    public TMP_Text xpText;
    public TMP_Text levelText;

    private PlayerLevel playerLevel;
    private float currentDisplayXP;
    private float xpLerpSpeed = 5f;
    void Start()
    {
        playerLevel = FindFirstObjectByType<PlayerLevel>();
        
        if (playerLevel != null)
        {
            PlayerLevel.OnLevelUp += UpdateUI;
            UpdateUI(playerLevel.currentLevel); 
        }
    }

    void OnDestroy()
    {
        if (playerLevel != null)
        {
            PlayerLevel.OnLevelUp -= UpdateUI;
        }
    }



    void Update()
    {
        if (playerLevel != null && xpBarFill != null)
        {
            // Smoothly interpolate to the target XP
            currentDisplayXP = Mathf.Lerp(currentDisplayXP, playerLevel.currentXP, xpLerpSpeed * Time.deltaTime);
            float fillAmount = currentDisplayXP / playerLevel.xpToNextLevel;
            xpBarFill.fillAmount = fillAmount;
            
            if (xpText != null)
            {
                xpText.text = $"{Mathf.RoundToInt(currentDisplayXP)}/{playerLevel.xpToNextLevel}";
            }
        }
    }

    void UpdateUI(int newLevel)
    {
        if (playerLevel != null)
        {
            if (levelText != null)
            {
                levelText.text = $"Lvl. {playerLevel.currentLevel}";
            }

            if (xpText != null)
            {
                xpText.text = $"{playerLevel.currentXP}/{playerLevel.xpToNextLevel}";
            }
        }
    }
    
}