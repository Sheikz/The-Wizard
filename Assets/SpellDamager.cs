using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SpellDamager : MonoBehaviour
{
    public bool activated = true;
    public enum DamageType { DamageOnce, DamageOverTime, None };
    public DamageType damageType = DamageType.DamageOverTime;
    public float delayBetweenDamage = 0.25f;
    [Tooltip("Ratio of the parent damage it should do")]
    public float damageRatio = 1f; 

    private SpellController spell;
    private Explosion explosion;
    private List<Damageable> damagedObjects;
    private DrainSpell drainSpell;
    private StatusEffect[] statusEffects;
    private int damage;
    private SpellCaster emitter;

    void Awake()
    {
        spell = GetComponent<SpellController>();
        if (!spell)
            spell = GetComponentInParent<SpellController>();
        explosion = GetComponent<Explosion>();
        if (!explosion)
            explosion = GetComponentInParent<Explosion>();

        damagedObjects = new List<Damageable>();
        drainSpell = GetComponent<DrainSpell>();
        statusEffects = GetComponentsInChildren<StatusEffect>();
    }

    void Start()
    {
        applyLayer();
        if (spell)
        {
            damage = Mathf.CeilToInt(spell.damage * damageRatio);
            emitter = spell.emitter;
        }
        else if (explosion)
        {
            damage = Mathf.CeilToInt(explosion.damage * damageRatio);
            emitter = explosion.emitter;
        }
        else
            Debug.LogError("No spell or explosion defined for " + name);
    }

    private void applyLayer()
    {
        if (!spell && explosion)
            gameObject.layer = explosion.gameObject.layer;

        if (!spell)
            return;

        if (spell.collidesWithBothParties)
        {
            gameObject.layer = LayerMask.NameToLayer("MonstersAndHero");
        }
        else if (spell.emitter == null)
        {
            Debug.Log("Emitter not defined for " + name);
            return;
        }
        else if (spell.emitter.isMonster)
        {
            gameObject.layer = LayerMask.NameToLayer("MonsterSpells");
        }
        else
            gameObject.layer = LayerMask.NameToLayer("Spells");
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!activated)
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

        StartCoroutine(damageObject(dmg));

        BuffsReceiver receiver = dmg.GetComponent<BuffsReceiver>();
        if (!receiver)
            return;

        foreach (StatusEffect effect in statusEffects)
        {
            effect.applyBuff(receiver);
        }
    }

    IEnumerator damageObject(Damageable dmg)
    {
        damagedObjects.Add(dmg);
        if (damageType == DamageType.DamageOnce)
            yield break;

        yield return new WaitForSeconds(delayBetweenDamage);
        damagedObjects.Remove(dmg);
    }

    /// <summary>
    /// Remove all objects in the damaged objects list, allowing the spell to hit again
    /// </summary>
    public void resetDamagedObjects()
    {
        damagedObjects.Clear();
    }
}
