using UnityEngine;

// Holds the fully computed runtime numbers for a spell:
// Base stats + that spell's upgrades + player stats.
public struct SpellRuntimeStats
{
    public float damage;
    public float attackSpeed;
    public float critChance;
    public float critDamage;
    public float size;
    public float aoe;
    public float projectileSpeed;
    public float range;
    public float lifetime;

    public SpellRuntimeStats(
        float damage,
        float attackSpeed,
        float critChance,
        float critDamage,
        float size,
        float aoe,
        float projectileSpeed,
        float range,
        float lifetime
    )
    {
        this.damage = damage;
        this.attackSpeed = attackSpeed;
        this.critChance = critChance;
        this.critDamage = critDamage;
        this.size = size;
        this.aoe = aoe;
        this.projectileSpeed = projectileSpeed;
        this.range = range;
        this.lifetime = lifetime;
    }
}
