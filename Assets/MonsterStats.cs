using UnityEngine;
using System.Collections.Generic;
using System;

public class MonsterStats : CharacterStats
{
    protected float dungeonLevel = 1;
    public float dungeonLevelMultiplier = 1.2f;
    public float monsterDamageMultiplier = 0.5f;
    public float difficultyModifier = 1.0f;

    new void Awake()
    {
        base.Awake();
    }

    new void Start()
    {
        dungeonLevel = GameManager.instance.levelNumber;
        level = GameManager.instance.hero.GetComponent<CharacterStats>().level;
        base.Start();
        damageable.multiplyMaxHP(difficultyModifier);
    }

    public override float getDamageMultiplier(MagicElement school)
    {
        return (levelDamageMultiplier() + (dungeonLevel - 1) * (dungeonLevelMultiplier - 1)) * monsterDamageMultiplier * difficultyModifier;
    }
}
