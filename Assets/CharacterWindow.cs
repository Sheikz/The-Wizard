using UnityEngine;
using System.Collections;

public class CharacterWindow : MonoBehaviour 
{
	private Inventory inventory;
	public Transform inventoryWindow;

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
		InventoryItemIcon newIcon = Instantiate (UIManager.instance.inventoryItemIcon);
		newIcon.initialize (itemStats);
		newIcon.transform.SetParent (inventoryWindow);
	}
}
