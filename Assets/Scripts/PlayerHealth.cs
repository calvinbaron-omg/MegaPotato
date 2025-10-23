using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Player Invincibility")]
    public float invincibilityTime = 1f;
    public bool useFlashEffect = true;
    
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        // Call base Awake first
        base.Awake();
        
        // Player-specific setup
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Handle invincibility frames
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            
            // Visual flash effect
            if (useFlashEffect && spriteRenderer != null)
            {
                spriteRenderer.enabled = Mathf.PingPong(Time.time * 10f, 1f) > 0.5f;
            }
            
            // End invincibility
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
        // Check if invincible
        if (isInvincible) return;
        
        // Apply damage using base method
        base.TakeDamage(amount);
        
        // Start invincibility if damage was taken and we're still alive
        if (amount > 0 && currentHealth > 0)
        {
            StartInvincibility();
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.TriggerGameOver();
        }
        
        // Player-specific death effects
        gameObject.SetActive(false); // Instead of destroy for potential respawn
    }
    
    // Public method to check if currently invincible
    public bool IsInvincible()
    {
        return isInvincible;
    }
    
    // Optional: Test method - remove after testing
    void OnTestDamage()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
            Debug.Log($"Health: {currentHealth}, Invincible: {isInvincible}");
        }
    }
}