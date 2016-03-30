using UnityEngine;
using System.Collections;

public enum ItemSlot { Weapon, OffHand, Armor, Boots, Gloves };

public class EquipableItem
{
    public int power;
    public int hp;
    public int moveSpeed;
    public Sprite sprite;
    public ItemSlot slot;
}
