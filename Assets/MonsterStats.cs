using UnityEngine;
using System.Collections.Generic;
using System;

public class MonsterStats : CharacterStats
{
    protected float dungeonLevel = 1;
    private NPCController monster;

    new void Awake()
    {
        base.Awake();
        monster = GetComponent<NPCController>();
    }

    new void Start()
    {
        dungeonLevel = GameManager.instance.levelNumber;
        level = GameManager.instance.hero.GetComponent<CharacterStats>().level;
        base.Start();
    }

    public override float getDamageMultiplier(MagicElement school)
    {
        return levelDamageMultiplier() + dungeonLevel * 0.2f;
    }
}
