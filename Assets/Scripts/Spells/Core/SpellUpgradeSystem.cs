using UnityEngine;
using System;
using System.Collections.Generic;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public static class RarityHelper
{
    // Multiplier relative to "Common"
    public static float GetMultiplier(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Uncommon:  return 2f; // 2x common
            case Rarity.Rare:      return 3f;
            case Rarity.Epic:      return 4f;
            case Rarity.Legendary: return 5f;
            default:               return 1f; // Common
        }
    }

    // Weighted rarity roll.
    // You can tune these drop rates however you like.
    public static Rarity RollRarity()
    {
        // 60% Common, 25% Uncommon, 10% Rare, 4% Epic, 1% Legendary
        float roll = UnityEngine.Random.value;

        if (roll < 0.60f) return Rarity.Common;
        if (roll < 0.85f) return Rarity.Uncommon;
        if (roll < 0.95f) return Rarity.Rare;
        if (roll < 0.99f) return Rarity.Epic;
        return Rarity.Legendary;
    }
}

// Which stat are we upgrading?
public enum SpellStatType
{
    Damage,
    ProjectileSpeed,
    Size,
    CritChance,
    CritDamage,
    AttackSpeed,
    ProjectileCount,
    ProjectileBounce
}

// One stat upgrade choice that will be shown to the player in the level up UI.
// Example: "Crit Chance +15%" with information so we can apply it later.
[Serializable]
public struct RolledUpgradeChoice
{
    public SpellStatType statType;
    public Rarity rarity;
    public float effectiveValue; // already multiplied by rarity
    public string uiText;        // e.g. "Crit Chance +15%"

    public RolledUpgradeChoice(SpellStatType statType, Rarity rarity, float effectiveValue, string uiText)
    {
        this.statType = statType;
        this.rarity = rarity;
        this.effectiveValue = effectiveValue;
        this.uiText = uiText;
    }
}

// Shared helpers for formatting upgrade text for the UI
public static class SpellUpgradeFormatter
{
    // For % style buffs (10% damage, 5% crit chance)
    public static string FormatPercentStat(SpellStatType statType, float valueAsDecimal)
    {
        // valueAsDecimal 0.15 -> "15%"
        int percent = Mathf.RoundToInt(valueAsDecimal * 100f);

        switch (statType)
        {
            case SpellStatType.Damage:
                return $"Damage +{percent}%";
            case SpellStatType.ProjectileSpeed:
                return $"Projectile Speed +{percent}%";
            case SpellStatType.Size:
                return $"Size / AoE Radius +{percent}%";
            case SpellStatType.CritChance:
                return $"Crit Chance +{percent}%";
            case SpellStatType.CritDamage:
                return $"Crit Damage +{percent}%";
            case SpellStatType.AttackSpeed:
                return $"Attack Speed +{percent}%";
            //Keeping these here at placeholders, because it should be in int but maybe not, we can do something like +120% prjectile amount, then do a math.floor when calculating the number of projectiles getting fired.
            //So at the end number of proectiles = 2.2 = 2; And we bisuall say number of prokectiles for 80% be ProjectileCount +80% = 0.8. Same with bounces
            case SpellStatType.ProjectileCount:
                return $"Extra Projectiles +{percent}%";
            case SpellStatType.ProjectileBounce:
                return $"Extra Bounces +{percent}%";
            default:
                return $"{statType} +{percent}%";
        }
    }

    // For flat integer-like buffs (ProjectileCount +1)
    public static string FormatFlatStat(SpellStatType statType, int flatAmount)
    {
        switch (statType)
        {
            case SpellStatType.ProjectileCount:
                return $"+{flatAmount} Projectile";
            case SpellStatType.ProjectileBounce:
                return $"+{flatAmount} Projectile";
            default:
                return $"{statType} +{flatAmount}";
        }
    }
}
