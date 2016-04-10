using UnityEngine;
using System.Collections.Generic;
using System;

public class SummonNPC : SpellController
{
    public CompanionController NPC;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        CompanionController newNPC = Instantiate(NPC);
        newNPC.transform.position = target;
        newNPC.initialize(emitter.gameObject, GameManager.instance.map);
        newNPC.killAfterSeconds(duration);

        // Set the level of the companion
        CharacterStats companionStats = newNPC.GetComponent<CharacterStats>();
        if (companionStats)
        {
            CharacterStats masterStats = emitter.GetComponent<CharacterStats>();
            if (masterStats)
                companionStats.level = masterStats.level;
        }

        emitter.addFollower(newNPC);
        return this;
    }
}
