﻿using UnityEngine;
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

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.enabled = false;
	}

	void Start()
	{
		itemStats = new EquipableItemStats ();
		spriteRenderer.sprite = itemStats.sprite;
		spriteRenderer.enabled = true;
	}
}
