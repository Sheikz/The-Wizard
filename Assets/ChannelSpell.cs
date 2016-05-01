using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChannelSpell : SpellController
{
    private ChannelSpell currentChannel;
    private Laser laser;

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
        return currentChannel;
    }

    internal void refresh(Vector3 targetPosition)
    {
        if (laser)
            laser.update(emitter.transform.position, targetPosition);
    }

    internal void stop()
    {
        if (currentChannel)
            Destroy(currentChannel.gameObject);
    }

    internal void update(Vector3 targetPosition)
    {
        if (currentChannel)
            currentChannel.refresh(targetPosition);

        if (currentChannel && currentChannel.emitter)
            currentChannel.emitter.payChannelMana(manaCost, manaCostInterval);
    }
}
