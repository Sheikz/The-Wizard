using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class InventoryItemIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
	public EquipableItemStats itemStats;
	private Image image;
	private Tooltip tooltip;
	private Inventory inventory;	// Reference to the inventory containing this item

	void Awake()
	{
		image = GetComponent<Image>();
	}

	public void initialize(EquipableItemStats itemStats, Inventory inventory)
	{
		this.inventory = inventory;
        refresh(itemStats);
	}

    public void refresh(EquipableItemStats itemStats)
    {
        if (itemStats == null)
        {
            this.itemStats = null;
            GetComponent<Image>().sprite = UIManager.instance.emptyIcon;
            if (tooltip)
                tooltip.gameObject.SetActive(false);
            return;
        }
        this.itemStats = itemStats;
        image.sprite = itemStats.sprite;
    }

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (tooltip)
		{
			tooltip.gameObject.SetActive(true);
			tooltip.refresh(itemStats);
		}
		else
		{
			tooltip = Instantiate(UIManager.instance.tooltipPrefab);
			tooltip.transform.SetParent(transform);
			tooltip.transform.position = transform.position + new Vector3(25, -10, 0);
            UIManager.instance.characterWindow.registerTooltip(tooltip);
			tooltip.refresh(itemStats);
		}
	}
		
	public void OnPointerExit (PointerEventData eventData)
	{
		if (tooltip)
			tooltip.gameObject.SetActive(false);
	}

    /// <summary>
    /// Called when the player click an item in the inventory. Should equip it
    /// </summary>
	public void onClickInventory()
	{
        if (inventory == null)
            inventory = GameManager.instance.hero.GetComponent<Inventory>();
        
		inventory.equipItem (itemStats);
        SoundManager.instance.playSound("ClickOK");
    }

    /// <summary>
    /// Called when the player click an item equipped. Should unequip it
    /// </summary>
    public void onClickEquipped(int slot)
    {
        if (inventory == null)
            inventory = GameManager.instance.hero.GetComponent<Inventory>();

        inventory.unequipItem(slot);
        SoundManager.instance.playSound("ClickOK");
    }
}
