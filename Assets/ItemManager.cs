using UnityEngine;
using System;
using System.Collections.Generic;

public enum ItemPerk
{
    FireSlashReflect, FireSlashMultiply, EnergyBoltAimBot,
    IceShardMultiply, SparkBounce, DragonBreathMove,
    IceBeamMove, SanctuaryHeal, SanctuaryDamage,
    DoomrangHeal, DoomrangDamageTwice
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
            case ItemPerk.IceShardMultiply: return "Ice Shard launches 3 shards in an arc";
            case ItemPerk.SparkBounce: return "Spark bounces 2 times between enemies";
            case ItemPerk.IceBeamMove: return "Can move while casting "+ perk.getSpellName();
            case ItemPerk.DragonBreathMove: return "Can move while casting "+perk.getSpellName();
            case ItemPerk.SanctuaryDamage: return perk.getSpellName()+" does damage to enemies inside the circle";
            case ItemPerk.SanctuaryHeal: return perk.getSpellName() + " heal allies inside the circle";
            case ItemPerk.DoomrangHeal: return perk.getSpellName() + " heals for 5% of damage done";
            case ItemPerk.DoomrangDamageTwice: return perk.getSpellName() + " damages enemies when going and returning";
        }
        return "Not implemented";
    }

    public static string getSpellName(this ItemPerk perk)
    {
        switch (perk)
        {
            case ItemPerk.FireSlashMultiply: return "Flaming Whip";
            case ItemPerk.FireSlashReflect: return "Flaming Whip";
            case ItemPerk.EnergyBoltAimBot: return "Energy Bolt";
            case ItemPerk.IceShardMultiply: return "Ice Shard";
            case ItemPerk.SparkBounce: return "Spark";
            case ItemPerk.IceBeamMove: return "Ice Beam";
            case ItemPerk.DragonBreathMove: return "Dragon Breath";
            case ItemPerk.SanctuaryDamage: return "Sanctuary";
            case ItemPerk.SanctuaryHeal: return "Sanctuary";
            case ItemPerk.DoomrangHeal: return "Doomrang";
            case ItemPerk.DoomrangDamageTwice: return "Doomrang";
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
