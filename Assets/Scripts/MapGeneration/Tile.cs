using UnityEngine;
using System.Collections;
using System;

public enum TileType { Wall, Floor, Hole };

public class Tile
{
    public TileType type = TileType.Wall;
    public int x;
    public int y;
    public float distanceToClosestBlocking = Mathf.Infinity;
    public float distanceToClosestHighBlocking = Mathf.Infinity;

    public float gScore = Mathf.Infinity;
    public float fScore = Mathf.Infinity;

    private GridMap map;

    public Tile(int x, int y, TileType t, GridMap map)
    {
        type = t;
        this.x = x;
        this.y = y;
        this.map = map;
    }

    public Tile()
    {
        type = TileType.Wall;
        x = 0;
        y = 0;
    }

    public Vector3 position()
    {
        return new Vector3(x, y, 0);
    }

    public Vector2i position2i()
    {
        return new Vector2i(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public bool isBlocking(bool isFlying, bool isGhost)
    {
        switch (type)
        {
            case TileType.Wall:
                return !isGhost;
            case TileType.Hole:
                if (isGhost)
                    return false;
                else
                    return !isFlying;
            default:
                return false;
        }
    }

    public Tile getTileInDirection(Vector2i direction)
    {
        return map.getTile(x + direction.x, y + direction.y);
    }

    public TileType[,] getSurroundings()
    {
        TileType[,] result = new TileType[3, 3];
        for (int i= -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (map.getTile(x + j, y - i) == null)
                    result[j+1, i+1] = TileType.Wall;
                else
                    result[j+1, i+1] = map.getTile(x + j, y - i).type;
            }
        }
        return result;
    }

    public Tile getTileNorth()
    {
        return map.getTile(x, y + 1);
    }

    public Tile getTileEast()
    {
        return map.getTile(x + 1, y);
    }

    public Tile getTileWest()
    {
        return map.getTile(x - 1, y);
    }

    public Tile getTileSouth()
    {
        return map.getTile(x, y - 1);
    }

    public Tile getTileNorthWest()
    {
        return map.getTile(x - 1, y + 1);
    }
    public Tile getTileNorthEast()
    {
        return map.getTile(x + 1, y + 1);
    }
    public Tile getTileSouthWest()
    {
        return map.getTile(x - 1, y - 1);
    }
    public Tile getTileSouthEast()
    {
        return map.getTile(x + 1, y - 1);
    }

    public float getDistanceToClosest(bool isFlying)
    {
        if (isFlying)
            return distanceToClosestHighBlocking;
        else
            return distanceToClosestBlocking;
    }
}
