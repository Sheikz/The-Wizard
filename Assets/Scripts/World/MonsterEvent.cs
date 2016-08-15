using UnityEngine;
using System.Collections.Generic;
using System;

public enum RoomSize { Small, Medium, Large, UltraLarge };

public class MonsterEvent : RoomEvent
{
    public RoomSize roomSize;

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
        filterRoomTiles();
        var monstersToPut = WorldManager.instance.getMonsters(this);
        foreach (NPCController monsterPrefab in monstersToPut)
        {
            Tile tileToPutMonster = Utils.pickRandom(roomTiles);
            //if (tileToPutMonster.getDistanceToClosest(monsterPrefab.isFlying) <= monsterPrefab.getRadius())   // Check if there is enough room to put the monster
            //    continue;
            NPCController newMonster = Instantiate(monsterPrefab, tileToPutMonster.position(), Quaternion.identity) as NPCController;
            newMonster.transform.SetParent(room.map.monsterHolder);
            newMonster.initialize(room.map);
            newMonster.activate(false);
            monsters.Add(newMonster);
        }
    }

    /// <summary>
    /// Remove the tiles which are not practicable
    /// </summary>
    void filterRoomTiles()
    {
        for (int i= roomTiles.Count -1; i >= 0; i--)
        {
            if ((roomTiles[i].type != TileType.Floor) ||
                roomTiles[i].getDistanceToClosest(true) <= 0.1f)
            {
                roomTiles.RemoveAt(i);
                continue;
            }
        }
    }

    public override void playerEnteredRoom(PlayerController player)
    {
        foreach (NPCController monster in monsters)
        {
            if (monster)
                monster.activate(true);
        }
    }

    public override void playerExitedRoom(PlayerController player)
    {
        return;
    }
}
