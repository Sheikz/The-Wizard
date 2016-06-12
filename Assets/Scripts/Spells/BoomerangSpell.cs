using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpellAutoPilot))]
public class BoomerangSpell : MovingSpell
{
    private Vector3 targetPosition;
    private enum BoomerangState { Going, ComingBack };
    private BoomerangState state;

    private DrainSpell drainSpell;

    new void Awake()
    {
        base.Awake();
        drainSpell = GetComponent<DrainSpell>();
    }

    new void Start()
    {
        base.Start();
        applyBoomerangPerks();
        autoPilot.searchNewTargetIfDead = false;
        autoPilot.lockToTargetPosition(targetPosition);
        autoPilot.rotatingStep = 0.1f;
        state = BoomerangState.Going;
    }

    private void applyBoomerangPerks()
    {
        if (!stats)
            return;
        switch (spellName)
        {
            case "Doomrang":
                if (drainSpell) drainSpell.activated = stats.getItemPerk(ItemPerk.DoomrangHeal) ? true : false;
                break;
        }
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

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("BlockingLayer"))
            return;

        if (!collision.bounds.Contains(transform.position))
            return;

        explode();
    }

    void explode()
    {
        ExplodingSpell explodingSpell = GetComponent<ExplodingSpell>();
        if (explodingSpell)
            explodingSpell.explode();
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
        if (spellName == ItemPerk.DoomrangDamageTwice.getSpellName() && stats & spellDamager)
        {
            if (stats.getItemPerk(ItemPerk.DoomrangDamageTwice))    // Allow the doomrang to damage twice enemies
                spellDamager.resetDamagedObjects();
        }
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
