using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public enum ItemSlot { Weapon, OffHand, Armor, Boots, Gloves };

public class EquipableItemStats
{
	public string name;
    public int power;
    public int hp;
    public int moveSpeed;
    public Sprite sprite;
    public ItemSlot slot;

	public EquipableItemStats()
	{
		name = "Item";
		power = Random.Range (0, 50);
		hp = Random.Range (0, 200);
		moveSpeed = Random.Range (0, 2);
		slot = (ItemSlot)Random.Range (0, Enum.GetNames (typeof(ItemSlot)).Length);
		sprite = ItemManager.instance.getRandomSprite (slot);
	}
}
