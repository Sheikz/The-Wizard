using UnityEngine;
using System.Collections;
using System;

public class SpeedUpScript : Skill
{
    public float additionalSpeed;
    public float multiplicativeSpeed;

    public override void applySkill(GameObject hero)
    {
        hero.GetComponent<PlayerController>().speed += additionalSpeed;
        hero.GetComponent<PlayerController>().speed *= multiplicativeSpeed;
    }

    public override string getDescription()
    {
        string result = base.getDescription();
        result = result.Replace("<multiplier>", "<color=orange>" + additionalSpeed + "</color>");
        return result;
    }
}
