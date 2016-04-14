using UnityEngine;
using System.Collections;

public class GoldPile : Item
{
    public int amount;

    void Start()
    {
        amount = (100 * level) + Random.Range(-75 * level, 75 * level);
    }

    public override void isPickedUpBy(Inventory looter)
    {
        looter.addGold(amount);
        SoundManager.instance.playSound("GetTreasure");
        Destroy(gameObject);
    }
}
