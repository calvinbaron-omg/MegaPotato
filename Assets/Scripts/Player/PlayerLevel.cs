using UnityEngine;
using System;

public class PlayerLevel : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    
    [Header("XP Scaling")]
    public int baseXPRequired = 100;
    public float xpMultiplierPerLevel = 1.2f;
    
    // Events
    public static event Action<int> OnLevelUp;
    
    void Start()
    {
        CalculateXPForNextLevel();
        UpdateUI();
    }
    
    public void AddXP(float xpAmount)
    {
        // Convert to int to remove decimals
        int xpToAdd = Mathf.RoundToInt(xpAmount);
        currentXP += xpToAdd;
        CheckForLevelUp();
        UpdateUI();
    }
    
   void CheckForLevelUp()
{
    while (currentXP >= xpToNextLevel)
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        CalculateXPForNextLevel();
        
        // Trigger event for other scripts
        OnLevelUp?.Invoke(currentLevel);
        Debug.Log(" OnLevelUp?.Invoked");
        // Double check the event is being called
        if (OnLevelUp == null)
        {
            Debug.Log("OnLevelUp event has no subscribers!");
        }
    }
}
    
    void CalculateXPForNextLevel()
    {
        // Use Mathf.RoundToInt to avoid floating point precision issues
        xpToNextLevel = Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpMultiplierPerLevel, currentLevel - 1));
    }
    
    void UpdateUI()
    {
        //Debug.Log($"Level {currentLevel} - XP: {currentXP}/{xpToNextLevel}");
    }
}