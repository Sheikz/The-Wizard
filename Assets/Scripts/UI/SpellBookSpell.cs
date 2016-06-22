using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SpellBookSpell : MonoBehaviour
{
    public GameObject iconPrefab;
    public Text spellLevel;
    public Image spellImage;
    public Button levelUpButton;
    
    private SpellCaster hero;
    private PlayerStats heroStats;
    [HideInInspector]
    public SpellController containedSpell;
    private Color spellColor;

    private SpellBook spellBook;

    public void initialize(SpellController spell)
    {
        containedSpell = spell;
        GameObject newSpell = Instantiate(iconPrefab);
        newSpell.transform.SetParent(transform);
        newSpell.transform.localPosition = Vector3.zero;
        spellImage = newSpell.GetComponent<Image>();
        spellImage.sprite = spell.icon;
        spellColor = spellImage.color;
        hero = GameManager.instance.hero.GetComponent<SpellCaster>();
        heroStats = hero.GetComponent<PlayerStats>();
        spellBook = UIManager.instance.spellBook;
        HoveringToolTip tooltip = GetComponent<HoveringToolTip>();
        if (tooltip)
            tooltip.initialize(hero, containedSpell);
        refreshSpellLevel();
    }

    public void onClick()
    {
        if (!containedSpell)
            return;

        if (heroStats.spellLevels[(int)containedSpell.spellSet, (int)containedSpell.magicElement, (int)containedSpell.spellType] <= 0)
            return; // If the spell is lvl 0, don't equip it

        SoundManager.instance.playSound("ClickOK");
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

        SoundManager.instance.playSound("SelectNewSpell");
        heroStats.pointsToAllocate[(int)containedSpell.spellType]--;
        refreshSpellLevel();
        UIManager.instance.spellWindowByType.activateHelpMessage(false);
        UIManager.instance.spellWindowByType.refresh();
        UIManager.instance.spellWindowBySet.activateHelpMessage(false);
        UIManager.instance.spellWindowBySet.refresh();
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

    public void refresh(PlayerStats heroStats)
    {
        int spellSet = (int)containedSpell.spellSet;
        int spellElement = (int)containedSpell.magicElement;
        int spellType = (int)containedSpell.spellType;
        int spellLevel = heroStats.spellLevels[spellSet, spellElement, spellType];
        int pointsToAllocate = heroStats.pointsToAllocate[spellType];
        if (!heroStats.elementUnlocked[spellElement] || !heroStats.spellTypeUnlocked[spellType])
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        // If we have points to allocate, or if the spell is already known, make it visible
        if (pointsToAllocate > 0 || spellLevel > 0)
            spellColor.a = 1f;
        else        // Else, if should be shadowed
            spellColor.a = 0.5f;

        if (spellLevel >= 1)
            showButton(false);
        else if (pointsToAllocate > 0)
            showButton(true);
        else
            showButton(false);
    }

    
}
