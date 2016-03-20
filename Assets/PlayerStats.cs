using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerStats : CharacterStats
{
    // In order: Fire, Ice, Lightning, Arcane, Other
    protected float[] masteries;
    public int[,,] spellLevels; // In order: set, element, type
    public int[] pointsToAllocate;

    private int numberOfTypes;
    private int numberOfElements;
    private int numberOfSets;
    private SpellCaster hero;

    new void Awake()
    {
        base.Awake();
        numberOfElements = Enum.GetNames(typeof(MagicElement)).Length;
        numberOfTypes = Enum.GetNames(typeof(SpellType)).Length;
        numberOfSets = Enum.GetNames(typeof(SpellSet)).Length;
        masteries = new float[numberOfElements];
        for (int i = 0; i < masteries.Length; i++)
            masteries[i] = 1f;

        spellLevels = new int[numberOfSets, numberOfElements, numberOfTypes];
        hero = GetComponent<SpellCaster>();
        pointsToAllocate = new int[numberOfTypes];
        pointsToAllocate[(int)SpellType.Primary] = 1;   // The hero stats with 1 point to allocate in the primary spells
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

    /// <summary>
    /// Level up a spell, return true if the levelup has been accepted
    /// </summary>
    /// <param name="containedSpell"></param>
    /// <returns></returns>
    public bool levelUpSpell(SpellController containedSpell)
    {
        int spellType = (int)containedSpell.spellType;
        int spellElement = (int)containedSpell.magicElement;
        int spellSet = (int)containedSpell.spellSet;
        if (spellLevels[spellSet, spellElement, spellType] >= 1)    // If the spell is already lvl 1, we cannot upgrade it
            return false;   
        else
        {
            spellLevels[spellSet, spellElement, spellType]++;
            if (hero.spellList[spellType] == null)
                hero.equipSpell(containedSpell);
        }

        UIManager.instance.spellWindowByType.refresh();
        return true;
    }

    override public void levelUp()
    {
        base.levelUp();

        if (level % 3 == 0)
            pointsToAllocate[(int)SpellType.Primary]++;
        if ((level+1) % 3 == 0)
            pointsToAllocate[(int)SpellType.Secondary]++;

        if (level % 4 == 0)
            pointsToAllocate[(int)SpellType.Defensive]++;

        if (level % 5 == 0)
            pointsToAllocate[(int)SpellType.Ultimate1]++;
        if ((level + 1) % 5 == 0)
            pointsToAllocate[(int)SpellType.Ultimate2]++;
    }
}
