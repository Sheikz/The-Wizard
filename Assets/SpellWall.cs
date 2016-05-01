using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class SpellWall : StaticSpell
{
    public float length;

    ParticleSystem[] particleSystems;
    BoxCollider2D boxCollider;

    Vector3 directionRight;
    Vector3 directionLeft;

    RaycastHit2D hitLeft;
    RaycastHit2D hitRight;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        RaycastHit2D hit = Physics2D.Linecast(emitter.transform.position, target, GameManager.instance.layerManager.highBlockingLayer);
        if (hit)
            return null;

        SpellWall newWall = Instantiate(this);
        newWall.transform.position = target;
        newWall.initialize(emitter, target);
        return newWall;
    }

    void initialize(SpellCaster emitter, Vector3 target)
    {
        base.initialize(emitter, emitter.transform.position, target);
        Vector3 lineToTarget = target - emitter.transform.position;
        float angle = Vector3.Angle(lineToTarget, Vector3.up);
        if (angle >= 90)
            angle -= 180;
        if (lineToTarget.x >= 0)
            angle *= -1;
        transform.Rotate(0, 0, angle);

        resizeAccordingToWalls(angle);
    }

    private void resizeAccordingToWalls(float angle)
    {
        directionRight = Quaternion.Euler(0, 0, angle) * Vector3.right;
        directionLeft = Quaternion.Euler(0, 0, angle) * Vector3.left;
        hitRight = Physics2D.Raycast(transform.position, directionRight, length / 2, GameManager.instance.layerManager.blockingLayer | GameManager.instance.layerManager.holeLayer);
        hitLeft = Physics2D.Raycast(transform.position, directionLeft, length / 2, GameManager.instance.layerManager.blockingLayer | GameManager.instance.layerManager.holeLayer);
        if (hitLeft && hitRight)
        {
            float translationToRight = (hitRight.distance - hitLeft.distance) / 2;
            length = hitRight.distance + hitLeft.distance;
            transform.position += transform.rotation * new Vector3(translationToRight, 0, 0);
        }
        else if (hitRight)
        {
            float diffDistance = (length / 2) - hitRight.distance;
            length -= diffDistance;
            transform.position -= transform.rotation * new Vector3(diffDistance/2, 0, 0);
        }
        else if (hitLeft)
        {
            float diffDistance = (length / 2) - hitLeft.distance;
            length -= diffDistance;
            transform.position += transform.rotation * new Vector3(diffDistance / 2, 0, 0);
        }
    }

    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + directionRight);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + directionLeft);
        Gizmos.color = Color.blue;
        if (hitLeft)
            Gizmos.DrawSphere(hitLeft.point, 0.2f);
        Gizmos.color = Color.magenta;
        if (hitRight)
            Gizmos.DrawSphere(hitRight.point, 0.2f);
    }

    new void Start()
    {
        base.Start();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        setupParticleSystems();
        setupCollider();
    }

    void setupParticleSystems()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ParticleSystem.ShapeModule tmp = ps.shape;
            tmp.radius = length / 2;
            ps.GetComponent<ParticleSystemRenderer>().renderMode = ParticleSystemRenderMode.Billboard;
            var emissionModule = ps.emission;
            emissionModule.enabled = true;
            ParticleSystem.MinMaxCurve curve = emissionModule.rate;
            curve.constantMax /= (5 / length);
            curve.constantMin = (5 / length);
            emissionModule.rate = curve;
        }
    }

    void setupCollider()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(length, 1f);
        boxCollider.offset = new Vector2(0, 0.15f);
        boxCollider.isTrigger = true;
    }
}
