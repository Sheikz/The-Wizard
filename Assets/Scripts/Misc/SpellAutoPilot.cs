﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpellController))]
public class SpellAutoPilot : AutoPilot 
{
    public float detectionRadius = 3f;
    public float detectionDistance = 3f;

    private SpellController spell;
    private LayerMask enemyLayer;

    new void Awake()
    {
        base.Awake();
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        state = PilotState.Searching;
        // if the spell is emitted by the hero, it needs to lock on monsters
        if (spell.emitter.gameObject.CompareTag("Hero") || spell.emitter.gameObject.CompareTag("HeroCompanion"))
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
        else    // If not, it needs to lock on hero
            enemyLayer = GameManager.instance.layerManager.heroLayer;
    }

    void FixedUpdate()
    {
        if (!activated)
            return;

        switch (state)
        {
            case PilotState.DoNothing:
                return;
            case PilotState.Searching:
                searchTarget();
                break;
            case PilotState.LockedToObject:
                steerToTarget(targetObject, PilotState.Searching);
                break;
            case PilotState.LockedToPosition:
                steerToTarget();
                break;
        }
    }

    void searchTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position + (rigidBody.velocity.normalized * detectionRadius).toVector3(),
            detectionRadius, 
            rigidBody.velocity, 
            detectionDistance, 
            enemyLayer);

        if (hits.Length <= 0)
            return;

        Collider2D closestObject = getClosest(hits);
        if (closestObject == null)
            return;

        Damageable dmg = closestObject.GetComponent<Damageable>();
        if (dmg)
        {
            lockToObject(dmg.gameObject);
        }
    }

    Collider2D getClosest(RaycastHit2D[] hits)
    {
        if (hits.Length <= 0)
            return null;

        Collider2D result = hits[0].collider;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < hits.Length; i++)
        {
            if (spell.ignoredColliders.Contains(hits[i].collider))
                continue;
            float sqrDistance = (hits[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                result = hits[i].collider;
            }
        }
        if (spell.ignoredColliders.Contains(result))
            return null;

        return result;
    }

    
}
   