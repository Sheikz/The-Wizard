using UnityEngine;
using System.Collections;
using System;

public class ChargingSpell : SpellController
{
    [Serializable]
    public struct ChargingSpellStages
    {
        public SpellController spell;
        public float chargingTime;
    }

    public ChargingSpellStages[] spellStages;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        return null;
    }

    public bool castChargingSpell(SpellCaster emitter, Vector3 target, int stage)
    {
        if (spellStages[stage].spell)
        {
            spellStages[stage].spell.castSpell(emitter, target);
            return true;
        }

        return false;
    }
}
