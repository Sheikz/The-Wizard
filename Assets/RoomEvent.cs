using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class RoomEvent : MonoBehaviour
{
    public abstract void playerEnteredRoom(PlayerController player);
    public abstract void playerExitedRoom(PlayerController player);
    public abstract void monsterDied(NPCController mc);

    protected Room room;
    protected List<Tile> roomTiles;
    protected BoxCollider2D[] roomBounds;

    protected void Awake()
    {
        room = GetComponent<Room>();
    }

    virtual public void initialize()
    {
        room = GetComponent<Room>();
        roomTiles = new List<Tile>();
        if (room.map == null)
        {
            Debug.LogError("Room not found");
        }
        foreach (Tile tile in room.map.floorTiles)
        {
            foreach (BoxCollider2D box2d in room.getEdges())
            {
                if (box2d.bounds.Contains(tile.position()))
                    roomTiles.Add(tile);
            }
        }
    }
}
