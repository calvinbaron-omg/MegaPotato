using UnityEngine;
using System.Collections.Generic;

public class PickupDrop : MonoBehaviour
{
    [System.Serializable]
    public class DropChance
    {
        public Pickup.PickupType pickupType;
        [Range(0, 1000)] public float dropChance = 100f; // Percentage (can go over 100 for multiple drops)
    }
    
    [Header("Drop Settings")]
    public List<DropChance> dropChances = new List<DropChance>();
    
    public void CalculateDrops()
    {
        foreach (var drop in dropChances)
        {
            if (drop.dropChance <= 0) continue;
            
            // Calculate how many drops based on percentage
            int guaranteedDrops = Mathf.FloorToInt(drop.dropChance / 100f);
            float extraChance = drop.dropChance % 100f;
            
            // Spawn guaranteed drops
            for (int i = 0; i < guaranteedDrops; i++)
            {
                SpawnPickup(drop.pickupType);
            }
            
            // Chance for extra drop
            if (Random.Range(0f, 100f) <= extraChance)
            {
                SpawnPickup(drop.pickupType);
            }
        }
    }
    
    void SpawnPickup(Pickup.PickupType pickupType)
    {
        GameObject pickupPrefab = GetPickupPrefab(pickupType);
        if (pickupPrefab != null)
        {
            // Add random offset to space out multiple pickups
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-0.5f, 0.5f)
            );
            
            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    GameObject GetPickupPrefab(Pickup.PickupType pickupType)
    {
         string prefabName = pickupType switch
        {
            Pickup.PickupType.XP => "Pickups/PickupXPPrefab",
            Pickup.PickupType.XP5 => "Pickups/PickupXP5Prefab", 
            Pickup.PickupType.Gold => "Pickups/PickupGoldPrefab",
            Pickup.PickupType.Silver => "Pickups/PickupSilverPrefab",
            Pickup.PickupType.HP => "Pickups/PickupHPPrefab",
            _ => "Pickups/PickupXPPrefab"
        };
        
        return Resources.Load<GameObject>(prefabName);
    }
}