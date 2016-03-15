using UnityEngine;
using System.Collections;
using System;

public class SpraySpell : SpellController
{
    public Spray spray;

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        SpraySpell newSpell = Instantiate(this);
        if (!newSpell.initialize(emitter, position, target))
            return null;
        return newSpell;
    }

    private bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        Spray currentSpray = emitter.getActiveSpray(spray.name);
        if (currentSpray != null)
        {
            currentSpray.initialize(emitter, position, target);
            currentSpray.damage = damage;
        }
        else
        {
            Spray newSpray = Instantiate(spray, position, Quaternion.identity) as Spray;
            newSpray.transform.SetParent(emitter.transform);
            newSpray.name = spray.name;
            newSpray.emitter = emitter;
            newSpray.damage = damage;
            emitter.addSpray(newSpray);
        }
        Destroy(gameObject);
        return true;
    }
}
