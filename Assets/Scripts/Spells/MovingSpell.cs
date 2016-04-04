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
        MoveSpellCaster moveCaster = GetComponent<MoveSpellCaster>();
        if (moveCaster && !moveCaster.canCastSpell(emitter, position, target))
            return null;

        MovingSpell newSpell = Instantiate(this, position, Quaternion.identity) as MovingSpell;
        if (!newSpell.initialize(emitter, position, target))
            return null;

        return newSpell;
    }

    /// <summary>
    /// Initiliaze with a direction and the emitter
    /// </summary>
    /// <param name="emitter Tag"></param>
    /// <param name="direction"></param>
    public virtual bool initialize(SpellCaster emitter, Vector2 position, Vector2 target)
	{
        transform.position = position;
		this.emitter = emitter;
        this.target = target;
        rb.velocity = (target - position).normalized * speed;
        applyLayer();
        return true;
	}
}
