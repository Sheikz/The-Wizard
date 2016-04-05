using UnityEngine;
using System.Collections.Generic;
using System;

public class MonsterEvent : RoomEvent
{
    public List<ItemWithDropChance> monsterPrefabs;
    public int monsterNumber = 2;

    private List<NPCController> monsters;

    new void Awake()
    {
        base.Awake();
    }

    public override void monsterDied(NPCController mc)
    {
        return;
    }

    public override void initialize()
    {
        base.initialize();
        createMonsters();
    }

    private void createMonsters()
    {
        monsters = new List<NPCController>();
        if (roomTiles.Count <= 0)
        {
            Debug.LogError("Not a single tile to put monsters in room " + name);
            return;
        }
        for (int i = 0; i < monsterNumber; i++)
        {
            Tile tileToPutMonster = Utils.pickRandom(roomTiles);
            NPCController newMonsterPrefab = ItemWithDropChance.getItem(monsterPrefabs).item.GetComponent<NPCController>();
            if (tileToPutMonster.getDistanceToClosest(newMonsterPrefab.isFlying) <= newMonsterPrefab.getRadius())   // Check if there is enough room to put the monster
                continue;
            NPCController newMonster = Instantiate(newMonsterPrefab, tileToPutMonster.position(), Quaternion.identity) as NPCController;
            newMonster.transform.SetParent(room.map.monsterHolder);
            newMonster.initialize(room.map);
            newMonster.gameObject.SetActive(false);
            monsters.Add(newMonster);
        }
    }

    public override void playerEnteredRoom(PlayerController player)
    {
        foreach (NPCController monster in monsters)
        {
            monster.gameObject.SetActive(true);
        }
    }

    public override void playerExitedRoom(PlayerController player)
    {
        throw new NotImplementedException();
    }
}
