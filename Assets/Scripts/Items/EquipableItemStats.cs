using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public enum ItemSlot { Weapon, OffHand, Armor, Gloves, Boots };
public enum InventoryStats { Power, HP, MoveSpeed, CriticalStrikeChance, EnergyRegen, Gold };
public enum ItemRarity { Common, Rare, Epic, Legendary };

public class EquipableItemStats
{
    public int level;
	public string name;
    public int power;
    public int hp;
    public int energyRegen;
    [Tooltip("Critical Strike chance in percents")]
    public int criticalStrikeChance;
    public int moveSpeed;
    public Sprite sprite;
    public ItemSlot slot;
    public ItemRarity rarity;
    public float[] magicModifiers;
    public List<ItemPerk> itemPerks;

    public EquipableItemStats(int level)
	{
        this.level = level;
		name = "Undefined Item";
		slot = (ItemSlot)Random.Range (0, Enum.GetNames (typeof(ItemSlot)).Length);
		sprite = ItemManager.instance.getRandomSprite (slot);
        itemPerks = new List<ItemPerk>();

        magicModifiers = new float[Enum.GetNames(typeof(MagicElement)).Length];
        for (int i = 0; i < magicModifiers.Length; i++)
            magicModifiers[i] = 1f;

        randomizeStats();
	}

    void randomizeStats()
    {
        randomizeRarity();

        switch (slot)
        {
            case ItemSlot.Weapon:
                setRandomPower();
                setRandomMastery();
                addPerks();
                name = "Wand";
                break;
            case ItemSlot.OffHand:
                setRandomPower();
                addPerks();
                addManaRegen();
                name = "Book";
                break;
            case ItemSlot.Armor:
                setRandomHP();
                name = "Armor";
                addManaRegen();
                addPerks();
                break;
            case ItemSlot.Gloves:
                setRandomHP();
                name = "Gloves";
                addPerks();
                addCriticalChance();
                break;
            case ItemSlot.Boots:
                name = "Boots";
                moveSpeed = Random.Range(1, 3);
                setRandomHP();
                addPerks();
                break;
        }
    }

    private void addCriticalChance()
    {
        int[] lowerBound = { 0, 0, 3, 5 };
        int[] higherBound = { 3, 5, 5, 10 };
        criticalStrikeChance = Random.Range(lowerBound[(int)rarity], higherBound[(int)rarity] + 1);
    }

    private void addManaRegen()
    {
        int[] lowerBound = { 0, 1, 2, 3 };
        int[] higherBound = { 2, 3, 4, 5 };
        energyRegen = Random.Range(lowerBound[(int)rarity], higherBound[(int)rarity]+1);
    }

    private void randomizeRarity()
    {
        rarity = (ItemRarity)Utils.pickRandomIndexWithDifferentChances(ItemManager.instance.rarityChance);
    }

    void setRandomPower()
    {
        float lowerBound = -5f;
        float higherBound = 5f;
        switch (rarity)
        {
            case ItemRarity.Common: lowerBound = -8f; higherBound = -3f; break;
            case ItemRarity.Rare: lowerBound = -5f; higherBound = 0f; break;
            case ItemRarity.Epic: lowerBound = -2f; higherBound = 3f; break;
            case ItemRarity.Legendary: lowerBound = 0f; higherBound = 5f; break;
        }
        power = level * 10 + Mathf.RoundToInt(Random.Range(level * lowerBound, level * higherBound));
        power = Mathf.Clamp(power, 0, power);
    }

    void setRandomMastery()
    {
        // Mastery only for Epic and Legendaries
        if (rarity == ItemRarity.Common || rarity == ItemRarity.Rare)
            return;

        int mastery = Random.Range(0, Enum.GetNames(typeof(MagicElement)).Length);
        float masteryValue;
        if (rarity == ItemRarity.Epic)
            masteryValue = Utils.pickRandom(new float[]{ 0f, 10f, 15f, 20f }) / 100f;
        else
            masteryValue = Utils.pickRandom(new float[] {20f, 25f, 30f }) / 100f;

        magicModifiers[mastery] += masteryValue;
    }

    void setRandomHP()
    {
        float lowerBound = -50f;
        float higherBound = 50f;
        switch (rarity)
        {
            case ItemRarity.Common: lowerBound = -90f; higherBound = -45f; break;
            case ItemRarity.Rare: lowerBound = -75f; higherBound = -25; break;
            case ItemRarity.Epic: lowerBound = -40f; higherBound = 20f; break;
            case ItemRarity.Legendary: lowerBound = -25f; higherBound = 50f; break;
        }

        hp = level * 100 + Mathf.RoundToInt(Random.Range(level * lowerBound, level * higherBound));
        hp = Mathf.Clamp(hp, 0, hp);
    }

    void addPerks()
    {
        switch (rarity)
        {
            case ItemRarity.Epic: addRandomPerk(1); break;
            case ItemRarity.Legendary: addRandomPerk(2); break;
        }
    }

    /// <summary>
    /// Add perks to the item from the pool of available perks. A perk an be chosen only once
    /// </summary>
    /// <param name="number">Number of perks to add</param>
    void addRandomPerk(int number)
    {
        List<ItemPerk> availablePerks = Enum.GetValues(typeof(ItemPerk)).OfType<ItemPerk>().ToList();
        for (int i = 0; i < number; i ++)
        {
            if (availablePerks.Count == 0)
                break;

            ItemPerk toAdd = availablePerks[Random.Range(0, availablePerks.Count)];
            itemPerks.Add(toAdd);
            availablePerks.Remove(toAdd);
        }
    }
}
