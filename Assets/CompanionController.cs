using UnityEngine;
using System.Collections.Generic;
using System;

public class CompanionController : NPCController
{
    public float minimumDistanceToMaster = 1f;

    private GameObject master;
    private bool hasGoal = false;


    public void initialize(GameObject master, RoomBasedMapGenerator map)
    {
        initialize(map);
        this.master = master;
    }

    protected override void doWander()
    {
        if (searchTarget())
            return;

        followMaster();

        if (!canMove)
            return;
        moveToTarget();
    }

    void followMaster()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, master.transform.position, GameManager.instance.layerManager.obstacleLayer);
        if (hit)
        {
            computePathToMaster();
            return;
        }
        hasGoal = false;
        Vector3 lineToMaster = master.transform.position - transform.position;
        if (lineToMaster.magnitude <= minimumDistanceToMaster)
            target = transform.position;
        else
        {
            target = master.transform.position;
            updateMovementToReachTarget();
        }
    }

    void computePathToMaster()
    {
        if (!hasGoal)
        {
            goal = master.transform.position;
            if (!computePathToGoal())
                transform.position = master.transform.position;
            hasGoal = true;
        }

        if (hasReached(target))
            setNewTarget();
        if (hasReached(goal))
            hasGoal = false;

        updateMovementToReachTarget();
    }

    public override void die()
    {
        SpellCaster caster = master.GetComponent<SpellCaster>();
        if (caster)
            caster.removeFollower(this);
    }

    public override void receivesDamage()
    {
    }

    
}
