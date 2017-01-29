using UnityEngine;
using System.Collections.Generic;
using System;

public class SummonNPC : SpellController
{
    public CompanionController NPC;
    public int maxSummonNumber = 1;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        applyItemPerks(emitter);

        CompanionController newNPC = Instantiate(NPC);
        newNPC.transform.position = target;
        newNPC.initialize(emitter.gameObject, GameManager.instance.map);
        newNPC.killAfterSeconds(duration);

        // Set the level of the companion
        CharacterStats companionStats = newNPC.GetComponent<CharacterStats>();
        CharacterStats masterStats = emitter.GetComponent<CharacterStats>();
        if (companionStats && masterStats)
            companionStats.level = masterStats.level;

        int summonCount = emitter.followerList.FindAll(follower =>{
            return follower.name == newNPC.name;
        }).Count;

        if (summonCount >= maxSummonNumber)
            emitter.followerList.Find(follower => { return follower.name == newNPC.name; }).die();

        emitter.addFollower(newNPC);
        return this;
    }

    void applyItemPerks(SpellCaster emitter)
    {
        var stats = emitter.GetComponent<PlayerStats>();
        if (!stats)
            return;

        if (stats.getItemPerk(ItemPerk.Summon2Treants) && spellName == ItemPerk.Summon2Treants.getSpellName())
            maxSummonNumber = 2;
    }
}
