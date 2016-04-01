using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public float sqrDistanceLoot = 0.3f * 0.3f;
    public int goldAmount = 0;
	public EquipableItemStats[] equippedItems;
	private List<EquipableItemStats> inventoryItems;
	private CharacterWindow characterWindow;

	void Awake()
	{
		equippedItems = new EquipableItemStats[Enum.GetNames(typeof(ItemSlot)).Length];
		inventoryItems = new List<EquipableItemStats> ();
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
        return true;
	}

    public void unequipItem(int slot)
    {
        if (equippedItems[slot] == null)
            return;
        
        addItem(equippedItems[slot]);
            
        equippedItems[slot] = null;
        characterWindow.equippedItemIcons[slot].refresh(null);
    }

    public float getDamageMultiplier(MagicElement element)
    {
        float result = 1.0f;
        float multiplier = 1.0f;
        foreach (EquipableItemStats itemStats in equippedItems)
        {
            if (itemStats == null)
                continue;
            result += itemStats.power / 100f;
            multiplier += (itemStats.magicModifiers[(int)element] -1.0f);
        }
        return result * multiplier;
    }
}
