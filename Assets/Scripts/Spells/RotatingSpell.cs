using UnityEngine;
using System.Collections.Generic;
using System;

public class RotatingSpell : SpellController
{
    private enum RotatingSpellState { Rotating, ShootingTogether, ShootingSequential, Shooted};
    public float distanceToParent = 1f;
    public float rotationSpeed = 5f;
    public float speed = 5f;
    public float visionDistance = 7f;

    private float startTime;
    private RotatingSpellState state = RotatingSpellState.Rotating;
    private CircleSpell circleSpell;

    new void Start()
    {
        startTime = Time.time;
        collidesWithWalls = false;
        base.Start();
    }

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        RotatingSpell newSpell = Instantiate(this);
        newSpell.transform.position = position + Vector3.right * distanceToParent;
        newSpell.transform.parent = emitter.transform;
        newSpell.emitter = emitter;
        return newSpell;
    }

    public RotatingSpell castSpellWithReturn(SpellCaster emitter, Vector3 position, Vector3 target, CircleSpell parent)
    {
        RotatingSpell newSpell = Instantiate(this);
        newSpell.transform.parent = parent.transform;
        newSpell.transform.localPosition = Vector3.right * distanceToParent;
        newSpell.emitter = emitter;
        return newSpell;
    }

    void FixedUpdate()
	{
        if (!emitter)
            shootImmediate();
        switch (state)
        {
            case RotatingSpellState.Rotating:
                rotateAroundParent();
                break;
            case RotatingSpellState.ShootingSequential:
                rotateAroundParent();
                checkForTarget();
                break;
            case RotatingSpellState.ShootingTogether:
                rotateAroundParent();
                checkForTargetTogether();
                break;
        }
	}

    private void checkForTarget()
    {
        LayerMask layer = enemyLayer | GameManager.instance.layerManager.spellBlockingLayer;
        RaycastHit2D hit = Physics2D.Raycast(emitter.transform.position, transform.localPosition, visionDistance, layer);
        if (hit)
        {
            Damageable dmg = hit.collider.GetComponent<Damageable>();
            if (!dmg)
                return;

            shootImmediate();
        }
    }

    private void checkForTargetTogether()
    {
        LayerMask layer = enemyLayer | GameManager.instance.layerManager.spellBlockingLayer;
        RaycastHit2D hit = Physics2D.Raycast(emitter.transform.position, transform.localPosition, visionDistance, layer);
        if (hit)
        {
            Damageable dmg = hit.collider.GetComponent<Damageable>();
            if (!dmg)
                return;
            
            circleSpell.shootInDirection(transform.localPosition);
        }
    }

    void rotateAroundParent()
    {
        float time = Time.time - startTime;
        transform.localPosition = new Vector3(Mathf.Sin(time * rotationSpeed) * distanceToParent, 
                                              Mathf.Cos(time * rotationSpeed) * distanceToParent, 
                                              0);
    }

    public void shootImmediate()
    {
        state = RotatingSpellState.Shooted;
        rb.velocity = transform.localPosition.normalized * speed;
        transform.parent = null;
        collidesWithWalls = true;
    }

    public void shootInDirection(Vector3 direction)
    {
        state = RotatingSpellState.Shooted;
        rb.velocity = direction.normalized * speed;
        transform.parent = null;
        collidesWithWalls = true;
    }

    public void shootTogether(CircleSpell parentCircle)
    {
        circleSpell = parentCircle;
        state = RotatingSpellState.ShootingTogether;
    }

    public void shootSequential()
    {
        state = RotatingSpellState.ShootingSequential;
    }
}
