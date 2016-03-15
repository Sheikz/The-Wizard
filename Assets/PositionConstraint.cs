using UnityEngine;
using System.Collections.Generic;
using System;

public class PositionConstraint : DecoConstraint
{
    public override bool checkConstraint(Transform wallTransform, RoomBasedMapGenerator map)
    {
        if (Physics2D.Raycast(wallTransform.position, wallTransform.rotation * Vector2.up))
            return false;
        else
            return true;
    }
}
