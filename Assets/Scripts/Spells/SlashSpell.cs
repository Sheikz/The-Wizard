using UnityEngine;
using System.Collections;
using System;

public class SlashSpell : SpellController
{
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

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        SlashSpell newSpell = Instantiate(this);

        if (!newSpell.initialize(emitter, position, target))
            return null;
        return newSpell;
    }

    public bool initialize(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        this.emitter = emitter;
        transform.SetParent(emitter.transform);
        transform.position = position;

        rotateAroundY(target - position, Quaternion.Euler(90, 0, 0));
        return true;
    }

    void Update()
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
