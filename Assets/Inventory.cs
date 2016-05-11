using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public float sqrDistanceLoot = 0.3f * 0.3f;
    public int goldAmount = 0;
    public EquipableItemStats[] equippedItems;
    public float vacuumSpeed = 5f;
    private List<EquipableItemStats> inventoryItems;
    private CharacterWindow characterWindow;
    private CharacterStats characterStats;
    private MovingCharacter movingChar;

    private float[] elementMultiplier;
    public bool[] hasItemPerk;

	void Awake()
	{
        characterStats = GetComponent<CharacterStats>();
		equippedItems = new EquipableItemStats[Enum.GetNames(typeof(ItemSlot)).Length];
		inventoryItems = new List<EquipableItemStats> ();
        elementMultiplier = new float[Enum.GetNames(typeof(MagicElement)).Length];
        hasItemPerk = new bool[Enum.GetValues(typeof(ItemPerk)).Length];
        for (int i = 0; i < elementMultiplier.Length; i++)
            elementMultiplier[i] = 1.0f;
        for (int i = 0; i < hasItemPerk.Length; i++)
            hasItemPerk[i] = false;
	}

	void Start()
	{
		characterWindow = UIManager.instance.characterWindow;
	}

    internal void addGold(int amount)
    {
        goldAmount += amount;
    }

	public void addItem(EquipableItemStats itemStats)
	{
		characterWindow.addItem (itemStats);
		inventoryItems.Add (itemStats);
	}

	public bool equipItem(EquipableItemStats itemStats)
	{
        if (!inventoryItems.Contains(itemStats))
            return false;
        
	    characterWindow.removeItem (itemStats);

        if (equippedItems[(int)itemStats.slot] != null)
            addItem(equippedItems[(int)itemStats.slot]);

        equippedItems[(int)itemStats.slot] = itemStats;
        characterWindow.equippedItemIcons[(int)itemStats.slot].refresh(itemStats);
        refreshStats();
        return true;
	}

    public void unequipItem(int slot)
    {
        if (equippedItems[slot] == null)
            return;
        
        addItem(equippedItems[slot]);
            
        equippedItems[slot] = null;
        characterWindow.equippedItemIcons[slot].refresh(null);
        refreshStats();
    }

    /// <summary>
    /// Refresh the pre-computed stats
    /// </summary>
    void refreshStats()
    {
        refreshMultipliers();
        refreshHP();
        refreshMoveSpeed();
        refreshItemPerks();
        characterWindow.refresh();
    }

    private void refreshItemPerks()
    {
        for (int i = 0; i < hasItemPerk.Length; i++)
            hasItemPerk[i] = false;
        foreach (EquipableItemStats itemStats in equippedItems)
        {
            if (itemStats == null)
                continue;
            foreach (ItemPerk perk in itemStats.itemPerks)
                hasItemPerk[(int)perk] = true;
        }
    }

    public void refreshMoveSpeed()
    {
        if (!movingChar)
            movingChar = GameManager.instance.hero.GetComponent<MovingCharacter>();

        movingChar.speed = movingChar.baseSpeed + getMoveSpeed();
        PlayerStats pStats = characterStats.GetComponent<PlayerStats>();
        if (pStats)
            movingChar.speed += pStats.speedSkillBonus;

    }

    void refreshHP()
    {
        characterStats.refreshHP(false);
    }

    public int getAdditionalHPFromItems()
    {
        int addHP = 0;
        foreach (EquipableItemStats itemStats in equippedItems)
        {
            if (itemStats == null)
                continue;
            addHP += itemStats.hp;
        }
        return addHP;
    }

    public int getPower()
    {
        int power = 0;
        foreach (EquipableItemStats itemStats in equippedItems)
        {
            if (itemStats == null)
                continue;
            power += itemStats.power;
        }
        return power;
    }

    public int getMoveSpeed()
    {
        int moveSpeed = 0;
        foreach (EquipableItemStats itemStats in equippedItems)
        {
            if (itemStats == null)
                continue;
            moveSpeed += itemStats.moveSpeed;
        }
        return moveSpeed;
    }

    /// <summary>
    /// Refresh the damage multipliers according to power and %elemental damage
    /// </summary>
    void refreshMultipliers()
    {
        for (int i = 0; i < elementMultiplier.Length; i++)
        {
            float result = 1.0f;
            float multiplier = 1.0f;
            foreach (EquipableItemStats itemStats in equippedItems)
            {
                if (itemStats == null)
                    continue;
                result += itemStats.power / ItemManager.instance.powerToDamage;
                multiplier += (itemStats.magicModifiers[i] - 1.0f);
            }
            elementMultiplier[i] = result * multiplier;
        }
    }

    internal bool getItemPerk(ItemPerk itemPerk)
    {
        return hasItemPerk[(int)itemPerk];
    }

    public float getDamageMultiplier(MagicElement element)
    {
        return elementMultiplier[(int)element];
    }
}
