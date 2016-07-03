using UnityEngine;
using System.Collections;
using System;

public class SpraySpell : ChannelSpell
{
    private SpraySpell currentSpray;
    public bool lookToCursor = true;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        initialize(emitter, emitter.transform.position, target);

        return this;
    }

    private void initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        if (currentSpray != null)
        {
            currentSpray.emitSpray();
            currentSpray.refresh(emitter, position, target);
        }
        else
        {
            currentSpray = Instantiate(this, position, Quaternion.identity) as SpraySpell;
            currentSpray.transform.SetParent(emitter.transform);
            currentSpray.name = name;
            currentSpray.spellName = spellName;
            currentSpray.emitter = emitter;
        }
        currentSpray.manaCostInterval = manaCostInterval;
        currentSpray.damage = damage;
        currentSpray.applyChannelPerks();
    }

    internal override bool update(Vector3 targetPosition)
    {
        if (currentSpray)
            currentSpray.refresh(currentSpray.emitter, currentSpray.emitter.transform.position, targetPosition);

        if (currentSpray && currentSpray.emitter)
            return currentSpray.emitter.payChannelMana(manaCost, manaCostInterval);
        return true;
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();
        if (lookToCursor)
            transform.rotation = Quaternion.Euler(90, -90, 0);
    }

    void FixedUpdate()
    {
        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in partSystems)
        {
            if (ps.IsAlive(true))
            {
                return;
            }
        }
        Destroy(gameObject);    // If we arrive at this point, it means that the spray is no longer emitting
    }

    public void emitSpray()
    {
        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in partSystems)
        {
            ps.Play();
        }
    }

    public void stopSpray()
    {
        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in partSystems)
        {
            ps.Stop();
        }
    }

    internal override void stop()
    {
        if (currentSpray)
            currentSpray.stopSpray();
    }

    /// <summary>
    /// Rotate the spell to redirect the target
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="position"></param>
    /// <param name="target"></param>
    public bool refresh(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        this.emitter = emitter;
        if (lookToCursor)
            rotateAroundX(target - position, Quaternion.Euler(90, -90, 0));
        return true;
    }
}
