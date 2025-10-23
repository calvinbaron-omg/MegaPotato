using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;
    public Image healthFill;

    void Start()
    {
        // Auto-find the player's Health component
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                // Remove the Debug.Log since it's working
                // Debug.Log("Found player health component!");
            }
        }
    }

    private void Update()
    {
        if (playerHealth != null && healthFill != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            healthFill.fillAmount = fillAmount;
        }
    }
}