using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleTrigger : MonoBehaviour
{
    private ParticleSystem ps;
    private SpellController spell;
    private SpellDamager spellDamager;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        spell = GetComponentInParent<SpellController>();
        spellDamager = GetComponentInParent<SpellDamager>();
    }

    void Start()
    {
        if (!spell)
            return;

        ParticleSystem.CollisionModule module = ps.collision;
        module.collidesWith = spell.enemyLayer;
        if (spell.collidesWithWalls)
            module.collidesWith |= GameManager.instance.layerManager.blockingLayer;
        module.quality = ParticleSystemCollisionQuality.Medium;
    }

    public void OnParticleCollision(GameObject other)
    {
        if (!spellDamager)
            return;

        Damageable dmg = other.gameObject.GetComponent<Damageable>();
        if (!dmg)
            return;

        if (spell.emitter && other.gameObject == spell.emitter.gameObject)
            return;

        spellDamager.applyDamage(dmg);
    }
}
