using UnityEngine;
using System.Collections;
using System;

public class FreezeEffect : StatusEffect 
{
    public float slowPercent = 0.5f;
    public float duration = 3;
    public Color colorMask;

    private SpellController spell;
    private Explosion explosion;

    void Awake()
    {
        spell = GetComponent<SpellController>();
        explosion = GetComponent<Explosion>();
    }

    public override void applyBuff(BuffsReceiver receiver)
    {
        if (!spell && explosion)
            spell = explosion.spell;

        Buff buff = new Buff(BuffType.Freeze, duration, slowPercent);
        if (spell && spell.icon)
            buff.icon = spell.icon;
        if (spell && spell.spellName != "")
            buff.name = spell.spellName;

        receiver.addBuff(buff);
    }
}