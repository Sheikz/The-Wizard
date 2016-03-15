using UnityEngine;
using System.Collections.Generic;

public class WallTemplate : MonoBehaviour
{
    public List<ItemWithDropChance> wallPrefabs;
    public GameObject[] exteriorCornerPrefabsNorthWest;
    public GameObject[] exteriorCornerPrefabsNorthEast;
    public GameObject[] interiorCornerPrefabsSouthEast;
    public GameObject[] interiorCornerPrefabsSouthWest;
    public GameObject[] door;
    public List<ItemWithDropChance> floorPrefabs;
    public List<ItemWithDropChance> wallDecos;
    public MeshCreator meshCreator;

    public GameObject wall
    {
        get
        {
            return ItemWithDropChance.getItem(wallPrefabs).item;
        }

    }

    public GameObject wallDeco
    {
        get
        {
            return ItemWithDropChance.getItem(wallDecos).item;
        }
    }

    public GameObject floor
    {
        get
        {
            return ItemWithDropChance.getItem(floorPrefabs).item;
        }

    }

    public List<ItemWithDropChance> getWallDecos()
    {
        return wallDecos;
    }

    public GameObject getDoor(DoorType doorType)
    {
        if (doorType == DoorType.Normal)
            return door[0];
        else
            return door[1];
    }
}
