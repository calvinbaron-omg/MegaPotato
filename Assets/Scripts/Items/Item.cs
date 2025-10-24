using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("General Item Settings")]
    public float rotationSpeed = 100f;

    void Update()
    {
        // Slowly rotate around X axis
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        PlayerStats playerStats = player.GetPlayerStats();
        
        if (player != null)
        {
            ApplyEffect(playerStats);
            Destroy(gameObject); // Remove item after pickup
        }
    }

    // Each item defines its own effect here
    protected abstract void ApplyEffect(PlayerStats player);
}