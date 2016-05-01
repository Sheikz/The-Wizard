using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CharacterWindow : MonoBehaviour 
{
	public Transform inventoryWindow;
    public InventoryItemIcon[] equippedItemIcons;
    public Transform statsBreakdown;
    private List<InventoryItemIcon> inventoryItems;
    private StatsInfo[] statsInfo;
    private Inventory inventory;
    private Damageable damageable;
    private List<Tooltip> tooltips;
    private MovingCharacter movingChar;

    private enum Stats { Power, MaxHP, MoveSpeed, Gold };

    public void initialize()
    {
        foreach (InventoryItemIcon icon in equippedItemIcons)
        {
            icon.initialize(null, GameManager.instance.hero.GetComponent<Inventory>());
        }
        linkText();
        tooltips = new List<Tooltip>();
    }

    void linkText()
    {
        statsInfo = new StatsInfo[Enum.GetNames(typeof(Stats)).Length];
        statsInfo[(int)Stats.Power] = statsBreakdown.Find("Power").GetComponent<StatsInfo>();
        statsInfo[(int)Stats.MaxHP] = statsBreakdown.Find("MaxHP").GetComponent<StatsInfo>();
        statsInfo[(int)Stats.MoveSpeed] = statsBreakdown.Find("MoveSpeed").GetComponent<StatsInfo>();
        statsInfo[(int)Stats.Gold] = statsBreakdown.Find("Gold").GetComponent<StatsInfo>();

    }

    public void open()
	{
		if (gameObject.activeSelf)
		{
			gameObject.SetActive(false);
			GameManager.instance.setPause(false);
            closeTooltips();
		}
		else
		{
			UIManager.instance.closeWindows();
            refresh();
			gameObject.SetActive(true);
			GameManager.instance.setPause(true);
		}
	}

    private void closeTooltips()
    {
        for (int i = tooltips.Count - 1; i >= 0; i--)
        {
            if (tooltips[i] == null)
                tooltips.RemoveAt(i);
            else
                tooltips[i].gameObject.SetActive(false);
        }
    }

    public void refresh()
    {
        if (inventory == null)
            inventory = GameManager.instance.hero.GetComponent<Inventory>();
        if (movingChar == null)
            movingChar = GameManager.instance.hero.GetComponent<MovingCharacter>();

        statsInfo[(int)Stats.Gold].refresh(inventory.goldAmount);
        statsInfo[(int)Stats.Power].refresh(inventory.getPower());
        statsInfo[(int)Stats.MoveSpeed].refresh(movingChar.movingSpeed);
        if (damageable == null)
            damageable = inventory.GetComponent<Damageable>();
        if (damageable)
            statsInfo[(int)Stats.MaxHP].refresh(damageable.maxHP);
    }

	public void addItem(EquipableItemStats itemStats)
	{
        if (inventory == null)
            inventory = GameManager.instance.hero.GetComponent<Inventory> ();
		InventoryItemIcon newIcon = Instantiate (UIManager.instance.inventoryItemIcon);
		newIcon.initialize (itemStats, inventory);
		newIcon.transform.SetParent (inventoryWindow);
        if (inventoryItems == null)
            inventoryItems = new List<InventoryItemIcon>();
        inventoryItems.Add(newIcon);
	}

    public void removeItem(EquipableItemStats itemStats)
    {
        for (int i = inventoryItems.Count - 1; i >= 0; i--)
        {
            if (inventoryItems[i].itemStats == itemStats)
            {
                Destroy(inventoryItems[i].gameObject);
                inventoryItems.RemoveAt(i);
            }
        }
    }

    public void registerTooltip(Tooltip tooltip)
    {
        tooltips.Add(tooltip);
    }
}
