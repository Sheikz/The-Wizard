using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SpellHolder : Item
{
    public List<GameObject> possibleSpells;

    private SpellCaster hero;
    private SpellController spell;
    private SpriteRenderer spr;

    void Start()
    {
        hero = GameManager.instance.hero.GetComponent<SpellCaster>();

        spr = GetComponent<SpriteRenderer>();
        
        spell = Utils.pickRandom(possibleSpells).GetComponent<SpellController>();
        if (spell == null)
            return;

        SpellController controller = spell.GetComponent<SpellController>();
        spr.sprite = controller.icon;
    }

    public override void isPickedUpBy(Inventory other)
    {
        if (!hero)
            return;
        if (other.gameObject == hero.gameObject)
        {
            if (hero.addSpell(spell))
                Destroy(gameObject);
        }
    }

    public void setDroppedSpell(GameObject sp)
    {
        possibleSpells.Add(sp);
        deactivatePickupUntilLeftCollider();
    }
}
