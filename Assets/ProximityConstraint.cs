using UnityEngine;
using System.Collections.Generic;
using System;

public class ProximityConstraint : DecoConstraint
{
    public float minimumDistance = 2f;

    public override bool checkConstraint(Transform wallTransform, RoomBasedMapGenerator map)
    {
        return checkSurroundings(wallTransform.position, map.proxyObjects);
    }

    /// <summary>
    /// Check if there is already an object close to the position
    /// </summary>
    /// <param name="position"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public bool checkSurroundings(Vector3 position, List<GameObject> pcList)
    {
        foreach (GameObject pc in pcList)
        {
            if (!pc.name.Contains(gameObject.name)) // Check if it's the same object by checking name subset because of "Clone"
                continue;

            if ((position - pc.transform.position).magnitude < minimumDistance)
            {
                return false;
            }
        }
        return true;
    }
}
