﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour 
{
	public static ItemManager instance;
	public SpriteArray[] itemSprites;
    internal float powerToDamage = 100f;
    public List<ItemWithDropChance> monsterItems;

    [System.Serializable]
	public class SpriteArray
	{
		public Sprite[] items;
	}

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		name = "ItemManager";
	}

	public Sprite getRandomSprite(ItemSlot slot)
	{
		return Utils.pickRandom(itemSprites[(int)slot].items);
	}
}
