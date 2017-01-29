using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Damageable))]
public class SpellAbsorbDamage : MonoBehaviour 
{
    public int absorbDamage;
    public Explosion explosion;
    public Buff buff;

    private Damageable dmg;
    private Damageable emitterDmg;
    private SpellController spell;
    private BuffsReceiver bReceiver;

    void Awake()
    {
        dmg = GetComponent<Damageable>();
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        if (spell)
            emitterDmg = spell.emitter.GetComponent<Damageable>();
        if (spell && spell.emitter)
        {
            bReceiver = spell.emitter.GetComponent<BuffsReceiver>();
            absorbDamage = (int) (absorbDamage * spell.getMultiplier(spell.emitter));
        }
       
        dmg.baseHP = absorbDamage;
        dmg.maxHP = absorbDamage;
        dmg.currentHP = absorbDamage;
        if (emitterDmg)
            emitterDmg.setInvincible(true);

        applyBuff();
    }

    void applyBuff()
    {
        if (bReceiver)
        {
            buff.timeLeft = spell.duration;
            buff.icon = spell.icon;
            bReceiver.addBuff(buff);
        }
    }

    public int getAbsorbDamage(SpellCaster caster)
    {
        if (!caster)
            return absorbDamage;
        return (int)(absorbDamage * GetComponent<SpellController>().getMultiplier(caster));
    }

    void OnDestroy()
    {
        if (GameManager.instance.isShuttingDown)
            return;

        if (emitterDmg)
            emitterDmg.setInvincible(false);
        if (bReceiver)
            bReceiver.removeBuff(buff);

        if (explosion && spell)
        {
            Explosion newExplosion = Instantiate(explosion, transform.position, Quaternion.identity) as Explosion;
            newExplosion.initialize(spell);
        }
    }
}
