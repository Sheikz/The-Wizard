using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public enum ItemSlot { Weapon, OffHand, Armor, Gloves, Boots };
public enum ItemStats { Power, HP, MoveSpeed, Gold };

public class EquipableItemStats
{
    public int level;
	public string name;
    public int power;
    public int hp;
    public int moveSpeed;
    public Sprite sprite;
    public ItemSlot slot;
    public float[] magicModifiers;

    public EquipableItemStats(int level)
	{
        this.level = level;
		name = "Undefined Item";
		slot = (ItemSlot)Random.Range (0, Enum.GetNames (typeof(ItemSlot)).Length);
		sprite = ItemManager.instance.getRandomSprite (slot);

        magicModifiers = new float[Enum.GetNames(typeof(MagicElement)).Length];
        for (int i = 0; i < magicModifiers.Length; i++)
            magicModifiers[i] = 1f;

        randomizeStats();
	}

    void randomizeStats()
    {
        switch (slot)
        {
            case ItemSlot.Weapon:
                setRandomPower();
                setRandomMastery();
                break;
            case ItemSlot.OffHand:
                setRandomPower();
                break;
            case ItemSlot.Armor:
                setRandomHP();
                break;
            case ItemSlot.Gloves:
                setRandomHP();
                break;
            case ItemSlot.Boots:
                moveSpeed = Random.Range(0, 3);
                break;
        }
    }

    void setRandomPower()
    {
        power = level * 10 + Mathf.RoundToInt(Random.Range(-level * 5f, level * 5f));
        power = Mathf.Clamp(power, 0, power);
    }

    void setRandomMastery()
    {
        int mastery = Random.Range(0, Enum.GetNames(typeof(MagicElement)).Length);
        float masteryValue = Utils.pickRandom(new float[]{ 0f, 10f, 15f, 20f }) / 100f;
        magicModifiers[mastery] += masteryValue;
    }

    void setRandomHP()
    {
        hp = level * 100 + Mathf.RoundToInt(Random.Range(-level * 50f, level * 50f));
        hp = Mathf.Clamp(hp, 0, hp);
    }
}
