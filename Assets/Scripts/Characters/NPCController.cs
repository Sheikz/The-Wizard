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
    public float computePathToTargetCooldown = 1f;

    protected Vector3 target;     // short-term target
    protected Vector3 goal;       // long-term target
    private Stack<Tile> path;
    private GridMap gridMap;       // Reference to the map
    private Room room;

    private NPCState state;
    private Damageable targetOpponent;  // Target opponent to damage
    private Damageable targetAlly;  // Target ally to heal

    private bool hasStrafeValueExpired = true;
    private float strafeValue = 1f;
    private SpellCaster spellCaster;
    private Vector2 mobRadius;
    private Vector3 lineToTarget;
    private Vector3 directionToTarget;
    private float distanceToTarget;
    private LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    private LayerMask alliesLayer;
    private LayerMask visionLayer;
    private Collider2D[] potentialTargets;
    private Collider2D[] potentialInjuredAllies;
    private bool isDead = false;
    private bool hasSupportSpells;  // Should this NPC check for nearby allies to heal?
    private bool pathToTargetOpponentOnCooldown = false;

    private enum NPCState { Idle, Wander, Chase };

    new protected void Start()
    {
        base.Start();

        spellCaster = GetComponent<SpellCaster>();
        if (CompareTag("Hero") || CompareTag("HeroCompanion"))
        {
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
            alliesLayer = GameManager.instance.layerManager.heroLayer;
        }
        else
        {
            enemyLayer = GameManager.instance.layerManager.heroLayer;
            alliesLayer = GameManager.instance.layerManager.monsterLayer;
        }

        if (isFlying)   // If it is flying it only needs to avoid walls
            obstacleLayer = GameManager.instance.layerManager.highBlockingLayer;
        else            // If not, it need to avoid obstacles as well
            obstacleLayer = GameManager.instance.layerManager.obstacleLayer;

        if (isGhost)
            visionLayer = GameManager.instance.layerManager.nothing;
        else
            visionLayer = GameManager.instance.layerManager.monsterVisionLayer;

        hasSupportSpells = spellCaster.hasSupportSpells();
        if (hasSupportSpells)
            InvokeRepeating("checkForNearbyAllies", 0, 1f);
    }

    internal void activate(bool v)
    {
        foreach (Behaviour beh in GetComponents<Behaviour>())
        {
            if (beh)
                beh.enabled = v;
        }

        gameObject.SetActive(v);
    }

    public void initialize(RoomBasedMapGenerator map)
    {
        gridMap = map.gridMap;
        mobRadius = new Vector2(transform.localScale.x * circleCollider.radius, transform.localScale.y * circleCollider.radius);

        setNewGoal();
        computePathToGoal();
        startWander();
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
        if (hasSupportSpells && targetAlly) // If it's a healer and there is a potential injured ally, then heal
            healTargetAlly();
    }

    /// <summary>
    /// Check if there is a nearby ally that need healing. If yes, heal it
    /// </summary>
    private void checkForNearbyAllies()
    {
        potentialInjuredAllies = Physics2D.OverlapCircleAll(transform.position, visionDistance, alliesLayer);
        int maxMissingLife = 0;
        targetAlly = null;
        foreach (Collider2D potentialTarget in potentialInjuredAllies)
        {
            if (potentialTarget.gameObject == gameObject)   // Dont heal itself
                continue;

            Damageable dmg = potentialTarget.GetComponent<Damageable>();
            if (!dmg)
                continue;
            if (dmg.missingLife() > maxMissingLife)
                maxMissingLife = dmg.missingLife();
            else
                continue;

            if (inLineOfSight(dmg))
            {
                targetAlly = dmg;
            }
        }
    }

    void healTargetAlly()
    {
        if (!targetAlly)
            return;

        if (targetAlly.missingLife() <= 0)  // Only heal if it's missing life
            return;

        if (spellCaster && hasSupportSpells && spellCaster.hasHealingSpellsAvailable() && hasClearShot(targetAlly))    // If this hero is a support spell caster
            spellCaster.castHealingSpells(targetAlly);
    }

    private void doIdle()
    {
        return;
    }

    protected virtual void doWander()
    {
        if (!canMove)
            return;

        if (statusEffectReceiver && statusEffectReceiver.isStunned)
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
                spellCaster.targetOpponent = dmg.transform;
                startChasing();
                return true;
            }
        }
        return false;
    }

    bool inLineOfSight(Damageable dmg)
    {
        if (!dmg && !dmg.isDead)
            return false;

        if ((dmg.transform.position - transform.position).sqrMagnitude > visionDistance * visionDistance)
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
        if (statusEffectReceiver && statusEffectReceiver.isStunned)
            return;

        if (!targetOpponent)
        {
            startWander();
            return;
        }

        lineToTarget = targetOpponent.transform.position - transform.position;
        distanceToTarget = lineToTarget.magnitude;

        if (spellCaster && spellCaster.hasOffensiveSpellsAvailable() && hasClearShot(targetOpponent))    // If this NPC is a spell caster
            spellCaster.castOffensiveSpells(targetOpponent);

        direction = lineToTarget;   // Face the target
        if (!canMove)
            return;

        doStrafe(distanceToTarget);
        goal = targetOpponent.transform.position;

        moveToTarget();
    }

    /// <summary>
    /// Verify if the target is still in line of sight
    /// </summary>
    void lookForTarget()
    {
        if (!inLineOfSight(targetOpponent))
            startWander();
    }

    private void startWander()
    {
        if (spellCaster)
            spellCaster.targetOpponent = null;
        CancelInvoke("lookForTarget");
        InvokeRepeating("searchTarget", 0, 0.5f);
        state = NPCState.Wander;
        computePathToGoal();
    }

    private void startIdle()
    {
        if (spellCaster)
            spellCaster.targetOpponent = null;
        CancelInvoke("lookForTarget");
        InvokeRepeating("searchTarget", 0, 0.5f);
        state = NPCState.Idle;
        if (gameObject.activeSelf)
            StartCoroutine(startIdleRoutine());
    }

    private void startChasing()
    {
        CancelInvoke("searchTarget");
        InvokeRepeating("lookForTarget", 0, 0.5f);
        state = NPCState.Chase;
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

    private void doStrafe(float distanceToTarget) // Strafe according to the distance to the hero
    {
        directionToTarget = lineToTarget.normalized;
        if (distanceToTarget < preferedDistance.minimum)  // Too close to hero, go back
        {
            target = transform.position + directionToTarget * -1;
        }
        else if (distanceToTarget > preferedDistance.minimum && distanceToTarget < preferedDistance.maximum)    // In the prefered distance, do random strafe
        {
            applyStrafe();
        }
        else // Too far away from the away, need to go closer
        {
            moveToTargetOpponent();
        }
            
        movement = (target - transform.position).normalized * movingSpeed;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, circleCollider.radius, obstacleLayer);
        if (hit)
        {
            target = transform.position + (target - transform.position) * -1;
            StartCoroutine(switchStrafeValue(Random.Range(strafeFrequency.minimum, strafeFrequency.maximum)));
        }
    }

    private void applyStrafe()
    {
        updateRandomTemporaryStrafeValue();
        target = transform.position + (Vector3.Cross(directionToTarget, Vector3.back)) * strafeValue;
    }

    private void moveToTargetOpponent()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius*0.75f, lineToTarget, distanceToTarget, obstacleLayer);
        if (!hit)   // If the way is clear, the NPC can move in a straight line to the opponent
        {
            target = targetOpponent.transform.position;
            return;
        }
        if (pathToTargetOpponentOnCooldown == true)
        {
            if (hasReached(target) && path != null && path.Count > 0)
                target = path.Pop().position();
            return;
        }

        StartCoroutine(computePathToTargetStartCooldown());
        if (!computePathTo(targetOpponent.transform.position))  // If no path if found, apply strafe and return
        {
            applyStrafe();
            return;
        }
        if (path != null && path.Count > 0)
            target = path.Pop().position();
        else
        {
            Debug.LogWarning("This should not happen. Investigate");
            target = targetOpponent.transform.position;
        }
    }

    private IEnumerator computePathToTargetStartCooldown()
    {
        pathToTargetOpponentOnCooldown = true;
        yield return new WaitForSeconds(computePathToTargetCooldown);
        pathToTargetOpponentOnCooldown = false;
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
        movement = (target - transform.position).normalized * movingSpeed;
        direction = movement;
    }

    protected void moveToTarget()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, target, movingSpeed * Time.fixedDeltaTime));
    }

    protected bool hasReached(Vector3 t)
    {
        float distanceToT = (t - transform.position).sqrMagnitude;
        if (distanceToT <= float.Epsilon)
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
            if (gameObject.activeSelf)
                StartCoroutine(startIdleRoutine());
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

    private IEnumerator startIdleRoutine()
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
        startWander();
    }

    protected bool computePathToGoal()
    {
        path = gridMap.getPath(new Vector2i(transform.position), new Vector2i(goal), this);
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

    private bool computePathTo(Vector3 position)
    {
        path = gridMap.getPath(new Vector2i(transform.position), new Vector2i(position), this);
        if (path == null)
            return false;
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
        SoundManager.instance.playSound("EnemyDeath");
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
        SoundManager.instance.playSound("EnemyDamage");
    }
}