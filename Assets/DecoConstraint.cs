using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represent a constraint that need to be verified to put the deco
/// </summary>
public abstract class DecoConstraint : MonoBehaviour
{
    public bool destroyWall = false; // indicate if the wall should be destroyed
    public abstract bool checkConstraint(Vector3 wallPos, Transform wall, RoomBasedMapGenerator map);
}

