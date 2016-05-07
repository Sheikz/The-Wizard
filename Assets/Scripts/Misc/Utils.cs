using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public static class Utils
{
	/// <summary>
	/// Pick a random GameObject from an array of objects
	/// </summary>
	/// <param name="objects">objects array</param>
	/// <returns>a random object</returns>
	public static GameObject pickRandom(params GameObject[] objects)
	{
		if (objects.Length == 0)
			return null;

		return objects[Random.Range(0, objects.Length)];
	}

	public static Sprite pickRandom(params Sprite[] objects)
	{
		return objects [Random.Range (0, objects.Length)];
	}

	public static T pickRandom<T>(List<T> list)
	{
		if (list.Count == 0)
			return default(T);
		return list[Random.Range(0, list.Count)];
	}

    public static float pickRandom(params float[] input)
    {
        if (input.Length == 0)
            return 0.0f;
        return input[Random.Range(0, input.Length)];
    }

    public static int pickRandomIndexWithDifferentChances(params float[] input)
    {
        if (input == null)
            return -1;

        float totalSum = 0f;
        foreach (float chance in input)
        {
            totalSum += chance;
        }
        float index = Random.Range(0f, totalSum);
        float sum = 0f;
        int i = 0;
        while (sum < index)
        {
            sum += input[i++];
        }
        return i - 1;
    }

    public static Color[] pickRandom(params Color[][] objects)
    {
        return objects[Random.Range(0, 2)];
    }

    public static bool randomBool()
    {
        if (Random.Range(0, 2) == 0)
            return true;
        else
            return false;
    }

    public static void rotate(this Vector2 v, float degrees)
    {
        v = Quaternion.Euler(0, 0, degrees) * v;
    }

    /// <summary>
    /// Return an object based on the probability of dropping
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    public static ItemWithDropChance getObjectWithProbability(List<ItemWithDropChance> objects)
    {
        if (objects == null)
            return null;

        float totalSum = 0f;
        foreach (ItemWithDropChance obj in objects)
        {
            totalSum += obj.lootChance;
        }
        float index = Random.Range(0, totalSum);
        float sum = 0f;
        int i = 0;
        while (sum < index)
        {
            sum += objects[i++].lootChance;
        }
        return objects[Math.Max(0, i - 1)];
    }
}

public static class TransformDeepChildExtension
{
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }
}

[System.Serializable]
public class ItemWithDropChance
{
    public GameObject item;
    [Range(0, 100)]
    public float lootChance = 1;

    /// <summary>
    /// Return an object based on the probability of dropping
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    public static ItemWithDropChance getItem(List<ItemWithDropChance> objects)
    {
        if (objects == null)
            return null;

        float totalSum = 0f;
        foreach (ItemWithDropChance obj in objects)
        {
            totalSum += obj.lootChance;
        }
        float index = Random.Range(0, totalSum);
        float sum = 0f;
        int i = 0;
        while (sum < index)
        {
            sum += objects[i++].lootChance;
        }
        if (objects[Math.Max(0, i - 1)] == null)
        {
            Debug.Log("here");
            return null;
        }
        else
            return objects[Math.Max(0, i - 1)];
    }
}

public enum Direction { NORTH, WEST, SOUTH, EAST, NORTHEAST, SOUTHEAST, SOUTHWEST, NORTHWEST, CENTER, UNKNOWN};
[SerializeField]
public enum SpellType { Primary, Secondary, Defensive, Ultimate1, Ultimate2};
public enum MagicElement { Light, Air , Fire, Arcane, Ice, Earth, Shadow };
public enum SpellSet { SpellSet1, SpellSet2 };

static class SpellTypeMethods
{
    public static int getInt(this SpellType t)
    {
        switch (t)
        {
            case SpellType.Primary:     return 0;
            case SpellType.Secondary:   return 1;
            case SpellType.Defensive:   return 2;
            case SpellType.Ultimate1:   return 3;
            case SpellType.Ultimate2:   return 4;
            default:                    return 0;
        }
    }
}

