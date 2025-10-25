using UnityEngine;

public class SpellBallLightning : BaseProjectileSpell
{
    [Header("Ball Lightning Settings")]
    [SerializeField] private float slowChance = 0.3f;
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float aoeRadius = 3f;

    public override void CastSpell(Transform caster, Vector3 targetPosition)
    {
        GameObject ballLightning = CreateProjectile(caster, targetPosition);
        BallLightningBehavior behavior = ballLightning.AddComponent<BallLightningBehavior>();
        behavior.Initialize(targetPosition, projectileSpeed, lifetime, baseDamage, slowChance, slowAmount, slowDuration, aoeRadius);
    }
}