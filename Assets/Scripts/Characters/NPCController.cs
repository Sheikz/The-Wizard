using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System;

public class NPCController : MovingCharacter
{
    [Range(0f, 1f)]
    [Tooltip("The chance that the NPC will be idle after having reach his goal")]
    public float idleChance = 0.3f;
    [Tooltip("The min and max time the NPC will stay idle")]
    public Countf idleTime = new Countf(1, 3);
    public float goalDistance = 10f;
    public float visionDistance = 10f;
    public Count preferedDistance = new Count(3, 5);
    public Countf strafeFrequency = new Countf(0.2f, 0.5f);
    public bool isGhost = false;
    public float deathAnimationTime = 0f;

    protected Vector3 target;     // short-term target
    protected Vector3 goal;       // long-term target
    private Stack<Tile> path;
    private GridMap gridMap;       // Reference to the map
    private Room room;

    private NPCState state;
    private Damageable targetOpponent;

    private bool hasStrafeValueExpired = true;
    private float strafeValue = 1f;
    private SpellCaster spellCaster;
    private Vector2 mobRadius;
    private LayerMask enemyLayer;
    private LayerMask obstacleLayer;
    private LayerMask visionLayer;
    private Collider2D[] potentialTargets;
    private bool isDead = false;

    private enum NPCState { Idle, Wander, Chase };

    void Start()
    {
        spellCaster = GetComponent<SpellCaster>();
        if (CompareTag("Hero") || CompareTag("HeroCompanion"))
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
        else
            enemyLayer = GameManager.instance.layerManager.heroLayer;

        if (isFlying)   // If it is flying it only needs to avoid walls
            obstacleLayer = GameManager.instance.layerManager.highBlockingLayer;
        else            // If not, it need to avoid obstacles as well
            obstacleLayer = GameManager.instance.layerManager.obstacleLayer;

        if (isGhost)
            visionLayer = GameManager.instance.layerManager.nothing;
        else
            visionLayer = GameManager.instance.layerManager.monsterVisionLayer;
    }

    public void initialize(RoomBasedMapGenerator map)
    {
        gridMap = map.gridMap;
        mobRadius = new Vector2(transform.localScale.x * circleCollider.radius, transform.localScale.y * circleCollider.radius);

        setNewGoal();
        computePathToGoal();
        state = NPCState.Wander;
    }

    public void initialize(Room room)
    {
        this.room = room;
        initialize(room.map);
    }

