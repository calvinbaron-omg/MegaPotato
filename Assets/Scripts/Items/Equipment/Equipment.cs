using UnityEngine;

public class Equipment : MonoBehaviour
{
    public enum EquipmentType { Boots, Ring, Chest, Gloves, Helm }
    
    [Header("Equipment Settings")]
    public EquipmentType equipmentType;
    public float value = 1f;
    
    public void Collect()
    {
        Destroy(gameObject);
    }
}