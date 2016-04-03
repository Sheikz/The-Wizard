using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class CharacterStats : MonoBehaviour
{
    public int level = 1;
    public float DamageMultiplierPerLevel = 1.1f;
    public float HPMultiplierPerLevel = 1.1f;
    [Tooltip("Cooldown modifier")]
    public float cooldownModifier = 1f;

    public abstract float getDamageMultiplier(MagicElement school);

    protected Damageable damageable;
    protected SpellCaster caster;

    protected void Awake()
    {
        damageable = GetComponent<Damageable>();
        caster = GetComponent<SpellCaster>();
    }

    protected void Start()
    {
        refreshHP(true);
    }

    public abstract void refreshHP(bool updateCurrentHP);

    public virtual void levelUp()
    {
        level++;
        refreshHP(true);
        if (caster)
            caster.levelUpFollowers();

    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    protected float getDamageMultiplier()
    {
        return Mathf.Pow(DamageMultiplierPerLevel, level-1);
    }

    protected float getHPMultiplier()
    {
        return Mathf.Pow(HPMultiplierPerLevel, level - 1);
    }

}
