using UnityEngine;
using System.Collections.Generic;
using System;

public class PositionConstraint : DecoConstraint
{
    public override bool checkConstraint(Vector3 wallPos, Transform wall, RoomBasedMapGenerator map)
    {
        if (Physics2D.Raycast(wallPos, wall.transform.rotation * Vector2.up))
            return false;
        else
            return true;
    }
}
