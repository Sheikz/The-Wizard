using UnityEngine;
using System.Collections;
using System;

public class DashAttack : SpellController
{
    public float startSpeed = 10f;
    public float endSpeed = 1f;
    public float dashTime = 0.5f;
    public float minimumDistance = 3f;

    private Rigidbody2D emitterRigibody;
    private MovingCharacter movingChar;

    public override SpellController castSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        // Only dash if the target is close enough
        if (emitter.isMonster && (emitter.transform.position - target).sqrMagnitude > minimumDistance * minimumDistance)
            return null;

        DashAttack attack = Instantiate(this);
        attack.initialize(emitter, target);
        return attack;
    }

    public override bool canCastSpell(SpellCaster emitter, Vector3 initialPos, Vector3 target)
    {
        if (emitter.isMonster && (emitter.transform.position - target).sqrMagnitude > minimumDistance * minimumDistance)
            return false;
        else
            return true;
    }

    private void initialize(SpellCaster emitter, Vector3 target)
    {
        transform.SetParent(emitter.transform);
        transform.position = emitter.transform.position;
        this.emitter = emitter;
        this.target = target;
        emitterRigibody = emitter.GetComponent<Rigidbody2D>();
        if (!emitterRigibody)
        {
            Debug.LogError(name + " Error. Emitter " + emitter.name + " needs a rigibody2d attached");
        }
        movingChar = emitter.GetComponent<MovingCharacter>();
        StartCoroutine(startDashAttack());
    }

    private IEnumerator startDashAttack()
    {
        if (movingChar)
            movingChar.enableAction(false);
        float startTime = Time.time;
        while (Time.time - startTime <= dashTime)
        {
            if (!emitterRigibody)
                break;

            if ((emitter.transform.position - target).sqrMagnitude <= 0.02f)
                break;

            float step = Mathf.Lerp(startSpeed, endSpeed, (Time.time - startTime) / dashTime) * Time.fixedDeltaTime;
            emitterRigibody.MovePosition(Vector2.MoveTowards(emitter.transform.position, target, step));
            yield return new WaitForFixedUpdate();
        }
        if (movingChar)
            movingChar.enableAction(true);
        Destroy(gameObject);
    }
}
