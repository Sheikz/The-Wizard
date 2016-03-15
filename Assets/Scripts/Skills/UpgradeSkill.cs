using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UpgradeSkill : Skill
{
    public MagicElement magicSchool;
    public override void applySkill(GameObject hero)
    {
        hero.GetComponent<PlayerStats>().multiplyMastery(1.5f, magicSchool);
    }

    public override void initializeSkill()
    {
        base.initializeSkill();
    }
}
