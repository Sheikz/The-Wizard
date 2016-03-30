using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class TeleportBehindTarget : SpellController
{
    public SpellController teleport;
    public float distanceToTarget;

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        float randomAngle = Random.Range(0f, 360f);

        Vector3 randomDirection = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;
        Vector3 targetPosition = target + (randomDirection * distanceToTarget);

        return teleport.castSpell(emitter, emitter.transform.position, targetPosition);
    }
}
