using UnityEngine;
using System;
using System.Collections.Generic;

public enum ItemPerk
{
    FireSlashReflect, FireSlashMultiply, EnergyBoltAimBot
};

public static class ItemPerkExtensions
{
    public static string getDescription(this ItemPerk perk)
    {
        switch (perk)
        {
            case ItemPerk.FireSlashMultiply: return "Flaming Whip launches 3 slashes";
            case ItemPerk.FireSlashReflect: return "Flaming Whip reflects incoming projectiles";
            case ItemPerk.EnergyBoltAimBot: return "Energy Bolt auto-aims at enemies";
        }
        return "Not implemented";
    }
}

public class ItemManager : MonoBehaviour 
{
	public static ItemManager instance;
	public SpriteArray[] itemSprites;
    internal float powerToDamage = 100f;
    public List<ItemWithDropChance> monsterItems;
    public float[] rarityChance;

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
