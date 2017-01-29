using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SpellDamager : Damager
{
    public bool activated = true;


    [Tooltip("Ratio of the parent damage it should do")]
    public float damageRatio = 1f;
    public DamageValueType damageValueType = DamageValueType.Absolute;

    [HideInInspector]
    public SpellController spell;
    private Explosion explosion;

    private DrainSpell drainSpell;
    private StatusEffect[] statusEffects;
    public int damage;
    public float relativeDamageRatio;
    private SpellCaster emitter;
    private bool initialized = false;
    private List<DamageListener> damageListeners;

    new void Awake()
    {
        base.Awake();
        if (!spell)
            spell = GetComponent<SpellController>();
        if (!spell)
            spell = GetComponentInParent<SpellController>();
        explosion = GetComponent<Explosion>();
        if (!explosion)
            explosion = GetComponentInParent<Explosion>();

        damageListeners = new List<DamageListener>();
        foreach (DamageListener d in GetComponents<DamageListener>())
        {
            damageListeners.Add(d);
        }

        drainSpell = GetComponent<DrainSpell>();
        statusEffects = GetComponentsInChildren<StatusEffect>();
    }

    public void addDamageListener(DamageListener l)
    {
        damageListeners.Add(l);
    }

    void Start()
    {
        
        initialize();
    }

    void initialize()
    {
        applyLayer();
        if (spell)
        {
            damage = Mathf.CeilToInt(spell.damage * damageRatio);
            emitter = spell.emitter;
        }
        else if (explosion)
        {
            if (explosion.damageValueType == DamageValueType.Ratio)
            {
                damageValueType = DamageValueType.Ratio;
                relativeDamageRatio = explosion.damageRatio * damageRatio;
            }
            else
            {
                damage = Mathf.CeilToInt(explosion.damage * damageRatio);
                emitter = explosion.emitter;
            }
        }
        else
            Debug.LogError("No spell or explosion defined for " + name);

        initialized = true;
    }

    private void applyLayer()
    {
        if (!spell && explosion)
            gameObject.layer = explosion.gameObject.layer;

        if (!spell)
            return;

        if (spell.collidesWithBothParties)
        {
            gameObject.layer = LayerManager.monstersAndHeroLayerInt;
        }
        else if (spell.emitter == null)
        {
            Debug.Log("Emitter not defined for " + name);
            return;
        }
        else if (spell.emitter.isMonster)
        {
            gameObject.layer = LayerManager.monsterSpellsInt;
        }
        else
            gameObject.layer = LayerManager.spellsLayerInt;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        trigger(collision);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        trigger(collision);
    }

    void trigger(Collider2D other)
    {
        if (!activated)
            return;

        if (!initialized)
            return;

        if (damageType == DamageType.None)
            return;

        Damageable dmg = other.gameObject.GetComponent<Damageable>();
        if (!dmg)
            return;

        if (emitter && other.gameObject == emitter.gameObject)
            return;

        applyDamage(dmg);
    }

    public void applyDamage(Damageable dmg)
    {
        if (damagedObjects.Contains(dmg))
            return;

        dmg.doDamage(this, emitter, damage);
        if (drainSpell)
            drainSpell.absorbDamage(damage);

        foreach(DamageListener d in damageListeners)
        {
            d.onDamage(emitter, dmg, damage);
        }

        if (spell && dmg.isUnit)
            spell.giveMana();
        StartCoroutine(damageObject(dmg));

        BuffsReceiver receiver = dmg.GetComponent<BuffsReceiver>();
        if (!receiver)
            return;

        foreach (StatusEffect effect in statusEffects)
        {
            effect.applyBuff(receiver);
        }
    }

    /// <summary>
    /// Remove all objects in the damaged objects list, allowing the spell to hit again
    /// </summary>
    public void resetDamagedObjects()
    {
        damagedObjects.Clear();
    }
}
