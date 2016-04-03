using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SpellBookSpell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject iconPrefab;
    public Text spellLevel;
    public Image spellImage;
    public Button levelUpButton;
    public Tooltip tooltipPrefab;
    
    private SpellCaster hero;
    private PlayerStats heroStats;
    [HideInInspector]
    public SpellController containedSpell;

    private SpellBook spellBook;
    private Tooltip tooltip;

    public void initialize(SpellController spell)
    {
        containedSpell = spell;
        GameObject newSpell = Instantiate(iconPrefab);
        newSpell.transform.SetParent(transform);
        newSpell.transform.localPosition = Vector3.zero;
        spellImage = newSpell.GetComponent<Image>();
        spellImage.sprite = spell.icon;
        hero = GameManager.instance.hero.GetComponent<SpellCaster>();
        heroStats = hero.GetComponent<PlayerStats>();
        spellBook = UIManager.instance.spellBook;
        refreshSpellLevel();
    }

    public void onClick()
    {
        if (!containedSpell)
            return;

        if (heroStats.spellLevels[(int)containedSpell.spellSet, (int)containedSpell.magicElement, (int)containedSpell.spellType] <= 0)
            return; // If the spell is lvl 0, don't equip it

        hero.equipSpell(containedSpell);
        spellBook.close();
    }

    public SpellController getContainedSpell()
    {
        return containedSpell;
    }

    public void levelUp()
    {
        if (heroStats.pointsToAllocate[(int)containedSpell.spellType] <= 0)
            return;

        if (!heroStats.levelUpSpell(containedSpell))
            return;

        heroStats.pointsToAllocate[(int)containedSpell.spellType]--;
        refreshSpellLevel();
        UIManager.instance.spellWindowByType.activateHelpMessage(false);
        UIManager.instance.spellWindowByType.refresh();
        UIManager.instance.refreshUI();
    }

    private void refreshSpellLevel()
    {
        int level = heroStats.spellLevels[(int)containedSpell.spellSet, (int)containedSpell.magicElement, (int)containedSpell.spellType];
        if (level == 0)
        {
            spellLevel.gameObject.SetActive(false);
        }
        else
        {
            spellLevel.gameObject.SetActive(true);
            spellLevel.text = heroStats.spellLevels[(int)containedSpell.spellSet, (int)containedSpell.magicElement, (int)containedSpell.spellType].ToString();
        }
    }

    public void showButton(bool value)
    {
        levelUpButton.gameObject.SetActive(value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.refresh(hero, containedSpell);
        }
        else
        {
            tooltip = Instantiate(tooltipPrefab);
            tooltip.transform.SetParent(transform);
            tooltip.transform.position = transform.position + new Vector3(25, -10, 0);
            tooltip.refresh(hero, containedSpell);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip)
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}
