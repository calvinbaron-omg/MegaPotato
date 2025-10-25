using UnityEngine;

public class SpellFrozenOrb : BaseProjectileSpell
{
    [Header("Frozen Orb Settings")]
    [SerializeField] private float slowChance = 0.3f;
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float aoeRadius = 3f;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        GameObject frozenOrb = CreateProjectile(caster, targetPosition);
        FrozenOrbBehavior behavior = frozenOrb.AddComponent<FrozenOrbBehavior>();
        behavior.Initialize(targetPosition, projectileSpeed, lifetime, baseDamage, slowChance, slowAmount, slowDuration, aoeRadius);
    }
}