static class Vector2Extensions
{
    public static Vector3 toVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }
}

[System.Serializable]
public class Count
{
	public int minimum;
	public int maximum;
	public Count(int min, int max)
	{
		minimum = min;
		maximum = max;
	}

    public int getRandom()
    {
        return Random.Range(minimum, maximum+1);
    }
}

[System.Serializable]
public class Countf
{
    public float minimum;
    public float maximum;
    public Countf(float min, float max)
    {
        minimum = min;
        maximum = max;
    }

    public float getRandom()
    {
        return Random.Range(minimum, maximum);
    }
}

[System.Serializable]
public class WorldObject
{
    public GameObject[] North;
    public GameObject[] East;
    public GameObject[] South;
    public GameObject[] West;
    public GameObject[] NorthEast;
    public GameObject[] SouthEast;
    public GameObject[] SouthWest;
    public GameObject[] NorthWest;

    public GameObject get(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return Utils.pickRandom(North);
            case Direction.EAST: return Utils.pickRandom(East);
            case Direction.SOUTH: return Utils.pickRandom(South);
            case Direction.WEST: return Utils.pickRandom(West);
            case Direction.NORTHEAST: return Utils.pickRandom(NorthEast);
            case Direction.SOUTHEAST: return Utils.pickRandom(SouthEast);
            case Direction.SOUTHWEST: return Utils.pickRandom(SouthWest);
            case Direction.NORTHWEST: return Utils.pickRandom(NorthWest);
            default: return null;
        }
    }
}



/// <summary>
/// For this one, they are a set an cannot be mixed (examples: floor decorations)
/// </summary>
[System.Serializable]
public class WorldObjectSet
{
    public GameObject North;
    public GameObject East;
    public GameObject South;
    public GameObject West;
    public GameObject NorthEast;
    public GameObject SouthEast;
    public GameObject SouthWest;
    public GameObject NorthWest;

    public GameObject get(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return North;
            case Direction.EAST: return East;
            case Direction.SOUTH: return South;
            case Direction.WEST: return West;
            case Direction.NORTHEAST: return NorthEast;
            case Direction.SOUTHEAST: return SouthEast;
            case Direction.SOUTHWEST: return SouthWest;
            case Direction.NORTHWEST: return NorthWest;
            default: return null;
        }
    }
}

[System.Serializable]
public class WorldObjectSetWithCenter
{
    public GameObject North;
    public GameObject East;
    public GameObject South;
    public GameObject West;
    public GameObject NorthEast;
    public GameObject SouthEast;
    public GameObject SouthWest;
    public GameObject NorthWest;
    public GameObject Center;

    public GameObject get(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return North;
            case Direction.EAST: return East;
            case Direction.SOUTH: return South;
            case Direction.WEST: return West;
            case Direction.NORTHEAST: return NorthEast;
            case Direction.SOUTHEAST: return SouthEast;
            case Direction.SOUTHWEST: return SouthWest;
            case Direction.NORTHWEST: return NorthWest;
            case Direction.CENTER: return Center;
            default: return null;
        }
    }
}

[System.Serializable]
public class WorldObject4Sides
{
    public GameObject[] North;
    public GameObject[] East;
    public GameObject[] South;
    public GameObject[] West;

    public GameObject get(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return Utils.pickRandom(North);
            case Direction.EAST: return Utils.pickRandom(East);
            case Direction.SOUTH: return Utils.pickRandom(South);
            case Direction.WEST: return Utils.pickRandom(West);
            default: return null;
        }
    }
}

[System.Serializable]
public class WorldObject4Corners
{
    public GameObject NorthEast;
    public GameObject NorthWest;
    public GameObject SouthEast;
    public GameObject SouthWest;

    public GameObject get(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTHEAST: return NorthEast;
            case Direction.NORTHWEST: return NorthWest;
            case Direction.SOUTHWEST: return SouthWest;
            case Direction.SOUTHEAST: return SouthEast;
            default: return null;
        }
    }
}


