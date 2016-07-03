using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChannelSpell : SpellController
{
    private ChannelSpell currentChannel;
    private Laser laser;

    public bool canMoveWhileCasting = false;

    new void Awake()
    {
        base.Awake();
        laser = GetComponent<Laser>();
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        currentChannel = Instantiate(this);
        currentChannel.emitter = emitter;
        currentChannel.refresh(target);
        currentChannel.applyChannelPerks();
        return currentChannel;
    }

    protected void applyChannelPerks()
    {
        PlayerStats stats = emitter.GetComponent<PlayerStats>();
        if (!stats)
            return;

        switch (spellName)
        {
            case "Dragon Breath":
                if (stats.getItemPerk(ItemPerk.DragonBreathMove))
                    canMoveWhileCasting = true;
                break;
            case "Ice Beam":
                if (stats.getItemPerk(ItemPerk.IceBeamMove))
                    canMoveWhileCasting = true;
                break;
        }

        MovingCharacter movingChar = emitter.GetComponent<MovingCharacter>();
        if (movingChar)
            movingChar.enableMovement(canMoveWhileCasting);
    }

    internal void refresh(Vector3 targetPosition)
    {
        if (laser)
            laser.update(emitter.transform.position, targetPosition);
    }

    internal virtual void stop()
    {
        if (currentChannel)
            Destroy(currentChannel.gameObject);
    }

    internal virtual bool update(Vector3 targetPosition)
    {
        if (currentChannel)
            currentChannel.refresh(targetPosition);

        if (currentChannel && currentChannel.emitter)
        {
            return currentChannel.emitter.payChannelMana(manaCost, manaCostInterval);
        }
        return true;
    }
}
