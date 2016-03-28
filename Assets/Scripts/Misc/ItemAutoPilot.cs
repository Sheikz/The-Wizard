using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Item))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemAutoPilot : AutoPilot
{
    void Start()
    {
        activated = true;
        state = PilotState.DoNothing;
    }

    void FixedUpdate()
    {
        if (!activated)
            return;

        switch (state)
        {
            case PilotState.LockedToObject:
                steerToTarget(targetObject, PilotState.DoNothing);
                break;
        }
    }
}
