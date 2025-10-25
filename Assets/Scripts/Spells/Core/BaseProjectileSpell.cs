using UnityEngine;

public abstract class BaseProjectileSpell : MonoBehaviour, ISpell
{
    [Header("Spell Information")]
    [SerializeField] protected string spellName = "Unknown Spell";
    [SerializeField] protected string description = "Spell description";
    [SerializeField] protected Sprite icon;
    
    [Header("Base Stats")]
    [SerializeField] protected float baseDamage = 25f;
    [SerializeField] protected float baseCooldown = 1.2f;
    [SerializeField] protected float baseRange = 8f;
    [SerializeField] protected float projectileSpeed = 8f;
    [SerializeField] protected float lifetime = 3f;
    
    [Header("Prefab Reference")]
    [SerializeField] protected GameObject spellPrefab;

    // ISpell Implementation
    public string SpellName => spellName;
    public string Description => description;
    public Sprite Icon => icon;
    public float BaseDamage => baseDamage;
    public float BaseCooldown => baseCooldown;
    public float BaseRange => baseRange;

    public float GetActualCooldown(float globalAttackSpeed) => baseCooldown / globalAttackSpeed;
    public float GetActualRange(float globalRangeBonus) => baseRange + globalRangeBonus;

    public abstract void CastSpell(Transform caster, Vector3 targetPosition);

    protected virtual GameObject CreateProjectile(Transform caster, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - caster.position).normalized;
        Vector3 spawnPosition = caster.position + direction * 1f;
        
        GameObject projectile = Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
        EnsureProjectileComponents(projectile);
        
        return projectile;
    }

    private void EnsureProjectileComponents(GameObject projectile)
    {
        if (projectile.GetComponent<Collider>() == null)
            projectile.AddComponent<SphereCollider>().isTrigger = true;
            
        if (projectile.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
    }
}