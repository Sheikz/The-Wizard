using UnityEngine;
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
    public bool randomized = true;
    public bool hasCarpet = false;
    public bool hasPillars = false;
    public List<GameObject> carpets;
    public GameObject[] randomElements;
    public static float timeSpentRefreshingRoom = 0;
    public static int refreshRoomCount = 0;

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
    private List<Room> linkedRooms; // Room that have been merged with this one (events should shoot simultaneously)

    void OnEnable()
    {
        visibleRoom = roomEdge.GetComponent<VisibleRoom>();
        roomEvents = GetComponents<RoomEvent>();
        meshCreators = GetComponentsInChildren<MeshCreator>();
        linkedRooms = new List<Room>();
    }

    void Start()
    {
        map = GetComponentInParent<RoomBasedMapGenerator>();
        if (map == null)
            Debug.Log("Room: "+name+": map not found");
    }

    public BoxCollider2D[] getEdges()
    {
        return roomEdge.GetComponents<BoxCollider2D>();
    }

    public void refresh()
    {
        randomize();
        float now = Time.realtimeSinceStartup;
        Pillar[] pillars = GetComponentsInChildren<Pillar>();
        foreach (WallCreator wc in GetComponentsInChildren<WallCreator>())
        {
            wc.objectType = WallCreator.ObjectType.Mesh;
            wc.initializeContents();
        }

        OnEnable();
        foreach(MeshCreator meshCreator in meshCreators)
        {
            meshCreator.BuildMesh();
        }
        foreach (GameObject obj in carpets)
            obj.SetActive(hasCarpet);
        foreach (Pillar pillar in pillars)
            pillar.initialize(hasPillars, this);

        foreach (AdjustTheme adjustTheme in GetComponentsInChildren<AdjustTheme>())
        {
            adjustTheme.refresh();
        }

        foreach (RandomFurniture furniture in GetComponentsInChildren<RandomFurniture>())
        {
            furniture.instantiate();
        }

        timeSpentRefreshingRoom += (Time.realtimeSinceStartup - now);
        refreshRoomCount++;
        //combineMeshes();
    }

    internal void revealContents()
    {
        foreach (VisibleUnit unit in GetComponentsInChildren<VisibleUnit>())
        {
            unit.setVisible();
        }
    }

    public void randomize()
    {
        if (!randomized)
            return;

        hasCarpet = Utils.randomBool();
        hasPillars = Utils.randomBool();
        foreach (GameObject obj in randomElements)
        {
            obj.SetActive(Utils.randomBool());
        }
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

        entranceWall.objectType = WallCreator.ObjectType.Mesh;
        entranceWall.refreshContents();
    }

    public void setVisible(bool v)
    {
        visibleRoom.setVisible(v);
    }

    internal void linkRoom(Room newRoom)
    {
        linkedRooms.Add(newRoom);
    }

    /// <summary>
    /// Event coming from the RoomBounds stating that the player entered the room
    /// </summary>
    /// <param name="player"></param>
    public void playerEnteredRoom(PlayerController player)
    {
        foreach (Room room in linkedRooms)
        {
            room.playerEnteredRoom(player);
        }
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
        foreach (Room room in linkedRooms)
        {
            room.playerExitedRoom(player);
        }
        foreach (RoomEvent ev in roomEvents)
        {
            ev.playerExitedRoom(player);
        }
    }

    internal void initialize(RoomBasedMapGenerator map)
    {
        this.map = map;
        RoomEvent[] events = GetComponents<RoomEvent>();
        foreach (RoomEvent ev in events)
        {
            ev.initialize();
        }

        // Trying Static Batching without success

        /*
        Transform[] childrens = GetComponentsInChildren<Transform>();
        GameObject[] childrenGos = new GameObject[childrens.Length];
        for (int i=0; i < childrenGos.Length; i++)
        {
            childrenGos[i] = childrens[i].gameObject;
        }
        StaticBatchingUtility.Combine(childrenGos, gameObject);
        for (int i = 0; i < childrenGos.Length; i++)
        {
            childrenGos[i].gameObject.SetActive(false);
            childrenGos[i].gameObject.SetActive(true);
        }
        */
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
