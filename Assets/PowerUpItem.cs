using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public struct PowerUpBuff
{
    public MagicElement element;
    public float duration;
    public float multiplier;
}

public class PowerUpItem : Item
{
    public PowerUpBuff buff; 

    public override void isPickedUpBy(Inventory looter)
    {
        SpellCaster spellCaster = looter.GetComponent<SpellCaster>();
        if (!spellCaster)
            return;

        spellCaster.addBuff(buff);
        Destroy(gameObject);
    }
}
