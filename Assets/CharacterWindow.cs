using UnityEngine;
using System.Collections.Generic;

public class CharacterWindow : MonoBehaviour 
{
	private Inventory inventory;
	public Transform inventoryWindow;
    public InventoryItemIcon[] equippedItemIcons;
    private List<InventoryItemIcon> inventoryItems;

    public void initialize()
    {
        foreach (InventoryItemIcon icon in equippedItemIcons)
        {
            Debug.Log("init!");
            icon.initialize(null, GameManager.instance.hero.GetComponent<Inventory>());
        }
    }

	public void open()
	{
		if (gameObject.activeSelf)
		{
			gameObject.SetActive(false);
			GameManager.instance.setPause(false);
		}
		else
		{
			UIManager.instance.closeWindows();
			gameObject.SetActive(true);
			GameManager.instance.setPause(true);
		}
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
}
