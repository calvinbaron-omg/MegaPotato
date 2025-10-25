using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("UI Elements")]
    public TMP_Text spellNameText;
    public TMP_Text spellDescriptionText;
    public TMP_Text spellStatsText;
    public Image spellIconImage;
    public Button selectButton;
    
    [Header("Selection Visuals")]
    public Image backgroundImage;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color hoverColor = new Color(0.9f, 0.9f, 0.7f);
    
    private GameObject spellPrefab;
    private LevelUpUI levelUpUI;
    private bool isSelected = false;
    
    public void Initialize(GameObject prefab, LevelUpUI ui)
    {
        spellPrefab = prefab;
        levelUpUI = ui;
        
        UpdateUI();
        
        // Add listener to button
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
    }
    
    void UpdateUI()
    {
        if (spellPrefab == null) return;
        
        // Get the ISpell component from the prefab
        ISpell spell = spellPrefab.GetComponent<ISpell>();
        if (spell == null) return;
        
        if (spellNameText != null)
            spellNameText.text = spell.SpellName;
            
        if (spellDescriptionText != null)
            spellDescriptionText.text = spell.Description;
            
        if (spellIconImage != null && spell.Icon != null)
            spellIconImage.sprite = spell.Icon;
            
        if (spellStatsText != null)
        {
            spellStatsText.text = $"Damage: {spell.BaseDamage}\n" +
                                 $"Cooldown: {spell.BaseCooldown}s\n" +
                                 $"Range: {spell.BaseRange}";
        }
    }
    
    public void OnSelectButtonClicked()
    {
        if (isSelected) return;
        
        isSelected = true;
        levelUpUI.OnSpellSelected(spellPrefab);
    }
    
    public void SelectOption()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = selectedColor;
        }
    }
    
    public void DeselectOption()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null && !isSelected)
        {
            backgroundImage.color = hoverColor;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelectButtonClicked();
    }
}