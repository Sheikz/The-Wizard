using UnityEngine;
using System.Collections;
using System;

public class ChargingSpell : SpellController
{
    private bool isCharging = false;
    private ChargingSpell charginSpell;


    public struct ChargingSpellStages
    {
        SpellController spell;
        float chargingTime;
    }

    public ChargingSpellStages[] spellStages;

    new void Start()
    {
        base.Start();
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        if (!charginSpell)
        {
            ChargingSpell newSpell = Instantiate(this);
            newSpell.emitter = emitter;
            newSpell.transform.SetParent(emitter.transform);
        }
        else
        {
            charginSpell.isCharging = true;
        }
        return null;
    }

}
