using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public Pillar pillar;
    public Pillar pillarWithTorch;
    public List<ItemWithDropChance> monsters;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "WorldManager";
    }

    internal List<NPCController> getMonsters(MonsterEvent monsterEvent)
    {
        List<NPCController> result = new List<NPCController>();
        int monsterNumbers = 0;
        switch (monsterEvent.roomSize)
        {
            case RoomSize.Small: monsterNumbers = 2; break;
            case RoomSize.Medium: monsterNumbers = 3; break;
            case RoomSize.Large: monsterNumbers = 4; break;
        }
        List<ItemWithDropChance> duplicatedList = new List<ItemWithDropChance>(monsters);
        for (int i= 0; i < monsterNumbers; i++)
        {
            ItemWithDropChance chosenItem = ItemWithDropChance.getItem(duplicatedList);
            duplicatedList.Remove(chosenItem);
            result.Add(chosenItem.item.GetComponent<NPCController>());
        }
        return result;
    }
}
