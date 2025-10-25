using UnityEngine;

public class SpellFireball : BaseProjectileSpell
{
    [Header("Fireball Settings")]
    [SerializeField] private float burnChance = 0.25f;
    [SerializeField] private float burnDamage = 5f;
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float aoeRadius = 4f;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        GameObject fireball = CreateProjectile(caster, targetPosition);
        FireballBehavior behavior = fireball.AddComponent<FireballBehavior>();
        behavior.Initialize(targetPosition, projectileSpeed, lifetime, baseDamage, burnChance, burnDamage, burnDuration, aoeRadius);
    }
}