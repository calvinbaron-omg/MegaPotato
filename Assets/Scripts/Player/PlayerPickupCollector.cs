using UnityEngine;
using System.Collections.Generic;

public class PlayerPickupCollector : MonoBehaviour
{
    [Header("Base Collection Settings")]
    public float baseCollectionRadius = 3f;
    public float baseMoveSpeed = 15f;
    
    [Header("Tag Settings")]
    public string pickupTag = "Pickup";
    
    private List<Pickup> activePickups = new List<Pickup>();
    private PlayerLevel playerLevel;
    private PlayerCurrency playerCurrency;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    
    // Current stats after modifiers
    private float currentCollectionRadius;
    private float currentMoveSpeed;
    
    void Start()
    {
        playerLevel = GetComponent<PlayerLevel>();
        playerCurrency = GetComponent<PlayerCurrency>();
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        
        // Subscribe to stat changes
        if (playerStats != null)
        {
            playerStats.OnStatsChanged += UpdateCollectionStats;
        }
        
        UpdateCollectionStats();
        
        // Find all existing pickups using tag
        GameObject[] pickupObjects = GameObject.FindGameObjectsWithTag(pickupTag);
        foreach (GameObject pickupObj in pickupObjects)
        {
            Pickup pickup = pickupObj.GetComponent<Pickup>();
            if (pickup != null)
            {
                activePickups.Add(pickup);
            }
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateCollectionStats;
        }
    }
    
    void UpdateCollectionStats()
    {
        // Get collection radius multiplier from PlayerStats
        float collectionRadiusMultiplier = playerStats != null ? playerStats.GetCollectionRadiusMultiplier() : 1f;
        
        currentCollectionRadius = baseCollectionRadius * collectionRadiusMultiplier;
        currentMoveSpeed = baseMoveSpeed; // Just use base move speed without multiplier
        
    }
    
    void Update()
    {
        CheckForNewPickups();
        MovePickupsTowardPlayer();
    }
    
    void CheckForNewPickups()
    {
        if (Time.frameCount % 30 == 0)
        {
            GameObject[] pickupObjects = GameObject.FindGameObjectsWithTag(pickupTag);
            foreach (GameObject pickupObj in pickupObjects)
            {
                Pickup pickup = pickupObj.GetComponent<Pickup>();
                if (pickup != null && !activePickups.Contains(pickup))
                {
                    activePickups.Add(pickup);
                }
            }
            
            activePickups.RemoveAll(pickup => pickup == null);
        }
    }
    
    void MovePickupsTowardPlayer()
    {
        for (int i = activePickups.Count - 1; i >= 0; i--)
        {
            Pickup pickup = activePickups[i];
            if (pickup == null)
            {
                activePickups.RemoveAt(i);
                continue;
            }
            
            float distance = Vector3.Distance(transform.position, pickup.transform.position);
            
            if (distance <= currentCollectionRadius)
            {
                pickup.transform.position = Vector3.MoveTowards(
                    pickup.transform.position, 
                    transform.position, 
                    currentMoveSpeed * Time.deltaTime
                );
                
                float newDistance = Vector3.Distance(transform.position, pickup.transform.position);
                if (newDistance <= 0.5f)
                {
                    CollectPickup(pickup);
                    activePickups.RemoveAt(i);
                }
            }
        }
    }
    
    void CollectPickup(Pickup pickup)
    {
        if (pickup == null) return;
        
        float xpMultiplier = playerStats != null ? playerStats.GetXPMultiplier() : 1f;
        float goldMultiplier = playerStats != null ? playerStats.GetGoldMultiplier() : 1f;
        float silverMultiplier = playerStats != null ? playerStats.GetSilverMultiplier() : 1f;
        
        switch (pickup.pickupType)
        {
            case Pickup.PickupType.XP:
                if (playerLevel != null) playerLevel.AddXP(pickup.value * xpMultiplier);
                break;
                
            case Pickup.PickupType.XP5:
                if (playerLevel != null) playerLevel.AddXP(pickup.value * xpMultiplier);
                break;
                
            case Pickup.PickupType.Gold:
                if (playerCurrency != null) playerCurrency.AddGold(Mathf.RoundToInt(pickup.value * goldMultiplier));
                break;
                
            case Pickup.PickupType.Silver:
                if (playerCurrency != null) playerCurrency.AddSilver(Mathf.RoundToInt(pickup.value * silverMultiplier));
                break;
                
            case Pickup.PickupType.HP:
                if (playerHealth != null) playerHealth.Heal(pickup.value);
                break;
        }
        
        if (pickup.gameObject != null)
            Destroy(pickup.gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Show current collection radius in editor
        float radius = Application.isPlaying ? currentCollectionRadius : baseCollectionRadius;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}