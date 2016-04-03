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
        refreshHP(true);
    }

    public override float getDamageMultiplier(MagicElement school)
    {
        return (getDamageMultiplier() + (dungeonLevel - 1) * (dungeonLevelMultiplier - 1)) * monsterDamageMultiplier * difficultyModifier;
    }

    protected float getDungeonLevelMultiplier()
    {
        return Mathf.Pow(dungeonLevelMultiplier, dungeonLevel - 1);
    }

    public override void refreshHP(bool updateCurrentHP)
    {
        if (!damageable)
            return;

        float mult = 1.0f;
        mult *= getHPMultiplier();
        mult *= getDungeonLevelMultiplier();
        mult *= difficultyModifier;
        damageable.multiplyBaseHP(mult, 0, updateCurrentHP);
    }
}