    // This update is called once every fixed framerate
    void FixedUpdate()
    {
        actAccordingToState();
        updateAnimations();
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, target, Color.green);
        Debug.DrawLine(transform.position, goal, Color.red);
    }

    void actAccordingToState()
    {
        if (isDead)
            return;
        switch (state)
        {
            case NPCState.Wander:
                doWander();
                break;
            case NPCState.Chase:
                doChase();
                break;
            case NPCState.Idle:
                doIdle();
                break;
        }
    }

    private void doIdle()
    {
        searchTarget();
    }

    protected virtual void doWander()
    {
        if (searchTarget())
            return;

        if (!canMove)
            return;

        if (hasReached(goal))
            setNewGoal();
        if (hasReached(target))
            setNewTarget();

        updateMovementToReachTarget();
        moveToTarget();
    }

    /// <summary>
    /// Search for a target in the surroundings. If found, return with true
    /// </summary>
    /// <returns></returns>
    protected bool searchTarget()
    {
        if (!spellCaster)
            return false;

        potentialTargets = Physics2D.OverlapCircleAll(transform.position, visionDistance, enemyLayer);
        foreach (Collider2D potentialTarget in potentialTargets)
        {
            Damageable dmg = potentialTarget.GetComponent<Damageable>();
            if (!dmg)
                continue;

            if (inLineOfSight(dmg))
            {
                targetOpponent = dmg;
                spellCaster.targetObject = dmg.gameObject;
                state = NPCState.Chase;
                return true;
            }
        }
        return false;
    }

    bool inLineOfSight(Damageable dmg)
    {
        if (!dmg)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, dmg.transform.position, visionLayer);
        if (!hit)
        {
            return true;
        }
        return false;
    }

    void doChase()
    {
        
        if (!targetOpponent || !inLineOfSight(targetOpponent))
        {
            spellCaster.targetObject = null;
            state = NPCState.Wander;
            computePathToGoal();
            return;
        }

        Vector3 lineToTarget = targetOpponent.transform.position - transform.position;
        float distanceToTarget = lineToTarget.magnitude;

        if (spellCaster && hasClearShot(targetOpponent))    // If this hero is a spell caster
            spellCaster.castAvailableSpells(targetOpponent);

        direction = lineToTarget;   // Face the target
        if (!canMove)
            return;

        doStrafe(distanceToTarget, lineToTarget);
        goal = targetOpponent.transform.position;

        moveToTarget();
    }

    private bool hasClearShot(Damageable dmg)
    {
        if (!dmg)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, dmg.transform.position, GameManager.instance.layerManager.monsterVisionLayer);
        if (!hit)
        {
            return true;
        }
        return false;
    }

    private void doStrafe(float distanceToTarget, Vector3 lineToTarget) // Strafe according to the distance to the hero
    {
        Vector3 directionToHero = lineToTarget.normalized;
        if (distanceToTarget < preferedDistance.minimum)  // Too close to hero, go back
        {
            target = transform.position + directionToHero * -1;
        }
        else if (distanceToTarget > preferedDistance.minimum && distanceToTarget < preferedDistance.maximum)    // In the prefered distance, do random strafe
        {
            updateRandomTemporaryStrafeValue();
            target = transform.position + (Vector3.Cross(directionToHero, Vector3.back)) * strafeValue;
        }
        else
            target = targetOpponent.transform.position;

        movement = (target - transform.position).normalized * speed;
        
        if (isFlying)   // if it is flying, it does not need to avoid obstacles
            return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, circleCollider.radius, obstacleLayer);
        if (hit)
        {
            target = transform.position + (target - transform.position) * -1;
            StartCoroutine(switchStrafeValue(Random.Range(strafeFrequency.minimum, strafeFrequency.maximum)));
        }
    }

    private void updateRandomTemporaryStrafeValue()
    {
        if (hasStrafeValueExpired)
            StartCoroutine(resetStrafeValue(Random.Range(strafeFrequency.minimum, strafeFrequency.maximum)));
    }

    private IEnumerator resetStrafeValue(float duration)
    {
        strafeValue = Random.Range(0, 2) * 2 - 1; // Random value between -1 and +1
        hasStrafeValueExpired = false;
        yield return new WaitForSeconds(duration);
        hasStrafeValueExpired = true;
    }

    private IEnumerator switchStrafeValue(float duration)
    {
        strafeValue = strafeValue * -1; // Random value between -1 and +1
        hasStrafeValueExpired = false;
        yield return new WaitForSeconds(duration);
        hasStrafeValueExpired = true;
    }


    protected void updateMovementToReachTarget()
    {
        movement = (target - transform.position).normalized * speed;
        direction = movement;
    }

    protected void moveToTarget()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime));
    }

    protected bool hasReached(Vector3 t)
    {
        float distanceToTarget = (t - transform.position).sqrMagnitude;
        if (distanceToTarget <= float.Epsilon)
            return true;

        return false;
    }

    protected void setNewTarget()
    {
        if (path == null || path.Count == 0)
        {
            setNewGoal();
            return;
        }
        target = path.Pop().position();
    }

    /// <summary>
    /// Set a random target close. Used when mob is stuck
    /// </summary>
    private void setRandomTarget()
    {
        Tile t = gridMap.getRandomNotBlockingNeighbor(transform.position, isFlying, isGhost);
        if (t == null)
            return;
        target = t.position();
    }

    void setNewGoal()
    {
        if (Random.Range(0f, 1f) <= idleChance)
        {
            StartCoroutine(startIdle());
            return;
        }

        float maxMobRadius = getRadius();
        if (maxMobRadius > 0.5f)
        {
            goal = gridMap.findRandomTileWithinRadius(transform.position, goalDistance, maxMobRadius, isFlying, TileType.Floor).position();
        }
        else
        {
            goal = gridMap.findRandomTileWithinRadius(transform.position, goalDistance, TileType.Floor).position();
        }
        computePathToGoal();
    }

    private IEnumerator startIdle()
    {
        state = NPCState.Idle;
        movement = Vector2.zero;
        float duration = idleTime.getRandom();
        float startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            if (state != NPCState.Idle)
                yield break;
            yield return new WaitForFixedUpdate();
        }
        state = NPCState.Wander;
    }

    protected bool computePathToGoal()
    {
        path = gridMap.getPath(new Vector2i(transform.position), new Vector2i(goal), getRadius(), isFlying, isGhost);
        if (path == null)
        {
            Debug.Log(name + " no path found");
            setRandomTarget();
            Invoke("setNewGoal", 3f);
            return false;
        }
        setNewTarget();
        return true;
    }

    public float getRadius()
    {
        if (mobRadius.x == 0 && mobRadius.y == 0)
            return Mathf.Max(transform.localScale.x * GetComponent<CircleCollider2D>().radius, transform.localScale.y * GetComponent<CircleCollider2D>().radius);
        else
            return Mathf.Max(mobRadius.x, mobRadius.y);

    }
        
    public override void die()
    {
        circleCollider.enabled = false;
        isDead = true;
        movement = Vector2.zero;
        if (room)
        {
            room.monsterDied(this);
        }
        StartCoroutine(dieAfterSeconds(deathAnimationTime));
    }

    private IEnumerator dieAfterSeconds(float deathAnimationTime)
    {
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(deathAnimationTime);
        Instantiate(UIManager.instance.deathAnimation, transform.position, Quaternion.identity);
        StartCoroutine(fadeAfterSeconds(deathAnimationTime));
    }

    IEnumerator fadeAfterSeconds(float duration)
    {
        float startingTime = Time.time;
        while (Time.time - startingTime < duration)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = Mathf.Lerp(1, 0, (Time.time - startingTime) / duration);
            spriteRenderer.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }

    public override void receivesDamage()
    {
    }
}