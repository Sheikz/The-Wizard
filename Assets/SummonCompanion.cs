using UnityEngine;
using System.Collections.Generic;
using System;

public class SummonCompanion : SpellController
{
    public Companion companion;

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        Companion newCompanion = Instantiate(companion);
        newCompanion.transform.SetParent(emitter.transform);
        emitter.addCompanion(newCompanion);

        CharacterStats companionStats = newCompanion.GetComponent<CharacterStats>();
        if (companionStats)
        {
            CharacterStats masterStats = emitter.GetComponent<CharacterStats>();
            if (masterStats)
                companionStats.level = masterStats.level;
        }
        return this;
    }
}
