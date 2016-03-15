using UnityEngine;
using System.Collections;
using System;

public class SpellDeflector : MonoBehaviour
{
    public Countf angleDeviation;

    private SpellCaster emitter;

    // Use this for initialization
    void Start()
    {
        emitter = GetComponentInParent<SpellController>().emitter;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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
            float deviation = angleDeviation.getRandom();
            otherRB.velocity *= -1;
            otherRB.velocity.rotate(deviation);
            otherSpell.transform.Rotate(0, 0, 180 - deviation);
            otherSpell.emitter = emitter;
            if (emitter.isMonster)
                otherSpell.gameObject.layer = LayerMask.NameToLayer("MonsterSpells");
            else
                otherSpell.gameObject.layer = LayerMask.NameToLayer("Spells");
        }
    }
}
