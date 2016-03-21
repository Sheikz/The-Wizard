using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class StaticSpell : SpellController
{
    public float delayBeforeDamage;
    public float colliderDuration;
    public bool damageOverTime = true;

    [HideInInspector]
    public List<Damageable> affectedObjects;

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        StaticSpell newSpell = Instantiate(this);
        if (!newSpell.initialize(emitter, position, target))
            return null;
        return newSpell;
    }

    protected bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        transform.position = target;
        this.emitter = emitter;
        affectedObjects = new List<Damageable>();

        foreach (ParticleSystem partSystem in GetComponentsInChildren<ParticleSystem>())
        {
            StartCoroutine(stopLoopingAfterSeconds(partSystem, duration));
        }
        return true;
    }

    IEnumerator stopLoopingAfterSeconds(ParticleSystem system, float time)
    {
        yield return new WaitForSeconds(time);
        system.loop = false;
    }

    new protected void Start()
    {
        base.Start();
        StartCoroutine(enableAfterSeconds(delayBeforeDamage));
        StartCoroutine(disableAfterSeconds(colliderDuration));
    }

    protected void FixedUpdate()
    {
        checkIfAlive();
    }

    protected IEnumerator enableAfterSeconds(float seconds)
    {
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
            light.enabled = false;
        circleCollider.enabled = false;
        yield return new WaitForSeconds(seconds);
        circleCollider.enabled = true;
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
}