[System.Serializable]
public class Vector2i
{
	public int x;
	public int y;

	public static Vector2i North = new Vector2i(0, 1);
	public static Vector2i East = new Vector2i(1, 0);
	public static Vector2i West = new Vector2i(-1, 0);
	public static Vector2i South = new Vector2i(0, -1);

	public static Vector2i NorthEast = new Vector2i(1, 1);
	public static Vector2i NorthWest = new Vector2i(-1, 1);
	public static Vector2i SouthEast = new Vector2i(1, -1);
	public static Vector2i SouthWest = new Vector2i(-1, -1);

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2i(Vector3 v)
	{
		x = Mathf.RoundToInt(v.x);
		y = Mathf.RoundToInt(v.y);
	}

	public override bool Equals(object obj)
	{
		return obj is Vector2i && Equals((Vector2i)obj);
	}

	public bool Equals(Vector2i v)
	{
		return (x == v.x) && (y == v.y);
	}

    /// <summary>
    /// 90 degrees rotation to the right (positive)
    /// </summary>
    /// <returns></returns>
    public Vector2i getPerpendicular()
    {
        return new Vector2i(y, -x);
    }

	public override int GetHashCode()
	{
		return x * 17 + y * 31;
	}

	public Vector3 toVector3()
	{
		return new Vector3(x, y);
	}

	public float sqrDistanceTo(Vector2i goal)
	{
		return Mathf.Pow(goal.x - x, 2) + Mathf.Pow(goal.x - x, 2);
	}

	public static Vector2i operator+ (Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x + b.x, a.y + b.y);
	}

	public static Vector2i operator- (Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x - b.x, a.y - b.y);
	}

    public static Vector2i operator *(Vector2i a, int b)
    {
        return new Vector2i(a.x * b, a.y * b);
    }

    public override string ToString()
	{
		return "[" + x + "," + y + "]";
	}
}

/// <summary>
/// A class used to encapsulates a tile and a direction
/// </summary>
[System.Serializable]
public class TileDirection
{
	public Tile tile;
	public Vector2i direction;

	public TileDirection(Tile t, Vector2i d)
	{
		tile = t;
		direction = d;
	}

	public Vector2i getStartingPosition()
	{
		int x = tile.x + direction.x;
		int y = tile.y + direction.y;
		return new Vector2i(x, y);
	}

	public Direction getDirection()
	{
		if (direction.x == 0 && direction.y == 1)
			return Direction.NORTH;
		else if (direction.x == 0 && direction.y == -1)
			return Direction.SOUTH;
		else if(direction.x == 1 && direction.y == 0)
			return Direction.EAST;
		else if(direction.x == -1 && direction.y == 0)
			return Direction.WEST;
		return Direction.UNKNOWN;
	}

    public static TileDirection operator +(TileDirection od, Vector2i dir)
    {
        Tile result = od.tile.getTileInDirection(dir);
        return new TileDirection(result, od.direction);
    }

    public static TileDirection operator -(TileDirection od, Vector2i dir)
    {
        Tile result = od.tile.getTileInDirection(dir *-1);
        return new TileDirection(result, od.direction);
    }

    public static TileDirection operator *(TileDirection od, int b)
    {
        return new TileDirection(od.tile, od.direction*b);
    }

    public Vector2i getTileBehind()
    {
        int x = tile.x + direction.x;
        int y = tile.y + direction.y;
        return new Vector2i(x, y);
    }

    public Vector2i getTileRight()
    {
        int x = tile.x + direction.getPerpendicular().x;
        int y = tile.y + direction.getPerpendicular().y;
        return new Vector2i(x, y);
    }

    public Vector2i getTileLeft()
    {
        int x = tile.x - direction.getPerpendicular().x;
        int y = tile.y - direction.getPerpendicular().y;
        return new Vector2i(x, y);
    }

}


