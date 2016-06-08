using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class MovingSpell : SpellController
{
	public float speed = 5;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        MoveSpellCaster moveCaster = GetComponent<MoveSpellCaster>();
        if (moveCaster && !moveCaster.canCastSpell(emitter, target))
            return null;

        MovingSpell newSpell = Instantiate(this, emitter.transform.position, Quaternion.identity) as MovingSpell;
        if (!newSpell.initialize(emitter, emitter.transform.position, target))
            return null;

        return newSpell;
    }

    /// <summary>
    /// Initiliaze with a direction and the emitter
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="direction"></param>
    public virtual bool initialize(SpellCaster emitter, Vector2 position, Vector2 target)
	{
        transform.position = position;
		this.emitter = emitter;
        this.target = target;
        if (rb)
            rb.velocity = (target - position).normalized * speed;
        applyLayer();

        SpellAutoPilot autoPilot;
        if (emitter.targetOpponent || emitter.targetAlly)
        {
            autoPilot = GetComponent<SpellAutoPilot>();

            if (autoPilot && autoPilot.activated)
            {
                if (damage >= 0 && emitter.targetOpponent)
                    autoPilot.lockToObject(emitter.targetOpponent.transform);
                else if (emitter.targetAlly)
                {
                    Debug.Log(emitter.name + " locking on ally : " + emitter.targetAlly.name);
                    autoPilot.lockToObject(emitter.targetAlly.transform);
                }
            }
        }

        return true;
	}

    public void refreshSpeed()
    {
        if (rb)
            rb.velocity = rb.velocity.normalized * speed;
    }
}
