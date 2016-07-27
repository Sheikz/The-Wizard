using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StaticSpell : SpellController
{
    public float delayBeforeDamage;
    public float colliderDuration;
    [Tooltip("Does the spell verify if it's still alive and destroy itself if not?")]
    public bool automaticDestroy = true;
    public bool needLineOfSight = true;
    public bool createdAtCasterPosition = false;

    public static float IceNovaRadius = 2.5f;
    public static float IceNovaWarningSize = 7f;

    [HideInInspector]
    public float durationLeft;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        if (needLineOfSight && !emitter.hasLineOfSight(target))
            return null;

        if (createdAtCasterPosition)
            target = emitter.transform.position;

        StaticSpell newSpell = Instantiate(this);
        if (!newSpell.initialize(emitter, emitter.transform.position, target))
            return null;
        return newSpell;
    }

    public override bool canCastSpell(SpellCaster spellCaster, Vector3 initialPos, Vector3 target)
    {
        if (needLineOfSight)
            return spellCaster.hasLineOfSight(target);
        else
            return true;
    }

    protected bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        transform.position = target;
        this.emitter = emitter;
        stats = emitter.GetComponent<PlayerStats>();

        foreach (ParticleSystem partSystem in GetComponentsInChildren<ParticleSystem>())
        {
            StartCoroutine(stopLoopingAfterSeconds(partSystem, duration));
        }
        return true;
    }

    protected IEnumerator stopLoopingAfterSeconds(ParticleSystem system, float time)
    {
        if (time == 0)
            yield break;
        yield return new WaitForSeconds(time);
        system.loop = false;
    }

    new protected void Start()
    {
        base.Start();
        StartCoroutine(enableAfterSeconds(delayBeforeDamage));
        StartCoroutine(disableAfterSeconds(colliderDuration));
        //StartCoroutine(destroyAfterSeconds(duration * 2));  // Weird fix because fireVortex tends to stay
        durationLeft = duration;
        if (collidesWithBothParties)
            gameObject.layer = LayerManager.instance.monstersAndHeroLayerInt;
    }

    protected void FixedUpdate()
    {
        if (automaticDestroy)
            checkIfAlive();
        durationLeft -= Time.fixedDeltaTime;
    }

    protected IEnumerator enableAfterSeconds(float seconds)
    {
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
            light.enabled = false;
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;
        yield return new WaitForSeconds(seconds);
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;
        foreach (Light light in lights)
            light.enabled = true;
    }

    protected IEnumerator disableAfterSeconds(float seconds)
    {
        if (seconds == 0)
            yield break;

        yield return new WaitForSeconds(seconds);
        circleCollider.enabled = false;
    }

    public override bool shouldCastSpell(SpellCaster spellCaster, Damageable target)
    {
        switch (spellName)
        {
            case "Ice Nova":
                return (hasEnemiesAround(spellCaster, StaticSpell.IceNovaRadius));
        }
        return true;
    }
}
