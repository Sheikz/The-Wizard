using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpellController))]
public class SpellAutoPilot : AutoPilot 
{
    public float detectionRadius = 3f;
    public float detectionDistance = 3f;
    public bool explodeOnlyOnTarget = false;

    private SpellController spell;
    private LayerMask enemyLayer;

    new void Awake()
    {
        base.Awake();
        spell = GetComponent<SpellController>();
        state = PilotState.Searching;
    }

    void Start()
    {
        if (spell.damage >= 0)
        {
            // if the spell is emitted by the hero, it needs to lock on monsters
            if (spell.emitter.gameObject.CompareTag("Hero") || spell.emitter.gameObject.CompareTag("HeroCompanion"))
                enemyLayer = GameManager.instance.layerManager.monsterLayer;
            else    // If not, it needs to lock on hero
                enemyLayer = GameManager.instance.layerManager.heroLayer;
        }
        else // If it's a healing spell, it's the other way around
        {
            if (spell.emitter.gameObject.CompareTag("Hero") || spell.emitter.gameObject.CompareTag("HeroCompanion"))
                enemyLayer = GameManager.instance.layerManager.heroLayer;
            else
                enemyLayer = GameManager.instance.layerManager.monsterLayer;
        }
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
                if (searchNewTargetIfDead)
                    steerToTarget(targetObject, PilotState.Searching);
                else
                    steerToTarget(targetObject, PilotState.DoNothing);
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

        Damageable closestDmg = getClosest(hits);
        if (closestDmg == null)
            return;

        if (closestDmg && !closestDmg.isDead)
        {
            Debug.Log(gameObject.name + " from " + spell.emitter + " locking on " + closestDmg.name);
            lockToObject(closestDmg.transform);
        }
    }

    Damageable getClosest(RaycastHit2D[] hits)
    {
        if (hits.Length <= 0)
            return null;

        Damageable result = null;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < hits.Length; i++)
        {
            Damageable dmg = hits[i].collider.GetComponent<Damageable>();
            if (!dmg || dmg.isDead)
                continue;

            if (hits[i].collider.gameObject == spell.emitter)   // Don't lock on its emitter
                continue;

            if (spell.ignoredColliders.Contains(hits[i].collider))
                continue;

            float sqrDistance = (hits[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                result = dmg;
            }
        }

        return result;
    }

    
}
   
