﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour, IComparable<Room>
{
    public WallCreator entranceWall;
    public WallCreator[] exitWalls;
    [Tooltip("Exterior bounds of the room")]
    public GameObject roomEdge;
    [Tooltip("Indicates if the room can be merged with another")]
    public bool canBeMergedAtEntrance = true;
    public bool canBeMergedAtExit = true;
    public bool hasCarpet = false;
    public bool hasPillars = false;
    public List<GameObject> pillars;
    public List<GameObject> carpets;

    [HideInInspector]
    public int roomDepth;
    [HideInInspector]
    public VisibleRoom visibleRoom;
    private RoomEvent[] roomEvents;
    [HideInInspector]
    public RoomBasedMapGenerator map;
    public int depth = 0;
    private MeshCreator[] meshCreators;
    public Material prefabMaterial;

    void OnEnable()
    {
        visibleRoom = roomEdge.GetComponent<VisibleRoom>();
        roomEvents = GetComponents<RoomEvent>();
        meshCreators = GetComponentsInChildren<MeshCreator>();
    }

    void Start()
    {
        map = GetComponentInParent<RoomBasedMapGenerator>();
        if (map == null)
            Debug.Log("map not found");
    }

    public BoxCollider2D[] getEdges()
    {
        return roomEdge.GetComponents<BoxCollider2D>();
    }

    public void refreshWalls()
    {
        foreach (WallCreator wc in exitWalls)
        {
            wc.refreshContents();
        }
        if (meshCreators == null)
            OnEnable();
        foreach(MeshCreator meshCreator in meshCreators)
        {
            meshCreator.BuildMesh();
        }
        foreach (GameObject obj in carpets)
            obj.SetActive(hasCarpet);
        foreach (GameObject obj in pillars)
            obj.SetActive(hasPillars);

        //combineMeshes();
    }

    public void randomize()
    {
        hasCarpet = Utils.randomBool();
        hasPillars = Utils.randomBool();
    }

    // TODO: Need to fix this
    void combineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters == null)
            return;
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        MeshFilter newMeshFilter = gameObject.AddComponent<MeshFilter>();
        newMeshFilter.mesh = new Mesh();
        newMeshFilter.mesh.CombineMeshes(combine);
        MeshRenderer newMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        newMeshRenderer.material = prefabMaterial;
        transform.gameObject.SetActive(true);
    }

    public void setMerged(bool merged)
    {
        if (merged)
            entranceWall.doorStatus = WallCreator.DoorStatus.Open;
        else
            entranceWall.doorStatus = WallCreator.DoorStatus.Door;
        entranceWall.refreshContents();
    }

    public void setVisible(bool v)
    {
        visibleRoom.setVisible(v);
    }

    /// <summary>
    /// Event coming from the RoomBounds stating that the player entered the room
    /// </summary>
    /// <param name="player"></param>
    public void playerEnteredRoom(PlayerController player)
    {
        foreach (RoomEvent ev in roomEvents)
        {
            ev.playerEnteredRoom(player);
        }
    }

    /// <summary>
    /// Event coming from the RoomBounds stating that the player exited the room
    /// </summary>
    /// <param name="player"></param>
    public void playerExitedRoom(PlayerController player)
    {
        foreach (RoomEvent ev in roomEvents)
        {
            ev.playerExitedRoom(player);
        }
    }

    public void openEntrance(bool open)
    {
        if (entranceWall && entranceWall.door)
        {
            if (open)
                entranceWall.door.openDoor();
            else
                entranceWall.door.closeDoor();
        }
    }

    public void openExits(bool open)
    {
        foreach (WallCreator exit in exitWalls)
        {
            if (exit.door)
            {
                if (open)
                    exit.door.openDoor();
                else
                    exit.door.closeDoor();
            }
        }
    }

    public void monsterDied(NPCController mc)
    {
        foreach (RoomEvent ev in roomEvents)
        {
            ev.monsterDied(mc);
        }
    }

    /// <summary>
    /// Compare to other room depth (descending order)
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <returns></returns>
    public int CompareTo(Room otherRoom)
    {
        return otherRoom.depth - depth;
    }
}