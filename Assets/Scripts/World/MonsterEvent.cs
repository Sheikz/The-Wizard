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
        monsters.Remove(mc);
        if (monsters.Count == 0)
            endEvent();
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
        int monsterNumbers = 0;
        switch (roomSize)
        {
            case RoomSize.Small: monsterNumbers = 2; break;
            case RoomSize.Medium: monsterNumbers = 3; break;
            case RoomSize.Large: monsterNumbers = 4; break;
            case RoomSize.UltraLarge: monsterNumbers = 8; break;
        }
        var monstersToPut = WorldManager.instance.getMonsters(monsterNumbers);
        foreach (NPCController monsterPrefab in monstersToPut)
        {
            Tile tileToPutMonster = Utils.pickRandom(roomTiles);
            //if (tileToPutMonster.getDistanceToClosest(monsterPrefab.isFlying) <= monsterPrefab.getRadius())   // Check if there is enough room to put the monster
            //    continue;
            NPCController newMonster = Instantiate(monsterPrefab, tileToPutMonster.position(), Quaternion.identity) as NPCController;
            newMonster.transform.SetParent(room.map.monsterHolder);
            newMonster.initialize(room);
            newMonster.activate(false);
            monsters.Add(newMonster);
        }
    }

    public override void playerEnteredRoom(PlayerController player)
    {
        if (eventFinished)
            return;

        startEvent();
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
