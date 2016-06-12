using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SpellDamager : MonoBehaviour
{
    public bool activated = true;
    public enum DamageType { DamageOnce, DamageOverTime, None };
    public DamageType damageType = DamageType.DamageOverTime;
    public float delayBetweenDamage = 0f;

    private SpellController spell;
    private List<Damageable> damagedObjects;
    private DrainSpell drainSpell;
    private StatusEffect[] statusEffects;

    void Awake()
    {
        spell = GetComponent<SpellController>();
        if (!spell)
            spell = GetComponentInParent<SpellController>();

        damagedObjects = new List<Damageable>();
        drainSpell = GetComponent<DrainSpell>();
        statusEffects = GetComponentsInChildren<StatusEffect>();
    }

    void Start()
    {
        applyLayer();
    }

    private void applyLayer()
    {
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

        if (spell.emitter && other.gameObject == spell.emitter.gameObject)
            return;

        applyDamage(dmg);
    }

    public void applyDamage(Damageable dmg)
    {
        if (damagedObjects.Contains(dmg))
            return;

        dmg.doDamage(this, spell.emitter, spell.damage);
        if (drainSpell)
            drainSpell.absorbDamage(spell.damage);

        StartCoroutine(damageObject(dmg));

        StatusEffectReceiver receiver = dmg.GetComponent<StatusEffectReceiver>();
        if (!receiver)
            return;

        foreach (StatusEffect effect in statusEffects)
        {
            effect.inflictStatus(receiver);
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
