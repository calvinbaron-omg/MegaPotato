using UnityEngine;
using System.Collections.Generic;

public class EquipmentDrop : MonoBehaviour
{
    [System.Serializable]
    public class DropChance
    {
        public Equipment.EquipmentType equipmentType;
        [Range(0, 1000)] public float dropChance = 100f; // Percentage (can go over 100 for multiple drops)
    }
    
    [Header("Drop Settings")]
    public List<DropChance> dropChances = new List<DropChance>();
    
    public void CalculateDrops()
    {
        var drop = dropChances[Random.Range(0, dropChances.Count)];
        SpawnPickup(drop.equipmentType);
    }
    
    void SpawnPickup(Equipment.EquipmentType equipmentType)
    {
        GameObject equipmentPrefab = GetEquipmentPrefab(equipmentType);
        if (equipmentPrefab != null)
        {
            // Add random offset to space out multiple pickups
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-0.5f, 0.5f)
            );
            
            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(equipmentPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    GameObject GetEquipmentPrefab(Equipment.EquipmentType equipmentType)
    {
         string prefabName = equipmentType switch
         {
             Equipment.EquipmentType.Boots => "Equipment/EquipmentBootsPrefab",
             Equipment.EquipmentType.Ring => "Equipment/EquipmentRingPrefab",
             Equipment.EquipmentType.Helm => "Equipment/EquipmentHelmPrefab",
             Equipment.EquipmentType.Chest => "Equipment/EquipmentChestPrefab",
             Equipment.EquipmentType.Gloves => "Equipment/EquipmentGlovesPrefab",
             _ => null
         };
        
        return Resources.Load<GameObject>(prefabName);
    }
}