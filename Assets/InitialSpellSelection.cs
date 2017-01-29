using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSpellSelection : MonoBehaviour {

	public Transform[] initialSpells;
    public SpellBookSpell spellIconPrefab;
    public bool chosenFirstSpell = false;

    public void initialize()
    {
        SpellController[] spells = {
            SpellManager.getSpell(MagicElement.Fire, SpellType.Primary, SpellSet.SpellSet1).GetComponent<SpellController>(),
            SpellManager.getSpell(MagicElement.Arcane, SpellType.Primary, SpellSet.SpellSet1).GetComponent<SpellController>(),
            SpellManager.getSpell(MagicElement.Ice, SpellType.Primary, SpellSet.SpellSet1).GetComponent<SpellController>()
        };

        int n = 0;
        foreach(SpellController sp in spells)
        {
            SpellBookSpell newIcon = Instantiate(spellIconPrefab);
            newIcon.initialize(sp);
            newIcon.transform.SetParent(initialSpells[n]);
            newIcon.transform.localPosition = Vector3.zero;
            n++;
        }
    }

    internal void chooseSpell(SpellController containedSpell)
    {
        if (chosenFirstSpell)
            return;

        PlayerStats stats = GameManager.instance.hero.GetComponent<PlayerStats>();
        stats.elementUnlocked[(int)containedSpell.magicElement] = true;
        gameObject.SetActive(false);
        GameManager.instance.setPause(false);
        chosenFirstSpell = true;
    }
}
