using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class RoomBasedMapGenerator : MonoBehaviour
{
    public WallTemplate wallTemplate;
    public GameObject startingRoom;
    public GameObject roomLiaison;
    public List<ItemWithDropChance> roomPrefabs;
    public List<ItemWithDropChance> bossRoomPrefabs;
    public bool bossRoom = true;
    public float allowedGenerationTime;
    public int roomNumber;
    public int distanceBetweenRoom = 4;
    public GridMap gridMap;
    public List<ItemWithDropChance> monsters;
    [Range(0, 100)]
    public int monsterNumber;
    [Range(0f,1f)]
    public float mergingChance = 0.5f;

    private List<WallCreator> availableWalls;
    private GameObject liaisonHolder;
    private List<Room> roomList;
    [HideInInspector]
    public List<GameObject> proxyObjects;
    private PlayerController hero;
    [HideInInspector]
    public List<NPCController> monsterList;
    [HideInInspector]
    public Transform spellHolder;
    [HideInInspector]
    public List<Tile> floorTiles;
    [HideInInspector]
    public Transform monsterHolder;

    public void Start()
    {
        if (roomPrefabs.Count == 0)
        {
            Debug.LogError("Not a single room to instantiate");
            return;
        }

        availableWalls = new List<WallCreator>();
        roomList = new List<Room>();
        proxyObjects = new List<GameObject>();
        monsterList = new List<NPCController>();
        liaisonHolder = new GameObject();
        liaisonHolder.name = "Liaison Holder";
        liaisonHolder.transform.SetParent(transform);

        spellHolder = new GameObject("Spells").transform;

        hero = GameManager.instance.hero;
        hero.transform.position = new Vector3(0, 5f, 0f);
        createMap();
        createDecos();
        //createMonsters();
    }

    public void createMap()
    {
        float startingTime = Time.realtimeSinceStartup;
        int roomCount = 1;
        int stepCount = 0;

        roomList.Add(createRoom(startingRoom, Vector3.zero, Quaternion.identity));

        while (Time.realtimeSinceStartup - startingTime < allowedGenerationTime && roomCount < roomNumber && stepCount <= 1000)
        {
            stepCount++;
            WallCreator wall = Utils.pickRandom(availableWalls);
            Vector3 roomPosition = wall.getNextRoomPosition(distanceBetweenRoom);
            Room newRoom = createRoom(ItemWithDropChance.getItem(roomPrefabs).item, roomPosition, wall.transform.rotation);
            if (!newRoom)
                continue;
            if (Random.value < mergingChance && wall.parentRoom.canBeMergedAtExit && newRoom.canBeMergedAtEntrance)   // Merging the room
            {
                newRoom.setMerged(true);
                wall.doorStatus = WallCreator.DoorStatus.Open;
                VisibleRoom newLiaison = (Instantiate(roomLiaison, roomPosition + wall.transform.rotation * Vector3.down * 2, wall.transform.rotation)
                    as GameObject).GetComponent<VisibleRoom>();  // Instantiate the liaison

                newLiaison.transform.SetParent(liaisonHolder.transform);
                wall.parentRoom.visibleRoom.linkTo(newLiaison);
                wall.parentRoom.visibleRoom.linkTo(newRoom.visibleRoom);
                wall.parentRoom.linkRoom(newRoom);
            }
            else
                wall.doorStatus = WallCreator.DoorStatus.Door;

            newRoom.depth = wall.parentRoom.depth + 1;
            roomList.Add(newRoom);
            wall.refreshContents();
            availableWalls.Remove(wall);
            roomCount++;
        }
        createBossRoom();
        gridMap = new GridMap(this); // Create a grid representation of the map
        initializeRooms();
        Debug.Log("Created map in: " + (Time.realtimeSinceStartup - startingTime) + " seconds");
        Debug.Log("Number of steps: " + stepCount);
        Debug.Log("Number of rooms created " + roomCount);
    }

    /// <summary>
    /// Destroy the map, the monsters and the spells
    /// </summary>
    internal void destroy()
    {
        Destroy(monsterHolder.gameObject);
        Destroy(spellHolder.gameObject);
        Destroy(gameObject);
    }

    private void initializeRooms()
    {
        floorTiles = gridMap.getTilesOfType(TileType.Floor);
        monsterHolder = new GameObject("Monsters").transform;
        foreach (Room room in roomList)
        {
            room.initialize(this);
        }
    }

    public void createDecos()
    {
        int decoCount = 0;
        float t = Time.realtimeSinceStartup;
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            // Copy constructor
            List<ItemWithDropChance> wallDecos = new List<ItemWithDropChance>(wallTemplate.getWallDecos());

            // The idea is to check all decos and verify if the constraints are verified
            // We iterate in reverse order to allow removing while iterating
            for (int i = wallDecos.Count - 1; i >= 0; i--)
            {
                if (wallDecos[i].item == null)
                    continue;
                DecoConstraint[] constraints = wallDecos[i].item.GetComponents<DecoConstraint>();
                bool passedAllChecks = true;
                foreach (DecoConstraint constraint in constraints)
                {
                    if (!constraint.checkConstraint(wall.transform, this))
                    {
                        passedAllChecks = false;
                        break;
                    }
                }
                if (!passedAllChecks)
                    wallDecos.RemoveAt(i);
            }

            GameObject toInstantiate = ItemWithDropChance.getItem(wallDecos).item;
            if (toInstantiate == null)
                continue;

            GameObject newDeco = Instantiate(toInstantiate, wall.transform.position, wall.transform.rotation) as GameObject;
            ProximityConstraint proxyObject = newDeco.GetComponent<ProximityConstraint>();
            if (proxyObject)
                proxyObjects.Add(proxyObject.gameObject);       // Need to add the objects to the list of proxy objects

            newDeco.transform.SetParent(wall.transform);
            decoCount++;
        }
        Debug.Log("Created " + decoCount + " wall decorations in " + (Time.realtimeSinceStartup - t) + " seconds");
    }

    /// <summary>
    /// Create the monsters
    /// </summary>
    public void createMonsters()
    {
        if (monsters.Count == 0)
        {
            Debug.LogError("No monsters defined!");
            return;
        }

        // Only take the tiles that are far enough from the hero
        List<Tile> floorTilesToPutMonster = new List<Tile>();
        foreach (Tile tile in floorTiles)
        {
            if ((tile.position() - hero.transform.position).magnitude > 15)      // Arbritrary safe distance value (greater than vision distance of mobs)
                floorTilesToPutMonster.Add(tile);
        }

        if (floorTilesToPutMonster.Count == 0)
        {
            Debug.Log("Not a single floor to put monsters");
            return;
        }

        Transform monsterHolder = new GameObject("Monsters").transform;
        monsterHolder.SetParent(transform);
        int monsterCount = 0;
        while (monsterCount < monsterNumber)
        {
            Tile tileToPutMonster = Utils.pickRandom(floorTilesToPutMonster);
            NPCController newMonsterPrefab = ItemWithDropChance.getItem(monsters).item.GetComponent<NPCController>();
            if (tileToPutMonster.getDistanceToClosest(newMonsterPrefab.isFlying) <= newMonsterPrefab.getRadius())   // Check if there is enough room to put the monster
                continue;
            NPCController newMonster = Instantiate(newMonsterPrefab, tileToPutMonster.position(), Quaternion.identity) as NPCController;
            newMonster.transform.SetParent(monsterHolder);
            newMonster.initialize(this);
            monsterList.Add(newMonster);
            monsterCount++;
        }
    }

    /// <summary>
    /// Check if the room can be created at this position.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="position"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public Room createRoom(GameObject room, Vector3 position, Quaternion rot)
    {
        BoxCollider2D[] edges = room.GetComponent<Room>().getEdges();   // Get the prefab edges
        foreach (BoxCollider2D edge in edges)
        {
            BoxCollider2D newRoomEdge = Instantiate(edge);      // Instantiate a copy of the edge prefabs
            newRoomEdge.transform.position = position;
            newRoomEdge.transform.rotation = rot;

            if (isTouchingAnotherRoom(newRoomEdge.bounds))  // Check if the edge intersect with any room of the dungeon
            {
                Destroy(newRoomEdge.gameObject);
                return null;
            }
            Destroy(newRoomEdge.gameObject);    // The edges are no longer needed. Destroy the GameObject
        }
        Room newRoom = (Instantiate(room, position, Quaternion.identity) as GameObject).GetComponent<Room>();
        newRoom.randomize();
        newRoom.refreshWalls();
        newRoom.transform.rotation = rot;
        newRoom.transform.SetParent(transform);

        foreach (WallCreator wall in newRoom.exitWalls)
        {
            availableWalls.Add(wall);           // Add the exits walls as potential entrances
        }
        return newRoom;
    }

    /// <summary>
    /// Create the boss room. Need to be added after the room with most depth.
    /// If this room is not available, then iterate backward until finding a good candidate
    /// </summary>
    private void createBossRoom()
    {
        if (!bossRoom)
            return;
        roomList.Sort();    // First we sort the rooms according to depth
        foreach (Room room in roomList)
        {
            foreach (WallCreator wc in room.exitWalls)
            {
                if (wc.doorStatus != WallCreator.DoorStatus.Wall)   
                    continue;       // This wall is not available
                Vector3 roomPosition = wc.getNextRoomPosition(distanceBetweenRoom);
                Room newRoom = createRoom(ItemWithDropChance.getItem(bossRoomPrefabs).item, roomPosition, wc.transform.rotation);
                if (!newRoom)
                    continue;

                // Room creation succeeded at this point
                wc.doorStatus = WallCreator.DoorStatus.Door;
                wc.refreshContents();
                roomList.Add(newRoom);
                return;
            }
        }
    }


    /// <summary>
    /// Check if the room is touching another room
    /// </summary>
    /// <param name="Bounds of the room"></param>
    /// <returns></returns>
    private bool isTouchingAnotherRoom(Bounds col)
    {
        List<Bounds> edgeBounds = getEdgeBounds();
                
        foreach (Bounds colToCheck in edgeBounds)
        {
            if (col.Intersects(colToCheck))
            {
                return true;
            }
        }
        return false;
    }

    private List<Bounds> getEdgeBounds()
    {
        List<Bounds> result = new List<Bounds>();
        foreach (Room room in roomList)
        {
            foreach (BoxCollider2D edgeCollider in room.getEdges())
            {
                Bounds currentBounds = edgeCollider.bounds;
                //currentBounds.center = edgeCollider.transform.position;
                result.Add(currentBounds);
            }
        }
        return result;
    }

    public void revealRooms()
    {
        foreach (Room r in roomList)
        {
            r.setVisible(true);
        }
    }

    public void levelUpMonsters()
    {
        for (int i = monsterList.Count - 1; i >= 0; i--)
        {
            if (monsterList[i] == null)
            {
                monsterList.RemoveAt(i);
                continue;
            }
            CharacterStats monsterStats = monsterList[i].GetComponent<CharacterStats>();
            if (monsterStats)
                monsterStats.levelUp();
        }
    }

    /// <summary>
    /// Debug gizmos to see the grid
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        if (gridMap.grid == null)
            return;
        foreach (Tile t in gridMap.grid)
        {
            if (t.type == TileType.Floor)
            {
                if (t.distanceToClosestBlocking <= 0.9f)
                    Gizmos.color = Color.red;
                else if (t.distanceToClosestBlocking <= 1.6f)
                    Gizmos.color = Color.yellow;
                else if (t.distanceToClosestBlocking <= 2.4f)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.white;
                /*
                if (t.distanceToClosestHighBlocking <= 0.9f)
                    Gizmos.color = Color.red;
                else if (t.distanceToClosestHighBlocking <= 1.6f)
                    Gizmos.color = Color.yellow;
                else if (t.distanceToClosestHighBlocking <= 2.4f)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.white;*/

                Gizmos.DrawSphere(t.position(), 0.1f);
            }
            else if (t.type == TileType.Wall)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(t.position(), 0.1f);
            }
            else if (t.type == TileType.Hole)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(t.position(), 0.1f);
            }
        }
    }
}
