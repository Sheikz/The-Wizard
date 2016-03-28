using UnityEngine;
using System.Collections;
using System;

public class GoldPile : Item
{
    public int amount;

    public override void isPickedUpBy(Inventory looter)
    {
        looter.addGold(amount);
        Destroy(gameObject);
    }
}
