using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class AStarPathFinder
{
    private GridMap map;

    public AStarPathFinder(GridMap map)
    {
        this.map = map;
    }

    public Stack<Tile> getPath(Tile start, Tile goal, NPCController npc)
    {
        bool isFlying = npc.isFlying;
        bool isGhost = npc.isGhost;
        float radius = npc.getRadius();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        HashSet<Tile> openSet = new HashSet<Tile>();
        Tile startingTile = computeStart(start, goal, radius, isFlying, isGhost);
        if (startingTile == null)
        {
            Debug.LogError("Did not find a starting tile for " + npc.name);
            startingTile = start;
        }

        openSet.Add(startingTile);
        int rows = map.width;
        int columns = map.height;
        int maxLengthAllowed = 200;

        // Creation and initialization of both scores to Infinity
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                map.grid[y, x].gScore = Mathf.Infinity;
                map.grid[y, x].fScore = Mathf.Infinity;
            }
        }
        start.gScore = 0;
        start.fScore = start.position2i().sqrDistanceTo(goal.position2i());

        while (openSet.Count > 0)
        {
            Tile current = getLowestF(openSet);
            if (current.position2i().Equals(goal.position2i()))
            {
                return reconstructPath(cameFrom, goal, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);
            List<Tile> neighbors = map.getNeighbors(current, radius, isFlying, isGhost);
            if (neighbors.Count == 0)
                Debug.Log("Neighbor count: " + neighbors.Count);
            foreach (Tile neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                    continue;       // Ignore the neighbor that is already evaluated

                float tentativeGScore = current.gScore + 1;   // Using 1 to estimate the distance between tiles. May use a cost value instead
                if (tentativeGScore >= maxLengthAllowed)
                {
                    return null;
                }

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= neighbor.gScore)
                    continue;

                cameFrom[neighbor] = current;
                neighbor.gScore = tentativeGScore;
                neighbor.fScore = tentativeGScore + neighbor.position2i().sqrDistanceTo(goal.position2i());
            }
        }
        return null;
    }

    private Tile computeStart(Tile start, Tile goal, float radius, bool isFlying, bool isGhost)
    {
        // If the start tile is wide enough for the monster, use it as a start tile
        if (isFlying && (start.distanceToClosestHighBlocking > radius))
            return start;
        else if (!isFlying && (start.distanceToClosestBlocking > radius))
            return start;

        // If not, we need to find a neighbor with enough space
        List<Tile> neighbors = map.getNeighbors(start, radius, isFlying, isGhost);
        Tile result = null;
        float minDistanceToGoal = Mathf.Infinity;
        foreach (Tile t in neighbors)
        {
            float distanceToGoal = (t.position() - goal.position()).sqrMagnitude;
            if (distanceToGoal < minDistanceToGoal)
            {
                result = t;
                minDistanceToGoal = distanceToGoal;
            }
        }
        return result;
    }

    private Stack<Tile> reconstructPath(Dictionary<Tile, Tile> cameFrom, Tile goal, Tile current)
    {
        Stack<Tile> path = new Stack<Tile>();
        path.Push(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Push(current);
        }
        return path;
    }

    private Tile getLowestF(HashSet<Tile> set)
    {
        Tile result = null;
        float lowestScore = Mathf.Infinity;
        foreach (Tile t in set)
        {
            if (t.fScore < lowestScore)
            {
                lowestScore = t.fScore;
                result = t;
            }
        }
        return result;
    }
}