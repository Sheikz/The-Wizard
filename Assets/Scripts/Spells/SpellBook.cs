using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// A class containing all the spells the hero has learned
/// </summary>
public class SpellBook : MonoBehaviour
{
    private class SpellBookPanel
    {
        public Transform primary;
        public Transform secondary;
        public Transform defensive;
        public Transform ultimate;

        public Transform get(SpellType t)
        {
            switch (t)
            {
                case SpellType.Primary: return primary;
                case SpellType.Secondary: return secondary;
                case SpellType.Defensive: return defensive;
                case SpellType.Ultimate1: return ultimate;
                case SpellType.Ultimate2: return ultimate;
            }
            return null;
        }
    }

    public GameObject spellBookIcon;

    public Transform primaryPanel;
    public Transform secondaryPanel;
    public Transform defensivePanel;
    public Transform ultimatePanel;

    private HashSet<SpellController> spellList;
    private SpellBookPanel spellPanel;
    private SpellManager spellManager;

    public void initialize()
    {
        spellList = new HashSet<SpellController>();
        spellPanel = new SpellBookPanel();
        spellPanel.primary = primaryPanel;
        spellPanel.secondary = secondaryPanel;
        spellPanel.defensive = defensivePanel;
        spellPanel.ultimate = ultimatePanel;
        spellManager = SpellManager.instance;
        addAllSpells();
    }

    public void addSpell(params SpellController[] spells)
    {
        foreach (SpellController spell in spells)
        {
            if (spell == null)
                continue;
            if (spellList.Add(spell) == true)
            {
                GameObject newIcon = Instantiate(spellBookIcon);
                newIcon.GetComponent<SpellBookSpell>().initialize(spell);
                newIcon.transform.SetParent(spellPanel.get(spell.spellType));
                newIcon.transform.localScale = new Vector3(1, 1, 1);    // Weird bug fixing scale set to 0 for no reason
            }
        }
    }

    /// <summary>
    /// Remove a spell from the GUI (but not from the list of known spells)
    /// </summary>
    /// <param name="spells"></param>
    public void removeSpell(params SpellController[] spells)
    {
        foreach (SpellController spell in spells)
        {
            if (spell == null)
                continue;

            SpellBookSpell[] sps = GetComponentsInChildren<SpellBookSpell>();
            foreach (SpellBookSpell sp in sps)
            {
                if (sp.getContainedSpell() == spell)
                {
                    Destroy(sp.gameObject);
                }
            }
        }
    }

    public void addSpell(params GameObject[] spells)
    {
        foreach (GameObject spellObj in spells)
        {
            if (spellObj == null)
                continue;
            SpellController spell = spellObj.GetComponent<SpellController>();
            if (spellList.Add(spell) == true)
            {
                GameObject newIcon = Instantiate(spellBookIcon);
                newIcon.GetComponent<SpellBookSpell>().initialize(spell);
                newIcon.transform.SetParent(spellPanel.get(spell.spellType));
                newIcon.transform.localScale = new Vector3(1, 1, 1);    // Weird bug fixing scale set to 0 for no reason
            }
        }
    }

    public void addAllSpells()
    {
        foreach (GameObject spell in spellManager.spellList)
        {
            if (!spell)
                continue;
            addSpell(spell.GetComponent<SpellController>());
        }
    }

    public SpellController getRandom(SpellType type)
    {
        List<SpellController> result = new List<SpellController>();
        foreach (SpellController spell in spellList)
        {
            if (spell.spellType == type)
            {
                result.Add(spell);
            }
        }
        return Utils.pickRandom(result);
    }

    public List<SpellController> getSpellsOfType(SpellType type)
    {
        List<SpellController> result = new List<SpellController>();
        foreach (SpellController spell in spellList)
        {
            if (spell.spellType == type)
            {
                result.Add(spell);
            }
        }
        return result;
    }

    public HashSet<SpellController> getSpells()
    {
        return spellList;
    }

    public void open()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        else
        {
            UIManager.instance.closeWindows();
            gameObject.SetActive(true);
        }
    }

    public void close()
    {
        gameObject.SetActive(false);
    }
}
