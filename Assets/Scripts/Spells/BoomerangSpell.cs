using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(AutoPilot))]
public class BoomerangSpell : MovingSpell
{
    private AutoPilot autoPilot;
    private Vector3 targetPosition;
    private enum BoomerangState { Going, ComingBack };
    private BoomerangState state;

    private DrainSpell drainSpell;

    new void Awake()
    {
        base.Awake();
        autoPilot = GetComponent<AutoPilot>();
        drainSpell = GetComponent<DrainSpell>();
    }

    new void Start()
    {
        base.Start();
        autoPilot.searchNewTargetIfDead = false;
        autoPilot.lockToTargetPosition(targetPosition);
        autoPilot.rotatingStep = 0.1f;
        state = BoomerangState.Going;
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        BoomerangSpell newSpell = Instantiate(this);
        newSpell.initialize(emitter, emitter.transform.position, target);
        return newSpell;
    }

    void initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        transform.position = position;
        this.emitter = emitter;
        targetPosition = target;

        rb.velocity = (target - position).normalized * speed;
    }

    void FixedUpdate()
    {
        if (autoPilot.state == AutoPilot.PilotState.DoNothing)
            goBackToEmitter();

        switch (state)
        {
            case BoomerangState.Going:
                if ((transform.position - targetPosition).sqrMagnitude <= 0.1f)
                    goBackToEmitter();
                break;
            case BoomerangState.ComingBack:
                if (emitter == null)
                    Destroy(gameObject);
                if ((transform.position - emitter.transform.position).sqrMagnitude <= 0.1f)
                    isBackToEmitter();
                break;
        }

    }

    /// <summary>
    /// Change the target to go back to the emitter
    /// </summary>
    void goBackToEmitter()
    {
        if (!emitter)
            Destroy(gameObject);

        autoPilot.lockToObject(emitter.transform);
        state = BoomerangState.ComingBack;
    }

    /// <summary>
    /// Is back to emitter. Must destroy itself. Give back damage absorbed if there is a drainspell attached
    /// </summary>
    void isBackToEmitter()
    {
        if (drainSpell && emitter)
        {
            Damageable dmg = emitter.GetComponent<Damageable>();
            if (dmg)
            {
                drainSpell.healDamageAbsorbed(dmg);
            }
        }
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, autoPilot.targetPosition);
    }
}
