using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public Tile[,] grid;
    public GameObject[] floorList;
    public int width;
    public int height;
    public Vector2i offset;

    private AStarPathFinder pathFinder;

    public GridMap(RoomBasedMapGenerator map)
    {
        Countf floorX = new Countf(Mathf.Infinity, -Mathf.Infinity);
        Countf floorY = new Countf(Mathf.Infinity, -Mathf.Infinity);
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] wallTiles = GameObject.FindGameObjectsWithTag("Wall");
        MeshCreator meshCreator;
        List<Vector3> positionList;
        // First, find out the size of the array required
        foreach (GameObject wall in wallTiles)
        {
            MeshCreator wallMesh = wall.GetComponent<MeshCreator>();
            if (wallMesh)   // If the wall object is a mesh, we need to compute take each individual positions into account
            {
                positionList = wallMesh.getPositions();
                foreach (Vector3 pos in positionList)
                {
                    if (pos.x < floorX.minimum)
                        floorX.minimum = pos.x;
                    if (pos.x > floorX.maximum)
                        floorX.maximum = pos.x;
                    if (pos.y < floorY.minimum)
                        floorY.minimum = pos.y;
                    if (pos.y > floorY.maximum)
                        floorY.maximum = pos.y;
                }
            }
            else
            {
                if (wall.transform.position.x < floorX.minimum)
                    floorX.minimum = wall.transform.position.x;
                if (wall.transform.position.x > floorX.maximum)
                    floorX.maximum = wall.transform.position.x;
                if (wall.transform.position.y < floorY.minimum)
                    floorY.minimum = wall.transform.position.y;
                if (wall.transform.position.y > floorY.maximum)
                    floorY.maximum = wall.transform.position.y;
            }
        }
        offset = new Vector2i(Mathf.RoundToInt(floorX.minimum), Mathf.RoundToInt(floorY.minimum));

        width  = Mathf.RoundToInt(floorX.maximum) - offset.x + 1;    // +1 is necessary to take the tile [0,0] into account
        height = Mathf.RoundToInt(floorY.maximum) - offset.y + 1;
        grid = new Tile[height, width];
        Debug.Log("creating a grid of size :" + width + ", " + height);

        for (int x = 0; x < width; x ++)
        {
            for (int y = 0; y < height; y ++)
            {
                grid[y, x] = new Tile(x + offset.x, y + offset.y, TileType.Wall, this);
                grid[y, x].distanceToClosestBlocking = getDistanceToClosestObject(new Vector3(x + offset.x, y + offset.y, 0), GameManager.instance.layerManager.blockingLayer);
                grid[y, x].distanceToClosestHighBlocking = getDistanceToClosestObject(new Vector3(x + offset.x, y + offset.y, 0), GameManager.instance.layerManager.highBlockingLayer);
            }
        }
        foreach (GameObject floor in floorTiles) // Put the floors
        {
            meshCreator = floor.GetComponent<MeshCreator>();
            if (meshCreator)
            {
                positionList = meshCreator.getPositions();
                foreach (Vector3 pos in positionList)
                {
                    Tile t = getTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
                    if (t != null)
                        t.type = TileType.Floor;
                }
            }
            else
            {
                int x = Mathf.RoundToInt(floor.transform.position.x);
                int y = Mathf.RoundToInt(floor.transform.position.y);
                getTile(x, y).type = TileType.Floor;
            }
        }
        foreach(GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))   // Update the obstacles position (pillar, rumbles, ...)
        {
            int x = Mathf.RoundToInt(obstacle.transform.position.x);
            int y = Mathf.RoundToInt(obstacle.transform.position.y);
            Tile t = getTile(x, y);
            if (t != null)
                t.type = TileType.Wall;
        }
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Hole"))   // Update the obstacles position (pillar, rumbles, ...)
        {
            meshCreator = obstacle.GetComponent<MeshCreator>();
            if (meshCreator)
            {
                positionList = meshCreator.getPositions();
                foreach (Vector3 pos in positionList)
                {
                    Tile t = getTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
                    if (t != null)
                        t.type = TileType.Hole;
                }
            }
            else
            {
                int x = Mathf.RoundToInt(obstacle.transform.position.x);
                int y = Mathf.RoundToInt(obstacle.transform.position.y);
                getTile(x, y).type = TileType.Hole;
            }
        }

        pathFinder = new AStarPathFinder(this);
    }

    /// <summary>
    /// Check the distance around the tile to see if blocking objects are nearby
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private float getDistanceToClosestObject(Vector3 point, LayerMask layer)
    {
        float[] radiusToTest = new float[] { 0.9f, 1.6f, 2.4f };
        foreach (float radius in radiusToTest)
        {
            if (Physics2D.OverlapCircle(point, radius, layer))
                return radius;
        }
        return Mathf.Infinity;
    }

    public List<Tile> getTilesOfType(TileType type)
    {
        List<Tile> result = new List<Tile>();
        foreach (Tile t in grid)
        {
            if (t.type == type)
                result.Add(t);
        }
        return result;
    }

    /// <summary>
    /// Given a world position, return the tile
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile getTile(float x, float y)
    {
        int targetX = Mathf.RoundToInt(x) - offset.x;
        int targetY = Mathf.RoundToInt(y) - offset.y;
        if (!checkValues(targetX, targetY))
            return null;

        return grid[targetY, targetX];
    }

    public Tile getTile(Vector2i v)
    {
        int targetX = v.x - offset.x;
        int targetY = v.y - offset.y;

        if (!checkValues(targetX, targetY))
            return null;

        return grid[targetY, targetX];
    }

    public bool checkValues(int x, int y)
    {
        if (x >= width || x < 0)
            return false;
        if (y >= height || y < 0)
            return false;

        return true;
    }

    public List<Tile> getNeighbors(Tile tile, float radius, bool isFlying, bool isGhost)
    {
        List<Tile> result = new List<Tile>();
        Vector2i[] directions = new Vector2i[] { new Vector2i(1, 0), new Vector2i(-1, 0), new Vector2i(0, 1), new Vector2i(0, -1) };
        foreach (Vector2i dir in directions)
        {
            Vector2i neighbor = tile.position2i() + dir;
            Tile neighborTile = getTile(neighbor.x, neighbor.y);
            if (neighborTile == null)
                continue;
            
            if (!neighborTile.isBlocking(isFlying, isGhost)) // is the tile blocking for this monster?
            {
                if (neighborTile.getDistanceToClosest(isFlying) > radius)   // check if there is enough space to go there relative to the size of the mob
                    result.Add(getTile(neighbor.x, neighbor.y));
            }
                
        }
        return result;
    }

    public Stack<Tile> getPath(Vector2i start, Vector2i goal, NPCController npc)
    {
        return pathFinder.getPath(getTile(start), getTile(goal), npc);
    }

    public Tile findRandomTileWithinRadius(Vector3 position, float radius, TileType type)
    {
        List<Tile> result = new List<Tile>();
        List<Tile> floorTiles = getTilesOfType(type);
        foreach (Tile tile in floorTiles)
        {
            if ((tile.position() - position).sqrMagnitude <= (radius * radius))
            {
                result.Add(tile);
            }
        }
        return Utils.pickRandom(result);
    }

    /// <summary>
    /// Overloaded method taking a space parameter representating the necessary space with blockinglayer
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    /// <param name="space"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Tile findRandomTileWithinRadius(Vector3 position, float radius, float space, bool isFlying, TileType type)
    {
        List<Tile> result = new List<Tile>();
        List<Tile> floorTiles = getTilesOfType(type);
        foreach (Tile tile in floorTiles)
        {
            if ((tile.position() - position).sqrMagnitude <= (radius * radius))
            {
                if (!isFlying)   // If the mob is not flying, we check low and high blocking objects
                {
                    if (tile.distanceToClosestBlocking > space)
                        result.Add(tile);
                }
                else            // If the mob is flying, we check only the high blocking objects
                {
                    if (tile.distanceToClosestHighBlocking > space)
                        result.Add(tile);
                }
            }
        }
        return Utils.pickRandom(result);
    }

    public Tile getRandomNotBlockingNeighbor(Vector3 position, bool isFlying, bool isGhost)
    {
        Tile tile = getTile(position.x, position.y);
        List<Tile> result = new List<Tile>();
        Vector2i[] directions = new Vector2i[] { new Vector2i(1, 0), new Vector2i(-1, 0), new Vector2i(0, 1), new Vector2i(0, -1),
                                                 new Vector2i(1, 1), new Vector2i(-1, 1), new Vector2i(1, -1), new Vector2i(-1, -1)};
        foreach (Vector2i dir in directions)
        {
            Vector2i neighbor = tile.position2i() + dir;
            Tile neighborTile = getTile(neighbor.x, neighbor.y);
            if (neighborTile == null)
                continue;

            if (!neighborTile.isBlocking(isFlying, isGhost)) // is the tile blocking for this monster?
            {
                result.Add(getTile(neighbor.x, neighbor.y));
            }

        }
        return Utils.pickRandom(result);
    }
}