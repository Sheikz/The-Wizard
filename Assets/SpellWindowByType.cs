using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SpellWindowByType : MonoBehaviour
{
    public TypeGroup[] typeGroups;
    public Text toAllocate;
    public Text helpMessage;

    public SpellBookSpell spellIconPrefab;

    private PlayerStats heroStats;
    private int activatedPanel = 0;
    private List<SpellBookSpell> spellIcons;

    void Awake()
    {
        heroStats = GameManager.instance.hero.GetComponent<PlayerStats>();
        spellIcons = new List<SpellBookSpell>();
        if (!heroStats)
            Debug.LogWarning("Hero stats not found");
    }

    public void initialize()
    {
        gameObject.SetActive(true);
        addAllSpells();
        activatePanel((int)SpellType.Primary);
    }

    void addAllSpells()
    {
        List<GameObject> spellList = GameManager.instance.spellManager.spellList;
        List<SpellController> spellCList = new List<SpellController>();
        foreach (GameObject sp in spellList)
        {
            if (!sp)
                continue;

            SpellController spell = sp.GetComponent<SpellController>();
            if (!spell)
            {
                Debug.Log("Spell " + sp.name + " is not valid");
                continue;
            }
            spellCList.Add(sp.GetComponent<SpellController>());

        }
        spellCList.Sort();
        foreach (SpellController spell in spellCList)
        {
            SpellBookSpell newIcon = Instantiate(spellIconPrefab);
            newIcon.initialize(spell);
            newIcon.transform.SetParent(typeGroups[(int)spell.spellType].spells[(int)spell.magicElement, (int)spell.spellSet]);
            spellIcons.Add(newIcon);
        }
    }

    public void activatePanel(int number)
    {
        foreach (TypeGroup typeGroup in typeGroups)
        {
            typeGroup.gameObject.SetActive(false);
        }
        typeGroups[number].gameObject.SetActive(true);
        activatedPanel = number;
        refresh();
    }

    public void close()
    {
        gameObject.SetActive(false);
        GameManager.instance.setPause(false);
    }

    public void open()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            GameManager.instance.setPause(false);
        }
        else
        {
            UIManager.instance.closeWindows();
            gameObject.SetActive(true);
            GameManager.instance.setPause(true);
            refresh();
        }
    }

    public void refresh()
    {
        int pointsToAllocate = heroStats.pointsToAllocate[activatedPanel];
        toAllocate.text = pointsToAllocate.ToString();
        foreach (SpellBookSpell spellIcon in spellIcons)
        {
            int spellSet = (int)spellIcon.containedSpell.spellSet;
            int spellElement = (int)spellIcon.containedSpell.magicElement;
            int spellLevel = heroStats.spellLevels[spellSet, spellElement, activatedPanel];
            if ((int)spellIcon.containedSpell.spellType != activatedPanel)
                continue;   // If the icon is not showing, don't refresh it

            // If we have points to allocate, or if the spell is already known, make it visible
            if (pointsToAllocate > 0 || spellLevel > 0)
            {
                Color tmp = spellIcon.spellImage.color;
                tmp.a = 1f;
                spellIcon.spellImage.color = tmp;
            }
            else        // Else, if should be shadowed
            {
                Color tmp = spellIcon.spellImage.color;
                tmp.a = 0.5f;
                spellIcon.spellImage.color = tmp;
            }

            if (spellLevel >= 1)    
                spellIcon.showButton(false);
            else if (pointsToAllocate > 0)
                spellIcon.showButton(true);
            else
                spellIcon.showButton(false);
        }
    }

    public void activateHelpMessage(bool value)
    {
        helpMessage.gameObject.SetActive(value);
    }
}
