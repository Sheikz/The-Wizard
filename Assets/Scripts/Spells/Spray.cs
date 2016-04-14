using UnityEngine;
using System.Collections;
using System;

public class Spray : SpellController
{
	private bool isEmitting = true;
    private bool isCasted = true;

    private bool isOnManaCooldown = false;

	// Use this for initialization
	new void Start ()
	{
        base.Start();
        transform.rotation = Quaternion.Euler(90, -90, 0);
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
        if (!isCasted)
            stopSpray();

        isCasted = false;

        if (isEmitting)
            return;

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in partSystems)
        {
            if (ps.IsAlive(true))
            {
                return;
            }
        }
        emitter.removeSpray(this);
        Destroy(gameObject);    // If we arrive at this point, it means that the spray is no longer emitting
    }

    public void emitSpray()
	{
		if (isEmitting)
			return;

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in partSystems)
        {
            ps.Play();
        }
        isEmitting = true;
	}

    public void stopSpray()
    {
        if (!isEmitting)
            return;

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in partSystems)
        {
            ps.Stop();
        }
        isEmitting = false;
    }

    internal bool shouldPayMana()
    {
        if (isOnManaCooldown)
            return false;
        else
            StartCoroutine(manaCostCooldown(manaCostInterval));
        return true;

    }

    private IEnumerator manaCostCooldown(float manaCostInterval)
    {
        isOnManaCooldown = true;
        yield return new WaitForSeconds(manaCostInterval);
        isOnManaCooldown = false;
    }

    /// <summary>
    /// Rotate the spell to redirect the target
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="position"></param>
    /// <param name="target"></param>
    public bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        isCasted = true;
        this.emitter = emitter;
        emitSpray();

        rotateAroundX(target - position, Quaternion.Euler(90, -90, 0));
        return true;
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        Spray spell = Instantiate(this);
        if (!spell.initialize(emitter, emitter.transform.position, target))
            return null;
        return spell;
    }
}
