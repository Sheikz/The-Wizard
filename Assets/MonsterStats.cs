using UnityEngine;
using System.Collections.Generic;
using System;

public class MonsterStats : CharacterStats
{
    protected float dungeonLevel = 1;
    [Tooltip("Used for Damage, HP")]
    public float dungeonLevelMultiplier = 1.2f;
    [Tooltip("Used for Damage")]
    public float monsterDamageMultiplier = 1f;
    [Tooltip("Used for Damage, HP")]
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
        float result = getDamageMultiplier();
        result *= getDungeonLevelMultiplier();
        result *= monsterDamageMultiplier * difficultyModifier;
        return result;
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
