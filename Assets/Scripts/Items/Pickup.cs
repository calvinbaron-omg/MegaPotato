using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { XP, XP5, Gold, Silver, HP }
    
    [Header("Pickup Settings")]
    public PickupType pickupType;
    public float value = 1f;
    
    public void Collect()
    {
        Debug.Log($"Collected {pickupType} with value {value}");
        Destroy(gameObject);
    }
}