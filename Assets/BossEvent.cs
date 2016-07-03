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
        SoundManager.instance.playBossMusic();
    }

    public override void playerExitedRoom(PlayerController player)
    {
        startEvent();
        room.openEntrance(true);
        spawner.clearMonsters();
        SoundManager.instance.playDungeonMusic();
    }

    public override void monsterDied(NPCController mc)
    {
        endEvent();
        room.openEntrance(true);
        room.openExits(true);
        eventFinished = true;
        SoundManager.instance.playDungeonMusic();
    }
}
