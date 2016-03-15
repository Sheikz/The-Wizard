using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerStats : CharacterStats
{
    // In order: Fire, Ice, Lightning, Arcane, Other
    protected float[] masteries;

    new void Awake()
    {
        base.Awake();
        int numberOfElements = Enum.GetNames(typeof(MagicElement)).Length;
        masteries = new float[numberOfElements];
        for (int i = 0; i < masteries.Length; i++)
            masteries[i] = 1f;

    }

    public float getMastery(MagicElement school)
    {
        return masteries[(int)school];
    }

    public override float getDamageMultiplier(MagicElement school)
    {
        return getMastery(school) * levelDamageMultiplier();
    }

    public void multiplyMastery(float value, MagicElement magicSchool)
    {
        masteries[(int)magicSchool] *= value;
    }
}
