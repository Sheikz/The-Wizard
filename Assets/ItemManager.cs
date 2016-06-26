using UnityEngine;
using System;
using System.Collections.Generic;

public enum ItemPerk
{
    //FireSlashReflect, FireSlashMultiply,
    FireBallUpgrade, EnergyBoltAimBot,
    EnergyBoltInstant, IceSlideNoCD,
    IceShardMultiply, SparkBounce, DragonBreathMove,
    IceBeamMove, SanctuaryHeal, SanctuaryDamage,
    DoomrangHeal, DoomrangDamageTwice,
    ChronosphereDamage
};

public static class ItemPerkExtensions
{
    public static string getDescription(this ItemPerk perk)
    {
        switch (perk)
        {
            //case ItemPerk.FireSlashMultiply: return "Flaming Whip launches 3 slashes";
            //case ItemPerk.FireSlashReflect: return "Flaming Whip reflects incoming projectiles";
            case ItemPerk.FireBallUpgrade: return perk.getSpellName() + " is bigger and does 100% more damage";
            case ItemPerk.EnergyBoltAimBot: return "Energy Bolt auto-aims at enemies";
            case ItemPerk.EnergyBoltInstant: return perk.getSpellName()+" has no cast time";
            case ItemPerk.IceShardMultiply: return "Ice Shard launches 3 shards in an arc";
            case ItemPerk.IceSlideNoCD: return perk.getSpellName() + " has no cooldown";
            case ItemPerk.SparkBounce: return "Spark bounces 2 times between enemies";
            case ItemPerk.IceBeamMove: return "Can move while casting "+ perk.getSpellName();
            case ItemPerk.DragonBreathMove: return "Can move while casting "+perk.getSpellName();
            case ItemPerk.SanctuaryDamage: return perk.getSpellName()+" does damage to enemies inside the circle";
            case ItemPerk.SanctuaryHeal: return perk.getSpellName() + " heal allies inside the circle";
            case ItemPerk.DoomrangHeal: return perk.getSpellName() + " heals for 5% of damage done";
            case ItemPerk.DoomrangDamageTwice: return perk.getSpellName() + " damages enemies when going and returning";
            case ItemPerk.ChronosphereDamage: return perk.getSpellName() + " damage enemies inside";
        }
        return "Not implemented";
    }

    public static string getSpellName(this ItemPerk perk)
    {
        switch (perk)
        {
            //case ItemPerk.FireSlashMultiply: return "Flaming Whip";
            //case ItemPerk.FireSlashReflect: return "Flaming Whip";
            case ItemPerk.FireBallUpgrade: return "Fire Ball";
            case ItemPerk.EnergyBoltAimBot: return "Energy Bolt";
            case ItemPerk.EnergyBoltInstant: return "Energy Bolt";  // TODO
            case ItemPerk.IceSlideNoCD: return "Ice Slide";
            case ItemPerk.IceShardMultiply: return "Ice Shard";
            case ItemPerk.SparkBounce: return "Spark";
            case ItemPerk.IceBeamMove: return "Ice Beam";
            case ItemPerk.DragonBreathMove: return "Dragon Breath";
            case ItemPerk.SanctuaryDamage: return "Sanctuary";
            case ItemPerk.SanctuaryHeal: return "Sanctuary";
            case ItemPerk.DoomrangHeal: return "Doomrang";
            case ItemPerk.DoomrangDamageTwice: return "Doomrang";
            case ItemPerk.ChronosphereDamage: return "ChronoSphere";
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
    public GameObject[] itemAuras;

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
