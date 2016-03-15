using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class RoomEvent : MonoBehaviour
{
    public abstract void playerEnteredRoom(PlayerController player);
    public abstract void playerExitedRoom(PlayerController player);
    public abstract void monsterDied(NPCController mc);

    protected Room room;

    void Awake()
    {
        room = GetComponent<Room>();
    }
}
