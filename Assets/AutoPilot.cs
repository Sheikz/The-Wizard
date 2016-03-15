using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpellController))]
[RequireComponent(typeof(Rigidbody2D))]
public class AutoPilot : MonoBehaviour 
{
    private enum PilotState { DoNothing, Searching, LockedToObject, LockedToPosition };    // LockedToDamageable lock to a damageable entity. Lock to position lock to a fixed position in space
    public float detectionRadius = 3f;
    public float detectionDistance = 3f;
    public float rotatingStep = 0.1f;
    public bool slowMode = false;

    private SpellController spell;
    private Rigidbody2D rigidBody;
    private PilotState state = PilotState.Searching;
    private GameObject targetObject;
    [HideInInspector]
    public Vector3 targetPosition;
    private LayerMask enemyLayer;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        // if the spell is emitted by the hero, it needs to lock on monsters
        if (spell.emitter.gameObject.CompareTag("Hero") || spell.emitter.gameObject.CompareTag("HeroCompanion"))
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
        else    // If not, it needs to lock on hero
            enemyLayer = GameManager.instance.layerManager.heroLayer;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case PilotState.DoNothing:
                return;
            case PilotState.Searching:
                searchTarget();
                break;
            case PilotState.LockedToObject:
                steerToTarget(targetObject);
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
            LockToObject(dmg.gameObject);
        }
    }

    void steerToTarget(GameObject obj)
    {
        if (obj == null)
        {
            state = PilotState.Searching;
            return;
        }
        targetPosition = obj.transform.position;
        steerToTarget();
    }

    void steerToTarget()
    {
        Vector3 lineToTarget = targetPosition - transform.position;
        if (Vector3.Angle(lineToTarget, rigidBody.velocity) == 180)
        {
            lineToTarget = Quaternion.Euler(0, 0, Random.Range(-1f, 1f)) * lineToTarget;   // Tiny fix to avoid extreme case when angle = 180
        }

        if (slowMode)
            rigidBody.velocity = lineToTarget;
        else
            rigidBody.velocity = Vector3.RotateTowards(rigidBody.velocity, lineToTarget, rotatingStep, 0);
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

    public void LockToObject(GameObject obj)
    {
        targetObject = obj;
        state = PilotState.LockedToObject;
    }

    public void lockToTargetPosition(Vector3 target)
    {
        targetPosition = target;
        state = PilotState.LockedToPosition;
    }

    public void disableAutoPilot()
    {
        state = PilotState.DoNothing;
    }
}
   
