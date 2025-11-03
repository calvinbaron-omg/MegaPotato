using UnityEngine;
using System.Collections;

public class PlayerHealth : Health
{
    [Header("Player Invincibility")]
    [Tooltip("Duration of brief invulnerability after taking damage.")]
    public float invincibilityTime = 1f;
    [Tooltip("Visual flash effect during invincibility.")]
    public bool useFlashEffect = true;

    [Header("Shield (Full Invincibility) Settings")]
    [Tooltip("Color applied when a temporary shield/invincibility buff is active.")]
    public Color invincibleColor = new Color(1f, 1f, 0.4f, 1f); // yellowish glow
    public bool showShieldEffect = true;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private Coroutine invincibilityRoutine;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        // Handle normal invincibility countdown (from taking damage)
        if (isInvincible && invincibilityRoutine == null)
        {
            invincibilityTimer -= Time.deltaTime;

            if (useFlashEffect && spriteRenderer != null)
                spriteRenderer.enabled = Mathf.PingPong(Time.time * 10f, 1f) > 0.5f;

            if (invincibilityTimer <= 0)
            {
                EndInvincibility();
            }
        }
    }

    public override void TakeDamage(float amount, bool isCrit = false, string source = "Unknown")
    {
        // Ignore all damage while invincible or shielded
        if (isInvincible) return;

        base.TakeDamage(amount, isCrit);

        // Trigger brief iframes after taking damage
        if (amount > 0 && currentHealth > 0)
            StartInvincibility(invincibilityTime);
    }

    public void Heal(float healAmount)
    {
        if (healAmount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }

    private void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
    }

    // ============================================================
    // 🛡️ NEW: Activate Full Invincibility ("Shield")
    // ============================================================
    public void ActivateInvincibility(float duration)
    {
        if (invincibilityRoutine != null)
            StopCoroutine(invincibilityRoutine);

        invincibilityRoutine = StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;

        if (showShieldEffect && spriteRenderer != null)
            spriteRenderer.color = invincibleColor;

        yield return new WaitForSeconds(duration);

        EndInvincibility();
        invincibilityRoutine = null;
    }

    protected override void HandleDeath()
    {
        var gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.TriggerGameOver();

        gameObject.SetActive(false);
    }

    public bool IsInvincible() => isInvincible;
}
