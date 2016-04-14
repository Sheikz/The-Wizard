using UnityEngine;
using System.Collections;
using System;

public class SpraySpell : SpellController
{
    public Spray spray;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        if (!initialize(emitter, emitter.transform.position, target))
            return null;
        else
            return this;
    }

    private bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        Spray currentSpray = emitter.getActiveSpray(spray.name);
        if (currentSpray != null)
        {
            currentSpray.initialize(emitter, position, target);
            currentSpray.manaCostInterval = manaCostInterval;
            currentSpray.damage = damage;
        }
        else
        {
            currentSpray = Instantiate(spray, position, Quaternion.identity) as Spray;
            currentSpray.transform.SetParent(emitter.transform);
            currentSpray.name = spray.name;
            currentSpray.spellName = spellName;
            currentSpray.emitter = emitter;
            currentSpray.damage = damage;
            currentSpray.manaCostInterval = manaCostInterval;
            emitter.addSpray(currentSpray);
        }
        return currentSpray.shouldPayMana();
    }
}
