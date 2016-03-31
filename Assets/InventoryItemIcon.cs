using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class InventoryItemIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private EquipableItemStats itemStats;
	private Image image;
	private Tooltip tooltip;

	void Awake()
	{
		image = GetComponent<Image>();
	}

	public void initialize(EquipableItemStats itemStats)
	{
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
			tooltip.refresh(itemStats);
		}
	}
		
	public void OnPointerExit (PointerEventData eventData)
	{
		if (tooltip)
			tooltip.gameObject.SetActive(false);
	}
}
