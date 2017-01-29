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



    public override void playerEnteredRoom(PlayerController player)
    {
        if (eventFinished)
            return;

        startEvent();
        spawner.spawnMonster();
        SoundManager.instance.playBossMusic();
    }

    public override void playerExitedRoom(PlayerController player)
    {
        room.openEntrance(true);
        spawner.clearMonsters();
        SoundManager.instance.playDungeonMusic();
    }

    public override void monsterDied(NPCController mc)
    {
        endEvent();
        eventFinished = true;
        SoundManager.instance.playDungeonMusic();
    }
}
