using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public float sqrDistanceLoot = 0.3f * 0.3f;
    public int goldAmount = 0;
	public EquipableItemStats[] equippedItems;
	private List<EquipableItemStats> inventoryItems;

	void Awake()
	{
		equippedItems = new EquipableItemStats[Enum.GetNames(typeof(ItemSlot)).Length];
		inventoryItems = new List<EquipableItemStats> ();
	}

    internal void addGold(int amount)
    {
        goldAmount += amount;
    }

	public void addItem(EquipableItemStats itemStats)
	{
		UIManager.instance.characterWindow.addItem (itemStats);
	}
}
