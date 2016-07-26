using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ParticleSystem))]
public class SpellWarning : MonoBehaviour 
{
    public float radius;
    public float duration;

    private ParticleSystem partSystem;
    private SpellCaster spellCaster;
    private SpellController spell;

    void Awake()
    {
        partSystem = GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
        if (!partSystem.IsAlive(true))
            Destroy(gameObject);
    }

    internal void init(SpellController spell, SpellCaster spellCaster)
    {
        this.spell = spell;
        this.spellCaster = spellCaster;

        setRadius();
        partSystem.startLifetime = spell.castTime;
        transform.SetParent(spellCaster.transform);
        partSystem.Play();
    }

    void setRadius()
    {
        switch (spell.spellName)
        {
            case "Ice Nova":
                partSystem.startSize = StaticSpell.IceNovaWarningSize;
                break;
        }
    }
}
