using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomItem : Item
{
    public List<ItemWithDropChance> items;
    private GameObject itemToDrop;
    private CharacterStats looterStats;

    public override void isPickedUpBy(Inventory looter)
    {
        return;
    }

    public override void initialize(CharacterStats stats)
    {
        looterStats = stats;
    }

    void dropItem()
    {
        itemToDrop = ItemWithDropChance.getItem(items).item;
        Item newItem = (Instantiate(itemToDrop, transform.position, Quaternion.identity) as GameObject).GetComponent<Item>();
        newItem.transform.SetParent(GameManager.instance.map.transform);
        if (looterStats)
            newItem.initialize(looterStats);
    }

    new void Start()
    {
        dropItem();
        Destroy(gameObject);
    }
}
