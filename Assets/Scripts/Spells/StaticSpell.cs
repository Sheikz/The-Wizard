using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class StaticSpell : SpellController
{
    public float delayBeforeDamage;
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
    }

    protected void FixedUpdate()
    {
        checkIfAlive();
        /*
        if (!damageOverTime)
            return;

        for (int i = affectedObjects.Count - 1; i >= 0; i --)
        {
            if (affectedObjects[i] == null)
            {
                affectedObjects.RemoveAt(i);
                continue;
            }
            affectedObjects[i].doDamage(emitter, damage);
        }*/
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
    /*
    // Weird fix because OnTriggerStay2D randomly doesn't work. Need to keep a list of objects triggering
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!damageOverTime)
            return;

        Damageable dmg = other.GetComponent<Damageable>(); ;
        if (dmg)
        {
            affectedObjects.Add(dmg);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!damageOverTime)
            return;

        Damageable dmg = other.GetComponent<Damageable>();
        if (dmg)
        {
            affectedObjects.Remove(dmg);
        }
    }
    */

    /*  Should work, does not work
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Ontriggerstay: " + other.gameObject.name);
        Damageable dmg = other.gameObject.GetComponent<Damageable>();
        if (dmg)
            dmg.doDamage(emitter, damage);
    }*/
}
