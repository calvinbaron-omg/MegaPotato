using UnityEngine;
using System;

public class PlayerCurrency : MonoBehaviour
{
    [Header("Run Currency (Resets each run)")]
    public int gold = 0;
    public int silverThisRun = 0; // NEW: Track silver gained this run
    
    [Header("Permanent Currency (Saved between runs)")]
    public int totalSilver = 0; // RENAMED: Total silver across all runs
    
    public static event Action<int> OnGoldChanged;
    public static event Action<int, int> OnSilverChanged; // Now sends (totalSilver, silverThisRun)
    
    void Start()
    {
        LoadSilver();
    }
    
    // GOLD METHODS - Run-specific currency
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }
    
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        return false;
    }
    
    public void ResetGold()
    {
        gold = 0;
        OnGoldChanged?.Invoke(gold);
    }
    
    // SILVER METHODS - Permanent meta-currency
    public void AddSilver(int amount)
    {
        silverThisRun += amount; // Track this run's silver
        totalSilver += amount;   // Add to permanent total
        OnSilverChanged?.Invoke(totalSilver, silverThisRun);
        SaveSilver();
    }
    
    public bool SpendSilver(int amount)
    {
        if (totalSilver >= amount)
        {
            totalSilver -= amount;
            OnSilverChanged?.Invoke(totalSilver, silverThisRun);
            SaveSilver();
            return true;
        }
        return false;
    }
    
    public void ResetRunSilver()
    {
        // Only reset the run counter, keep total silver
        silverThisRun = 0;
        OnSilverChanged?.Invoke(totalSilver, silverThisRun);
    }
    
    public void LoadSilver()
    {
        // Load total silver from saved data
        totalSilver = PlayerPrefs.GetInt("PermanentSilver", 0);
        silverThisRun = 0; // Always start run with 0 silver gained
        OnSilverChanged?.Invoke(totalSilver, silverThisRun);
    }
    
    private void SaveSilver()
    {
        // Save total silver to persistent storage
        PlayerPrefs.SetInt("PermanentSilver", totalSilver);
        PlayerPrefs.Save();
    }
    
    // NEW: Call this when a run ends to prepare for next run
    public void OnRunEnd()
    {
        // Silver is automatically saved throughout the run
        // Just reset the run counter for next time
        ResetRunSilver();
    }
    
    // NEW: Call this when starting a new run
    public void OnRunStart()
    {
        ResetGold();
        ResetRunSilver();
    }
    }