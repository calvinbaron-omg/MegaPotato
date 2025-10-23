using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private Health enemyHealth;
    
    // Status effect tracking
    private bool isSlowed = false;
    private bool isBurning = false;
    private Coroutine slowCoroutine;
    private Coroutine burnCoroutine;

    void Start()
    {
        // Cache references to other enemy components
        enemyMovement = GetComponent<EnemyMovement>();
        enemyHealth = GetComponent<Health>();
    }
    
    // ===== PUBLIC STATUS EFFECT METHODS =====
    
    public void ApplySlow(float slowAmount, float duration)
    {
        // Stop existing slow effect and apply new one
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
            
        slowCoroutine = StartCoroutine(SlowRoutine(slowAmount, duration));
    }
    
    public void ApplyBurn(float burnDamagePerTick, float duration)
    {
        // Stop existing burn effect and apply new one
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);
            
        burnCoroutine = StartCoroutine(BurnRoutine(burnDamagePerTick, duration));
    }
    
    // ===== PRIVATE COROUTINES =====
    
    private IEnumerator SlowRoutine(float slowAmount, float duration)
    {
        isSlowed = true;
        
        // Apply slow to movement speed
        if (enemyMovement != null)
        {
            float originalSpeed = enemyMovement.chaseSpeed;
            enemyMovement.chaseSpeed *= (1f - slowAmount); // Reduce speed by slow percentage
            
            yield return new WaitForSeconds(duration);
            
            // Restore original movement speed
            enemyMovement.chaseSpeed = originalSpeed;
        }
        
        isSlowed = false;
    }
    
    private IEnumerator BurnRoutine(float burnDamagePerTick, float duration)
    {
        isBurning = true;
        float endTime = Time.time + duration;
        
        // Apply damage over time every 0.5 seconds
        while (Time.time < endTime && enemyHealth != null)
        {
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(burnDamagePerTick);
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        isBurning = false;
    }
    
    // ===== STATUS CHECK METHODS =====
    
    public bool IsSlowed()
    {
        return isSlowed;
    }
    
    public bool IsBurning()
    {
        return isBurning;
    }
    
    public bool HasAnyStatusEffect()
    {
        return isSlowed || isBurning;
    }
}