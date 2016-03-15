using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Class that launch an event
/// </summary>
[RequireComponent(typeof(Room))]
public class BossEvent : RoomEvent
{
    public MonsterSpawner spawner;

    private bool eventFinished = false;

    public override void playerEnteredRoom(PlayerController player)
    {
        if (eventFinished)
            return;

        room.openEntrance(false);
        room.openExits(false);
        spawner.spawnMonster();
    }

    public override void playerExitedRoom(PlayerController player)
    {
        room.openEntrance(true);
        spawner.clearMonsters();
    }

    public override void monsterDied(NPCController mc)
    {
        room.openEntrance(true);
        room.openExits(true);
        eventFinished = true;
    }
}
