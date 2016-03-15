using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellBookSpell : MonoBehaviour
{
    public GameObject iconPrefab;
    
    private SpellCaster hero;
    private SpellController containedSpell;
    private SpellBook spellBook;

    public void initialize(SpellController spell)
    {
        containedSpell = spell;
        GameObject newSpell = Instantiate(iconPrefab);
        newSpell.transform.SetParent(transform);
        newSpell.GetComponent<Image>().sprite = spell.icon;
        hero = GameManager.instance.hero.GetComponent<SpellCaster>();
        spellBook = UIManager.instance.spellBook;
    }

    public void onClick()
    {
        if (!containedSpell)
            return;

        hero.equipSpell(containedSpell);
        spellBook.close();
    }

    public SpellController getContainedSpell()
    {
        return containedSpell;
    }
}
