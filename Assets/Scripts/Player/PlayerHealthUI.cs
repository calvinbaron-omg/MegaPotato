using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;  // Reference to player's health component
    public Image healthFill;     // UI Image that represents health bar

    void Start()
    {
        // Auto-find the player's Health component if not assigned
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
        }
    }

    private void Update()
    {
        // Update health bar fill amount based on current health
        if (playerHealth != null && healthFill != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            healthFill.fillAmount = fillAmount;
        }
    }
}