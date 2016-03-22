using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class CharacterStats : MonoBehaviour
{
    public int level = 1;
    public float progressionRatio = 1.05f;
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
        if (damageable)
            damageable.multiplyMaxHP(levelDamageMultiplier());
    }

    public virtual void levelUp()
    {
        level++;
        damageable.multiplyMaxHP(1.05f);
        if (caster)
            caster.levelUpFollowers();

    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    protected float levelDamageMultiplier()
    {
        return Mathf.Pow(progressionRatio, level-1);
    }
}
