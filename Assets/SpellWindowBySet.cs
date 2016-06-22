using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellWindowBySet : MonoBehaviour 
{
    public SpellGroup[] spellGroups;
    public Text toAllocate;
    public Text helpMessage;

    public SpellBookSpell spellIconPrefab;

    private PlayerStats heroStats;
    private List<SpellBookSpell> spellIcons;
    private ToAllocateReminder[] reminders;
    private ElementName[] elementNames;

    void Awake()
    {
        heroStats = GameManager.instance.hero.GetComponent<PlayerStats>();
        spellIcons = new List<SpellBookSpell>();
        if (!heroStats)
            Debug.LogWarning("Hero stats not found");

        reminders = GetComponentsInChildren<ToAllocateReminder>();
        elementNames = GetComponentsInChildren<ElementName>();
    }

    public void initialize()
    {
        gameObject.SetActive(true);
        addAllSpells();
    }

    void addAllSpells()
    {
        List<GameObject> spellList = SpellManager.instance.spellList;
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
            if (spell.spellSet != SpellSet.SpellSet1)
                continue;

            SpellBookSpell newIcon = Instantiate(spellIconPrefab);
            newIcon.initialize(spell);
            newIcon.transform.SetParent(spellGroups[(int)spell.spellType].spells[(int)spell.magicElement]);
            newIcon.transform.localPosition = Vector3.zero;
            spellIcons.Add(newIcon);
        }
    }

    public void clickAllocateOk()
    {
        SoundManager.instance.playSound("ClickOK");
        open();
    }

    public void close()
    {
        foreach (Tooltip tooltip in GetComponentsInChildren<Tooltip>())
            tooltip.gameObject.SetActive(false);
        gameObject.SetActive(false);
        GameManager.instance.setPause(false);
    }

    public void open()
    {
        if (gameObject.activeSelf)
        {
            close();
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
        int pointsToAllocate = heroStats.getTotalToAllocate();
        toAllocate.text = pointsToAllocate.ToString();

        foreach (SpellBookSpell spellIcon in spellIcons)
            spellIcon.refresh(heroStats);

        foreach (ToAllocateReminder reminder in reminders)
            reminder.refresh();

        foreach (ElementName elemName in elementNames)
            elemName.refresh();
    }

    public void activateHelpMessage(bool value)
    {
        helpMessage.gameObject.SetActive(value);
    }
}
