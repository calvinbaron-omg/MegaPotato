using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Player Invincibility")]
    public float invincibilityTime = 1f;    // Duration of invincibility after taking damage
    public bool useFlashEffect = true;      // Visual feedback during invincibility
    
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        // Initialize health and get sprite renderer for visual effects
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Handle invincibility frames countdown
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            
            // Visual flash effect during invincibility
            if (useFlashEffect && spriteRenderer != null)
            {
                spriteRenderer.enabled = Mathf.PingPong(Time.time * 10f, 1f) > 0.5f;
            }
            
            // End invincibility when timer expires
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                    spriteRenderer.enabled = true;
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        // Ignore damage if currently invincible
        if (isInvincible) return;
        
        base.TakeDamage(amount);
        
        // Start invincibility if damage was taken and player is still alive
        if (amount > 0 && currentHealth > 0)
        {
            StartInvincibility();
        }
    }

    private void StartInvincibility()
    {
        // Begin invincibility period with visual feedback
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
    }

    protected override void HandleDeath()
    {
        // Trigger game over when player dies
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.TriggerGameOver();
        }
        
        // Deactivate player (for potential respawn system)
        gameObject.SetActive(false);
    }
    
    public bool IsInvincible()
    {
        return isInvincible;
    }
}