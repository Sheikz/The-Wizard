using UnityEngine;
using System.Collections;
using System;

public class Inventory : MonoBehaviour
{
    public float sqrDistanceLoot = 0.3f * 0.3f;
    public int goldAmount = 0;

    internal void addGold(int amount)
    {
        goldAmount += amount;
    }
}
