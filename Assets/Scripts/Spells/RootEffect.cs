using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RootEffect : StatusEffect
{
    public float duration;

    public override void applyBuff(BuffsReceiver receiver)
    {
        SpellController spell = GetComponent<SpellController>();

        Buff newBuff = new Buff(BuffType.Root, duration);
        if (spell && spell.icon)
            newBuff.icon = spell.icon;
        if (spell && spell.spellName != "")
            newBuff.name = spell.spellName;

        receiver.addBuff(newBuff);
    }
}
