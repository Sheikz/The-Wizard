using UnityEngine;
using System.Collections;
using System;

public class SpellDeflector : MonoBehaviour
{
    public Countf angleDeviation;
    [Tooltip("Does the spell get reflected in the direction of the mouse?")]
    public bool targetToMouseCursor = false;

    private SpellCaster emitter;
    private SpellController spell;
    public bool activated = true;

    // Use this for initialization
    void Start()
    {
        emitter = GetComponentInParent<SpellController>().emitter;
        spell = GetComponentInParent<SpellController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated)
            return;

        SpellController otherSpell = other.GetComponent<SpellController>();
        if (otherSpell) // It's a spell
        {
            if (otherSpell.emitter == emitter)  // Dont reflect our own spells!
                return;

            deflectSpell(otherSpell);
        }
    }

    void deflectSpell(SpellController otherSpell)
    {
        Rigidbody2D otherRB = otherSpell.GetComponent<Rigidbody2D>();
        if (otherRB)
        {
            float deviation = 0;
            if (targetToMouseCursor && emitter.isHero)
            {
                Vector3 target = InputManager.instance.getCursorPosition();
                Vector3 desiredDirection = target - transform.position;
                float magnitude = otherRB.velocity.magnitude;
                deviation = Vector3.Angle(-otherRB.velocity, desiredDirection);
                otherRB.velocity = desiredDirection.normalized * magnitude;
            }
            else
            {
                deviation = angleDeviation.getRandom();
                otherRB.velocity *= -1;
            }
            otherRB.velocity.rotate(deviation);
            otherSpell.transform.Rotate(0, 0, 180 - deviation);
            otherSpell.emitter = emitter;
            if (emitter.isMonster)
                otherSpell.gameObject.layer = LayerMask.NameToLayer("MonsterSpells");
            else
                otherSpell.gameObject.layer = LayerMask.NameToLayer("Spells");

            UIManager.instance.createFloatingText("Deflect!",
                UIManager.instance.elementColors[(int)spell.magicElement],
                otherSpell.transform.position);
        }
    }
}
