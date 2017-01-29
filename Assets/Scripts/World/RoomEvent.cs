using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class RoomEvent : MonoBehaviour
{
    public GameObject[] spawnAfterEvent;
    public abstract void playerEnteredRoom(PlayerController player);
    public abstract void playerExitedRoom(PlayerController player);
    public abstract void monsterDied(NPCController mc);

    protected Room room;
    protected List<Tile> roomTiles;
    protected BoxCollider2D[] roomBounds;
    protected bool eventFinished = false;

    protected void Awake()
    {
        room = GetComponent<Room>();
    }

    protected void Start()
    {
        foreach (GameObject obj in spawnAfterEvent)
        {
            obj.SetActive(false);
        }
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

    /// <summary>
    /// Remove the tiles which are not practicable
    /// </summary>
    protected void filterRoomTiles()
    {
        for (int i = roomTiles.Count - 1; i >= 0; i--)
        {
            if ((roomTiles[i].type != TileType.Floor) ||
                roomTiles[i].getDistanceToClosest(true) <= 0.1f)
            {
                roomTiles.RemoveAt(i);
                continue;
            }
        }
    }

    protected void startEvent()
    {
        room.openEntrance(false);
        room.openExits(false);
    }

    protected void endEvent(bool spawn = true)
    {
        eventFinished = true;
        room.openEntrance(true);
        room.openExits(true);
        if (spawn)
        {
            foreach (GameObject obj in spawnAfterEvent)
            {
                obj.SetActive(true);
            }
        }
    }
}
