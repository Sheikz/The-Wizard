using UnityEngine;
using System.Collections;
using System;

public class SlashSpell : SpellController
{
    private bool canBeMultiplied = true;

    [Serializable]
    public class ColliderWithStartTime
    {
        public Collider2D collider;
        public float delay;
        public float duration;
    }

    public ColliderWithStartTime[] colliders;

    new void Start()
    {
        base.Start();
        foreach (ColliderWithStartTime c in colliders)
        {
            StartCoroutine(enableForSecondsAfterDelay(c.collider, c.delay, c.duration));
        }
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        SlashSpell newSpell = Instantiate(this);

        if (!newSpell.initialize(emitter, emitter.transform.position, target))
            return null;
        return newSpell;
    }

    public bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        this.emitter = emitter;
        this.target = target;
        transform.SetParent(emitter.transform);
        transform.position = position;
        applyItemPerks();

        rotateAroundY(target - position, Quaternion.Euler(90, 0, 0));
        return true;
    }

    override protected void applyItemPerks()
    {
        PlayerStats stats = emitter.GetComponent<PlayerStats>();
        switch (spellName)
        {
            case "Flaming Whip":
                if (stats && stats.getItemPerk(ItemPerk.FireSlashReflect))
                    activateReflect();
                if (stats && stats.getItemPerk(ItemPerk.FireSlashMultiply) && canBeMultiplied)
                    StartCoroutine(multiply(transform.position, target));
                break;
        }
    }

    /// <summary>
    /// Create 2 others slashes after a certain delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator multiply(Vector3 position, Vector3 target)
    {
        Vector3 farTarget = position + (target - position).normalized * 5f;
        Vector3 position1 = position + (target - position).normalized * 1f;
        Vector3 position2 = position + (target - position).normalized * 2f;

        yield return new WaitForSeconds(0.2f);
        SlashSpell newSpell1 = Instantiate(this);
        newSpell1.canBeMultiplied = false;
        newSpell1.initialize(emitter, position1, farTarget);

        yield return new WaitForSeconds(0.2f);
        SlashSpell newSpell2 = Instantiate(this);
        newSpell2.canBeMultiplied = false;
        newSpell2.initialize(emitter, position2, farTarget);
    }

    void activateReflect()
    {
        foreach (SpellDeflector deflector in GetComponentsInChildren<SpellDeflector>())
            deflector.setActive(true);
    }

    void FixedUpdate()
    {
        if (transform.childCount == 0)
            Destroy(gameObject);

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();

        if (partSystems.Length == 0)
            Destroy(gameObject);

        foreach (ParticleSystem ps in partSystems)
        {
            if (ps.particleCount == 0)
            {
                Destroy(ps.gameObject);
            }
        }
    }

    
}
