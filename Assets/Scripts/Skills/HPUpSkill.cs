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

    public override string getDescription()
    {
        string result = base.getDescription();
        int ratioInPercents = Mathf.RoundToInt((ratioHP - 1.0f) * 100f);
        result = result.Replace("<multiplier>", "<color=orange>" + ratioInPercents + "%</color>");
        return result;
    }
}
