using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingSpell : SpellController
{
	public float speed = 5;

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        MovingSpell newSpell = Instantiate(this, position, Quaternion.identity) as MovingSpell;
        if (!newSpell.initialize(emitter, position, target))
            return null;

        return newSpell;
    }

    public SpellController castSpellWithReturn(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        MovingSpell newSpell = Instantiate(this);

        if (!newSpell.initialize(emitter, position, target))
            return null;

        return newSpell;
    }

    /// <summary>
    /// Initiliaze with a direction and the emitter
    /// </summary>
    /// <param name="emitter Tag"></param>
    /// <param name="direction"></param>
    public virtual bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
	{
        transform.position = position;
		this.emitter = emitter;

		rb.velocity = (target - position).normalized * speed;
        applyLayer();
        return true;
	}
}
