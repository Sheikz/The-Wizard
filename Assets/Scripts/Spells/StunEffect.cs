using System;
using UnityEngine;

public class StunEffect : StatusEffect
{
    public float stunDuration;

    public override void applyBuff(BuffsReceiver receiver)
    {
        Buff newBuff = new Buff(BuffType.Stun, stunDuration);
        SpellController spell = GetComponent<SpellController>();
        if (spell)
        {
            newBuff.icon = spell.icon;
            newBuff.name = spell.spellName;
        }

        receiver.addBuff(newBuff);
    }
}
