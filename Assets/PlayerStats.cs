using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerStats : CharacterStats
{
    protected float[] masteries;
    public int[,,] spellLevels; // In order: set, element, type
    public int[] pointsToAllocate;



    private int numberOfTypes;
    private int numberOfElements;
    private int numberOfSets;
    private SpellCaster hero;
    [HideInInspector]
    public float HPSkillBonus = 1.0f;
    public float speedSkillBonus = 0f;
    private Inventory inventory;

    new void Awake()
    {
        base.Awake();
        numberOfElements = Enum.GetNames(typeof(MagicElement)).Length;
        numberOfTypes = Enum.GetNames(typeof(SpellType)).Length;
        numberOfSets = Enum.GetNames(typeof(SpellSet)).Length;
        masteries = new float[numberOfElements];
        inventory = GetComponent<Inventory>();
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
        return getMastery(school) * getDamageMultiplier();
    }

    public void multiplyMastery(float value, MagicElement magicSchool)
    {
        masteries[(int)magicSchool] *= value;
    }

    internal void addSpeed(float additionalSpeed)
    {
        speedSkillBonus += additionalSpeed;
        Inventory inv = GetComponent<Inventory>();
        if (inv)
            inv.refreshMoveSpeed();

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
        UIManager.instance.refreshUI();
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
        if (level != 1 && (level - 1) % 5 == 0)
            pointsToAllocate[(int)SpellType.Ultimate2]++;

        UIManager.instance.spellWindowByType.refresh();
        UIManager.instance.refreshUI();
        SoundManager.instance.playSound("LevelUp");
    }

    public int getTotalToAllocate()
    {
        int result = 0;
        for (int i = 0; i < pointsToAllocate.Length; i++)
        {
            result += pointsToAllocate[i];
        }
        return result;
    }

    public override void refreshHP(bool updateCurrentHP)
    {
        if (!damageable)
            return;

        float mult = 1.0f;
        int hpFromItems = 0;
        mult *= getHPMultiplier();
        mult *= HPSkillBonus;
        
        if (inventory)
            hpFromItems = inventory.getAdditionalHPFromItems();

        damageable.multiplyBaseHP(mult, hpFromItems, updateCurrentHP);
    }
}
