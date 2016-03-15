using UnityEngine;
using System.Collections;
using System;

public class HPUpSkill : Skill
{
    public float ratioHP = 1.1f;

    public override void applySkill(GameObject hero)
    {
        if (!hero)
            return;
        hero.GetComponent<Damageable>().multiplyMaxHP(ratioHP);
    }
}
