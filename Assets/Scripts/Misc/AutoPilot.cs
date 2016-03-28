using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AutoPilot : MonoBehaviour
{
    public bool activated = true;
    public bool slowMode = false;

    public float rotatingStep = 0.1f;

    public float velocityIfStopped = 3f;

    [HideInInspector]
    public Vector3 targetPosition;
    [HideInInspector]
    public GameObject targetObject;
    protected enum PilotState { DoNothing, Searching, LockedToObject, LockedToPosition };    // LockedToDamageable lock to a damageable entity. Lock to position lock to a fixed position in space
    protected Rigidbody2D rigidBody;
    protected PilotState state;
    

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Steer to a target object. End state defines the state the object should fall in if the object does not exist anymore
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="endState"></param>
    protected void steerToTarget(GameObject obj, PilotState endState)
    {
        if (obj == null)
        {
            state = endState;
            return;
        }
        targetPosition = obj.transform.position;
        steerToTarget();
    }

    protected void steerToTarget()
    {
        Vector3 lineToTarget = targetPosition - transform.position;
        if (Vector3.Angle(lineToTarget, rigidBody.velocity) == 180)
        {
            lineToTarget = Quaternion.Euler(0, 0, Random.Range(-1f, 1f)) * lineToTarget;   // Tiny fix to avoid extreme case when angle = 180
        }

        if (slowMode)
            rigidBody.velocity = lineToTarget;
        else
        {
            if (rigidBody.velocity == Vector2.zero) // If the rigibody is null (static item for example)
            {
                rigidBody.velocity = lineToTarget.normalized * velocityIfStopped;
            }
            else
                rigidBody.velocity = Vector3.RotateTowards(rigidBody.velocity, lineToTarget, rotatingStep, 0);
        }
    }

    public void lockToObject(GameObject obj)
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
