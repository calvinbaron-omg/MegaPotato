using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpellOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("UI Elements")]
    public TMP_Text spellNameText;
    public TMP_Text spellDescriptionText;
    public TMP_Text spellStatsText;
    public Image spellIconImage;
    public Button selectButton;

    [Header("Visuals")]
    public Image backgroundImage;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color hoverColor = new Color(0.9f, 0.9f, 0.7f);

    private LevelUpUI levelUpUI;
    private LevelUpChoiceType choiceType;
    private GameObject spellPrefab;
    private BaseProjectileSpell targetSpell;
    private RolledUpgradeChoice? selectedUpgrade;
    private bool isSelected = false;

    public void InitializeNewSpell(GameObject prefab, LevelUpUI ui)
    {
        levelUpUI = ui;
        choiceType = LevelUpChoiceType.NewSpell;
        spellPrefab = prefab;

        UpdateNewSpellUI();
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    public void InitializeUpgrade(BaseProjectileSpell spell, List<RolledUpgradeChoice> rolledUpgrades, LevelUpUI ui)
    {
        levelUpUI = ui;
        choiceType = LevelUpChoiceType.Upgrade;
        targetSpell = spell;

        if (rolledUpgrades != null && rolledUpgrades.Count > 0)
        {
            // Pick one random upgrade to apply when selected
            selectedUpgrade = rolledUpgrades[Random.Range(0, rolledUpgrades.Count)];
            UpdateUpgradeUI(rolledUpgrades);
        }

        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    private void UpdateNewSpellUI()
    {
        if (spellPrefab == null) return;
        ISpell spell = spellPrefab.GetComponent<ISpell>();
        if (spell == null) return;

        spellNameText.text = spell.SpellName;
        spellDescriptionText.text = spell.Description;
        spellStatsText.text = $"Damage: {spell.BaseDamage}\nCooldown: {spell.BaseCooldown}s\nRange: {spell.BaseRange}";
        if (spellIconImage && spell.Icon)
            spellIconImage.sprite = spell.Icon;
    }

    private void UpdateUpgradeUI(List<RolledUpgradeChoice> rolled)
    {
        if (targetSpell == null) return;

        string rarityName = rolled[0].rarity.ToString();
        Color rarityColor = GetRarityColor(rolled[0].rarity);

        spellNameText.text = $"{targetSpell.SpellName} → Lv {targetSpell.SpellLevel + 1}";
        spellNameText.color = rarityColor;

        spellDescriptionText.text = $"Rarity: {rarityName}";

        string statLines = "";
        foreach (var upgrade in rolled)
            statLines += $"- {upgrade.uiText}\n";

        spellStatsText.text = statLines.TrimEnd('\n');

        if (spellIconImage && targetSpell.Icon)
            spellIconImage.sprite = targetSpell.Icon;
    }

    private Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Uncommon: return new Color(0.3f, 1f, 0.3f);
            case Rarity.Rare: return new Color(0.3f, 0.5f, 1f);
            case Rarity.Epic: return new Color(0.7f, 0.3f, 1f);
            case Rarity.Legendary: return new Color(1f, 0.7f, 0.2f);
            default: return Color.white;
        }
    }

    public void OnSelectButtonClicked()
    {
        if (isSelected) return;
        isSelected = true;

        levelUpUI.OnSpellSelected(choiceType, spellPrefab, targetSpell, selectedUpgrade);
    }

    public void SelectOption()
    {
        if (backgroundImage != null)
            backgroundImage.color = selectedColor;
    }

    public void DeselectOption()
    {
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null && !isSelected)
            backgroundImage.color = hoverColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelectButtonClicked();
    }
}
