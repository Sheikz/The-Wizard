using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Companion : MonoBehaviour
{
    public SpellCaster master;
    public float distanceToMaster = 0.5f;
    public float rotationSpeed = 1f;
    public float visionRadius = 5f;

    private LayerMask enemyLayer;
    private enum CompanionState { Searching, Locked };
    private CompanionState state = CompanionState.Searching;

    private Damageable target;
    private Collider2D[] potentialTargets;
    private SpellCaster spellCaster;
    private float duration;

    void Awake()
    {
        spellCaster = GetComponent<SpellCaster>();
    }

    void Start()
    {
        transform.localPosition = Vector3.up * distanceToMaster;
        master = transform.parent.GetComponent<SpellCaster>();
        if (master.tag == "Hero")
        {
            enemyLayer = GameManager.instance.layerManager.monsterLayer;
            tag = "HeroCompanion";
        }
        else
            enemyLayer = GameManager.instance.layerManager.heroLayer;

        StartCoroutine(destroyAfterSeconds(duration));

    }

    internal void initialize(float duration)
    {
        this.duration = duration;
    }

    private IEnumerator destroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        master.removeCompanion(this);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        rotateAroundMaster();
        actAccordingToState();
    }
    
    void actAccordingToState()
    {
        switch (state)
        {
            case CompanionState.Searching:
                searchTarget();
                break;
            case CompanionState.Locked:
                shootToTarget();
                break;
        }
    }

    void rotateAroundMaster()
    {
        transform.localPosition = (Quaternion.Euler(0, 0, rotationSpeed) * transform.localPosition).normalized * distanceToMaster;
    }

    void searchTarget()
    {
        if (!spellCaster)
            return;

        potentialTargets = Physics2D.OverlapCircleAll(transform.position, visionRadius, enemyLayer);
        foreach (Collider2D potentialTarget in potentialTargets)
        {
            Damageable dmg = potentialTarget.GetComponent<Damageable>();
            if (!dmg)
                continue;

            if (inLineOfSight(dmg))
            {
                target = dmg;
                state = CompanionState.Locked;
                return;
            }
        }

    }

    bool inLineOfSight(Damageable dmg)
    {
        if (!dmg)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, dmg.transform.position, GameManager.instance.layerManager.spellBlockingLayer);
        if (!hit)
        {
            return true;
        }
        return false;
    }

    void shootToTarget()
    {
        if (!spellCaster)
            return;

        if (inLineOfSight(target))   // If it has a clear shot
            spellCaster.castAvailableSpells(target.transform.position);
        else
            state = CompanionState.Searching;
    }
}
