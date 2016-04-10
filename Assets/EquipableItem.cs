using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EquipableItem : Item 
{
	private EquipableItemStats itemStats;
	private SpriteRenderer spriteRenderer;

	public override void isPickedUpBy (Inventory looter)
	{
		looter.addItem (itemStats);
		Destroy (gameObject);
	}

	new void Awake()
	{
        base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.enabled = false;
	}

	void Start()
	{
		itemStats = new EquipableItemStats(level);
		spriteRenderer.sprite = itemStats.sprite;
		spriteRenderer.enabled = true;
	}
}
