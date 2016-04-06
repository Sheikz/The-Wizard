using System.Collections.Generic;
using UnityEngine;

public class SpellSkill : Skill
{
    private SpellController chosenSpell;
    private SpellCaster hero;

    public override void initialize()
    {
        hero = GameManager.instance.hero.GetComponent<SpellCaster>();
        if (hero == null)
            Debug.Log("Hero not found");
    }

    public override void applySkill(GameObject hero)
    {
        if (!hero)
            return;

        if (chosenSpell == null)
            return;

        //hero.GetComponent<SpellCaster>().addSpell(chosenSpell);
    }

    /// <summary>
    /// Is there at least one skill that can be chosen?
    /// </summary>
    /// <returns></returns>
    public override bool canBeChosen()
    {
        if (hero == null)
            Debug.Log("hero not found");
        if (hero.getKnownSpells() == null)
            Debug.Log("No known spells");
        foreach(GameObject spell in GameManager.instance.spellManager.spellList)
        {
            SpellController spController = spell.GetComponent<SpellController>();

            if (isSpellAvailable(spController))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Chose the skill that will be picked
    /// </summary>
    override public void initializeSkill()
    {
        base.initializeSkill();
        List<GameObject> spellsToPickFrom = new List<GameObject>();
        foreach (GameObject spell in GameManager.instance.spellManager.spellList)
        {
            SpellController spController = spell.GetComponent<SpellController>();

            if (isSpellAvailable(spController))
                spellsToPickFrom.Add(spell);

        }
        chosenSpell = Utils.pickRandom(spellsToPickFrom).GetComponent<SpellController>();
        if (chosenSpell == null)
            return;
        spr.sprite = chosenSpell.GetComponent<SpellController>().icon;
    }

    /// <summary>
    /// Check if the spell is a) not known already and b) the hero already has the prerequisites
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
    private bool isSpellAvailable(SpellController spController)
    {
        // Does the hero already know the spell?
        if (!hero.getKnownSpells().Contains(spController))
        {
            return true;
        }
        return false;
    }
}
